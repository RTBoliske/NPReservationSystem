using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using NationalParkReservationSystem.Models;

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

        //returns list of all parks
        public List<Park> GetParks()
        {
            List<Park> parkList = new List<Park>();

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

					parkList.Add(park);
                }
            }

            return parkList;
        }

        //returns list of all campgrounds for selected park
        public List<Campground> GetCampgrounds(int park_id)
        {
            List<Campground> campgroundsList = new List<Campground>();

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

					campgroundsList.Add(campground);
                }
            }

            return campgroundsList;
        }

        //returns list of sites for selected campground
        public List<Site> GetSites(int campground_id)
        {
            List<Site> campgroundSitesList = new List<Site>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT site.site_id, site.site_number, site.max_occupancy, " +
                                  "site.accessible, site.max_rv_length, site.utilities " +
                                  "FROM site JOIN campground ON site.campground_id = campground.campground_id " +
                                  "WHERE campground.campground_id = @campgroundID ORDER BY site.site_number";
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@campgroundID", campground_id);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Site site = PopulateSiteFromReader(reader);

					campgroundSitesList.Add(site);
                }
            }

            return campgroundSitesList;
        }

		public List<Site> GetSitesWithOrWithoutReservations(int campground_id)
		{
			List<Site> allSitesWihtOrWithoutReservations = new List<Site>();

			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				connection.Open();

				SqlCommand cmd = new SqlCommand();
				cmd.CommandText = "Select site.site_id, site.site_number, site.campground_id, reservation.reservation_id From site " +
								  "Join campground on site.campground_id = campground.campground_id " +
								  "Left Join reservation on site.site_id = reservation.site_id " +
								  "Where site.campground_id = @campgroundID;";
				cmd.Connection = connection;
				cmd.Parameters.AddWithValue("@campgroundID", campground_id);

				SqlDataReader reader = cmd.ExecuteReader();

				while (reader.Read())
				{
					Site site = PopulateSiteFromReader(reader);

					allSitesWihtOrWithoutReservations.Add(site);
				}
			}

			return allSitesWihtOrWithoutReservations;
		}

		//public List<Reservation> SearchReservations(int park_id, int campground_id)
		//{
		//    List<Reservation> results = new List<Reservation>();

		//    using (SqlConnection connection = new SqlConnection(_connectionString))
		//    {
		//        connection.Open();

		//        SqlCommand cmd = new SqlCommand();
		//        cmd.CommandText = "Select * From reservation " +
		//                          "Join site on reservation.site_id = site.site_id " +
		//                          "Join campground on site.campground_id = campground.campground_id " +
		//                          "Join park on campground.park_id = park.park_id " +
		//                          "Where park.park_id = @parkID And campground.campground_id = @campgroundID";
		//        cmd.Connection = connection;
		//        cmd.Parameters.AddWithValue("@campgroundID", campground_id);
		//        cmd.Parameters.AddWithValue("@parkID", park_id);

		//        SqlDataReader reader = cmd.ExecuteReader();

		//        while (reader.Read())
		//        {
		//            Reservation reservation = PopulateReservationFromReader(reader);

		//            results.Add(reservation);
		//        }
		//    }

		//    return results;
		//}

		//returns list of available sites based on dates and selected campground
		public List<Site> AvailableSitesToReserve(int campground_id, string startDate, string endDate)
        {
            List<Site> availableSitesList = new List<Site>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "Select distinct campground.name, site.site_number ,site.site_id From site " +
                                  "Join reservation on reservation.site_id = site.site_id " +
                                  "Join campground on campground.campground_id = site.campground_id " +
                                  "Where ((@startDate Not Between reservation.from_date and reservation.to_date) and " +
                                  "(@endDate Not Between reservation.from_date and reservation.to_date)) " +
                                  "And campground.campground_id = @campgroundID";
                cmd.Connection = connection;
                cmd.Parameters.AddWithValue("@campgroundID", campground_id);
                cmd.Parameters.AddWithValue("@startDate", startDate);
                cmd.Parameters.AddWithValue("@endDate", endDate);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Site site = PopulateAvailableSitesFromReader(reader);

					availableSitesList.Add(site);
                }

            }

                return availableSitesList;
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
                                      "VALUES(@siteID, @name, @startDate, @endDate); " +
                                      "SELECT CAST(scope_identity() AS int);";
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@siteID", site_id);
                    cmd.Parameters.AddWithValue("@name", reservationName);
                    cmd.Parameters.AddWithValue("@startDate", from_date);
                    cmd.Parameters.AddWithValue("@endDate", to_date);

                    record = cmd.ExecuteNonQuery();

                    Console.WriteLine("Your reservation has been added.");
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return record;
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

            item.SiteId = Convert.ToInt32(reader["site_id"]);
            item.ReservationName = Convert.ToString(reader["name"]);
            item.StartReservationDate = Convert.ToDateTime(reader["from_date"]);
            item.EndReservationDate = Convert.ToDateTime(reader["to_date"]);

            return item;
        }

        private Site PopulateAvailableSitesFromReader(SqlDataReader reader)
        {
            Site item = new Site();

            item.SiteId = Convert.ToInt32(reader["site_id"]);
            item.SiteNumber = Convert.ToInt32(reader["site_number"]);

            return item;
        }
    }
}
