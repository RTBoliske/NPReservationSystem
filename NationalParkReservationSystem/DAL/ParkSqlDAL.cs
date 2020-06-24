using NationalParkReservationSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace NationalParkReservationSystem.DAL
{
	public class ParkSqlDAL
    {
        private string _connectionString;

        //constructor
        public ParkSqlDAL (string connectionString)
        {
            _connectionString = connectionString;
        }

        //returns list of all parks in system
        public List<Park> GetParks()
        {
            List<Park> output = new List<Park>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT * FROM park ORDER BY park.name";
                cmd.Connection = connection;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Park park = PopulateParkFromReader(reader);

                    output.Add(park);
                }
            }

			return output;
        }

        //returns list of all campgrounds for selected park
        public List<Campground> GetCampgrounds(int park_id)
        {
            List<Campground> output = new List<Campground>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT campground.campground_id, campground.park_id, campground.name, " +
                                  "campground.daily_fee, campground.open_from_mm, campground.open_to_mm " +
                                  "FROM campground JOIN park ON campground.park_id = park.park_id " +
                                  "WHERE park.park_id = @parkID ORDER BY campground.name";
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@parkID", park_id);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Campground campground = PopulateCampgroundFromReader(reader);

                    output.Add(campground);
                }
            }

            return output;
        }

        //returns list of all sites for selected campground
        public List<Site> GetSites(int campground_id)
        {
            List<Site> output = new List<Site>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT site.site_id, site.site_number, site.max_occupancy, " +
                                  "site.accessible, site.max_rv_length, site.utilities " +
                                  "FROM site Join campground ON site.campground_id = campground.campground_id " +
                                  "WHERE campground.campground_id = @campgroundID ORDER BY site.site_number";
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@campgroundID", campground_id);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Site site = PopulateSiteFromReader(reader);

                    output.Add(site);
                }
            }

            return output;
        }

        //returns List of available sites based on dates and selected campground 
        public List<Site> AvailableSitesToReserve (int campground_id, string startDate, string endDate)
        {
            List<Site> results = new List<Site>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT DISTINCT site.site_id, site.site_number, site.campground_id, " +
                                  "site.accessible, site.max_occupancy, site.max_rv_length, site.utilities FROM site " +
                                  "LEFT JOIN reservation ON reservation.site_id = site.site_id " +
                                  "WHERE site.campground_id = @campgroundID " +
                                  "EXCEPT " +
                                  "SELECT DISTINCT site.site_id, site.site_number, site.campground_id, " +
                                  "site.accessible, site.max_occupancy, site.max_rv_length, site.utilities FROM site " +
                                  "LEFT JOIN reservation ON reservation.site_id = site.site_id " +
                                  "WHERE site.campground_id = @campgroundID " +
                                  "AND (@startDate BETWEEN reservation.from_date AND reservation.to_date " +
                                  "OR @endDate BETWEEN reservation.from_date and reservation.to_date);";

                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@campgroundID", campground_id);
                cmd.Parameters.AddWithValue("@startDate", startDate);
                cmd.Parameters.AddWithValue("@endDate", endDate);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Site site = PopulateAvailableSitesFromReader(reader);

                    results.Add(site);
                }
            }

            return results;
        }

        public int AddReservation(int site_id, string from_date, string to_date, string reservationName)
        {
            int record = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand();
					cmd.CommandText = "INSERT INTO reservation (site_id, name, from_date, to_date) " +
									  "OUTPUT INSERTED.reservation_id " +
									  "VALUES (@siteID, @name, @startDate, @endDate);";

					cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@siteID", site_id);
                    cmd.Parameters.AddWithValue("@name", reservationName);
                    cmd.Parameters.AddWithValue("@startDate", from_date);
                    cmd.Parameters.AddWithValue("@endDate", to_date);

					record = (int)cmd.ExecuteScalar();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return record;
        }

		public List<Reservation> GetConfirmedReservationInfo(int reservation_id)
		{
			List<Reservation> output = new List<Reservation>();

			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				connection.Open();

				SqlCommand cmd = new SqlCommand();
				cmd.CommandText = "SELECT * FROM reservation WHERE reservation_id = @reservationID;";

				//sql query to return reservation confirmation including park and campground names
				//cmd.CommandText = "SELECT reservation.reservation_id, reservation.name, reservation.from_date, " +
								//	"reservation.to_date, reservation.create_date, site.site_number, campground.name, " +
								//  "park.park_id FROM reservation " +
								//  "JOIN site ON reservation.site_id = site.site_id " +
								//  "JOIN campground ON site.campground_id = campground.campground_id " +
								//  "JOIN park ON campground.park_id = park.park_id " +
								//  "WHERE reservation.reservation_id = @reservationID;";

				cmd.Connection = connection;
				cmd.Parameters.AddWithValue("@reservationID", reservation_id);

				SqlDataReader reader = cmd.ExecuteReader();

				while (reader.Read())
				{
					Reservation reservation = PopulateReservationFromReader(reader);

					output.Add(reservation);
				}
			}

			return output;
		}

		public List<Reservation> GetReservationBySiteId(int site_id)
		{
			List<Reservation> output = new List<Reservation>();

			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				connection.Open();

				SqlCommand cmd = new SqlCommand();
				cmd.CommandText = "SELECT * FROM reservation WHERE site_id = @siteID;";
				cmd.Connection = connection;
				cmd.Parameters.AddWithValue("@siteID", site_id);

				SqlDataReader reader = cmd.ExecuteReader();

				while (reader.Read())
				{
					Reservation reservation = PopulateReservationFromReader(reader);

					output.Add(reservation);
				}
			}

			return output;
		}


		private Park PopulateParkFromReader(SqlDataReader reader)
        {
            Park item = new Park();

            item.ParkId = Convert.ToInt32(reader["park_id"]);
            item.ParkName = Convert.ToString(reader["name"]);
            item.Location = Convert.ToString(reader["location"]);
            item.EstablishDate = Convert.ToDateTime(reader["establish_date"]);
            item.Area = Convert.ToInt32(reader["area"]);
            item.AnnualVisitorCount = Convert.ToInt32(reader["visitors"]);
            item.Description = Convert.ToString(reader["description"]);

            return item;
        }

        private Campground PopulateCampgroundFromReader(SqlDataReader reader)
        {
            Campground item = new Campground();

            item.CampgroundId = Convert.ToInt32(reader["campground_id"]);
            item.ParkId = Convert.ToInt32(reader["park_id"]);
            item.CampgroundName = Convert.ToString(reader["name"]);
            item.MonthOpen = Convert.ToInt32(reader["open_from_mm"]);
            item.MonthClose = Convert.ToInt32(reader["open_to_mm"]);
            item.DailyFee = Convert.ToInt32(reader["daily_fee"]);

            return item;
        }

        private Site PopulateSiteFromReader(SqlDataReader reader)
        {
            Site item = new Site();

            item.SiteId = Convert.ToInt32(reader["site_id"]);
            item.SiteNumber = Convert.ToInt32(reader["site_number"]);
            item.MaxOccupancy = Convert.ToInt32(reader["max_occupancy"]);
            item.Accessibility = Convert.ToBoolean(reader["accessible"]);
            item.MaxRvLength = Convert.ToInt32(reader["max_rv_length"]);
            item.Utilities = Convert.ToBoolean(reader["utilities"]);

            return item;
        }

		private Reservation PopulateReservationFromReader(SqlDataReader reader)
		{
			Reservation item = new Reservation();

			item.ReservationId = Convert.ToInt32(reader["reservation_id"]);
			item.SiteId = Convert.ToInt32(reader["site_id"]);
			item.ReservationName = Convert.ToString(reader["name"]);
			item.StartReservationDate = Convert.ToDateTime(reader["from_date"]);
			item.EndReservationDate = Convert.ToDateTime(reader["to_date"]);
			item.CreateReservationDate = Convert.ToDateTime(reader["create_date"]);

			return item;
		}

		private Site PopulateAvailableSitesFromReader(SqlDataReader reader)
        {
            Site item = new Site();

            item.SiteId = Convert.ToInt32(reader["site_id"]);
            item.SiteNumber = Convert.ToInt32(reader["site_number"]);
            item.CampgroundId = Convert.ToInt32(reader["campground_id"]);
            item.Accessibility = Convert.ToBoolean(reader["accessible"]);
            item.MaxOccupancy = Convert.ToInt32(reader["max_occupancy"]);
            item.MaxRvLength = Convert.ToInt32(reader["max_rv_length"]);
            item.Utilities = Convert.ToBoolean(reader["utilities"]);

            return item;
        }
    }
}
