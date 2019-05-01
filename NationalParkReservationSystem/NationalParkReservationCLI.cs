using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalParkReservationSystem.DAL;
using NationalParkReservationSystem.Models;

namespace NationalParkReservationSystem
{
	class NationalParkReservationCLI
	{
		const string _dbConnectionString = @"Data Source=TEDBOLISKE-PC;Initial Catalog=dbNationalParkReservation;Integrated Security=true;";

		public void DisplayMainMenu()
		{
			ParkSqlDAL parkDal = new ParkSqlDAL(_dbConnectionString);

			List<Park> parks = parkDal.GetParks();

			Console.Clear();
			Console.WriteLine();
			Console.WriteLine(" View Parks ");
			Console.WriteLine();

			bool exit = false;

			while (!exit)
			{
				for (int index = 0; index < parks.Count; index++)
				{
					Console.WriteLine(" " + (index + 1) + ") " + parks[index].ParkName);
				}

				Console.WriteLine(" Q - Quit");
				Console.WriteLine();
				Console.Write(" Select a park... ");

				string command = Console.ReadLine();
				int selection;

				if (command == "q" || command == "Q")
				{
					DisplayQuitApplication();
					exit = true;
				}
				else if (int.TryParse(command, out selection))
				{
					if (selection > 0 && selection <= parks.Count)
					{
						DisplayParkInfoMenu(parks[selection - 1]);
					}
				}
				else
				{
					DisplayInvalidRequest();
				}

				Console.ReadKey();
				Console.Clear();
			}
		}

		public void DisplayParkInfoMenu(Park park)
		{
			Console.Clear();

			//Selected Park information details
			Console.WriteLine();
			Console.WriteLine(" " + park.ParkName + " Information ");
			Console.WriteLine();

			Console.WriteLine("{0,0} {1,5}", " Location:", park.Location.ToString());
			Console.WriteLine("{0,0} {1,10}", " Established:", park.EstablishDate.ToString());
			Console.WriteLine("{0,0} {1,9}", " Area:", park.Area.ToString("N"));
			Console.WriteLine("{0,0} {1,9}", " Annual Visitor Count:", String.Format("{0:n0}", park.AnnualVisitorCount));
			Console.WriteLine();
			Console.WriteLine(" Description: ");
			Console.WriteLine();
			Console.WriteLine(" " + park.Description.ToString());

			//Menu to view selected park campgrounds
			Console.WriteLine();
			Console.WriteLine(" Menu Options ");
			Console.WriteLine();
			Console.WriteLine(" 1) View Campgrounds");
			Console.WriteLine(" 2) Return to Main Parks Menu");
			Console.WriteLine(" Q - Quit");
			Console.WriteLine();
			Console.Write(" Select an option... ");

			bool exit = false;

			while (!exit)
			{
				string command = Console.ReadLine();
				int selection;

				if (command == "q" || command == "Q")
				{
					DisplayQuitApplication();
					exit = true;
				}
				else if (int.TryParse(command, out selection))
				{
					if (selection == 1)
					{
						Console.Clear();
						DisplayCampgroundInfo(park);
					}
					else if (selection == 2)
					{
						DisplayMainMenu();
					}
					else
					{
						DisplayInvalidRequest();
					}
				}

				Console.ReadKey();
				Console.Clear();
			}
		}

		private void DisplayCampgroundInfo(Park park)
		{
			Console.Clear();

			ParkSqlDAL campgroundDal = new ParkSqlDAL(_dbConnectionString);
			List<Campground> campgrounds = campgroundDal.GetCampgrounds(park.ParkId);

			//display campground info for selected park
			Console.WriteLine();
			Console.WriteLine(" " + park.ParkName + " Campgrounds");
			Console.WriteLine();
			Console.WriteLine(" {0,0} {1,10} {2,30} {3,15} {4,15}", " ", "Name", "Opens", "Closes", "Daily Fee");
			Console.WriteLine();

			bool exit = false;

			while (!exit)
			{
				for (int index = 0; index < campgrounds.Count; index++)
				{
					Console.WriteLine(" " + (index + 1) + ") " + campgrounds[index].CampgroundName.PadRight(35) +
									  " " + campgrounds[index].MonthOpenStr.PadRight(15) +
									  " " + campgrounds[index].MonthCloseStr.PadRight(15) +
									  " " + campgrounds[index].DailyFee.ToString("c"));
				}

				//menu for selecting a campground to view site info
				Console.WriteLine();
				Console.WriteLine(" Menu Options ");
				Console.WriteLine();
				Console.WriteLine(" P) Return to Selected Park");
				Console.WriteLine(" M) Return to Main Parks Menu");
				Console.WriteLine(" Q - Quit");
				Console.WriteLine();
				Console.Write(" Select an option... or ");
				Console.WriteLine();
				Console.WriteLine(" Select a campground to view site information and submit reservation requests... ");

				string command = Console.ReadLine();
				int selection;

				if (command == "q" || command == "Q")
				{
					DisplayQuitApplication();
					exit = true;
				}
				else if (command == "m" || command == "M")
				{
					DisplayMainMenu();
				}
				else if (command == "p" || command == "P")
				{
					DisplayParkInfoMenu(park);
				}
				else if (int.TryParse(command, out selection))
				{
					if (selection > 0 && selection <= campgrounds.Count)
					{
						Console.Clear();
						RequestReservationMenu(campgrounds[selection - 1]);
					}
				}
				else
				{
					DisplayInvalidRequest();
				}

				Console.ReadKey();
				Console.Clear();
			}
		}

		public void RequestReservationMenu(Campground campground)
		{
			Console.Clear();

			ParkSqlDAL campSitesDal = new ParkSqlDAL(_dbConnectionString);
			List<Site> sites = campSitesDal.GetSites(campground.CampgroundId);

			//display site info
			Console.WriteLine();
			Console.WriteLine(" " + campground.CampgroundName + " Camp Site Information");
			Console.WriteLine();
			Console.WriteLine(" {0,0} {1,15} {2,15} {3,15} {4,15}", "Site #", "Max Occupancy", "Accessibility", "Max RV Length", "Utilities");
			Console.WriteLine();

			for (int index = 0; index < sites.Count; index++)
			{
				if (sites[index].SiteNumber <= 9)
				{
					Console.WriteLine("  " + sites[index].SiteNumber.ToString() + ") " +
									  " " + sites[index].MaxOccupancy.ToString().PadLeft(10) +
									  " " + sites[index].AccessibilityStr.PadLeft(15) +
									  " " + sites[index].MaxRvLength.ToString().PadLeft(15) +
									  " " + sites[index].UtilitiesStr.PadLeft(20));
				}
				else
				{
					Console.WriteLine(" " + sites[index].SiteNumber.ToString() + ") " +
									  " " + sites[index].MaxOccupancy.ToString().PadLeft(10) +
									  " " + sites[index].AccessibilityStr.PadLeft(15) +
									  " " + sites[index].MaxRvLength.ToString().PadLeft(15) +
									  " " + sites[index].UtilitiesStr.PadLeft(20));
				}
			}

			int selectedCampground = campground.CampgroundId;

			Park park = new Park();
			park.ParkId = campground.ParkId;

			bool exit = false;

			while (!exit)
			{
				Console.WriteLine();
				Console.WriteLine(" C - Return to Campgrounds Menu");
				Console.WriteLine(" N - Create new reservation");
				Console.WriteLine(" Q - Quit");
				Console.WriteLine();
				Console.Write(" Select an option... ");

				string command = Console.ReadLine();

				if (command == "n" || command == "N")
				{
					Console.WriteLine();
					CreateReservation(campground);
				}
				else if (command == "c" || command == "C")
				{
					Console.WriteLine();
					DisplayCampgroundInfo(park);
				}
				else if (command == "q" || command == "Q")
				{
					DisplayQuitApplication();
					exit = true;
				}
				else
				{
					DisplayInvalidRequest();
				}

				Console.ReadKey();
				Console.Clear();
			}
		}

		public void DisplaySitesForSelectedCampground(Campground campground)
		{
			Console.Clear();

			ParkSqlDAL campSitesDal = new ParkSqlDAL(_dbConnectionString);
			List<Site> sites = campSitesDal.GetSitesWithOrWithoutReservations(campground.CampgroundId);

			//display site info
			Console.WriteLine();
			Console.WriteLine(" " + campground.CampgroundName + " Camp Site Information");
			Console.WriteLine();
			Console.WriteLine(" {0,0} {1,15} {2,15} {3,15} {4,15}", "Site #", "Max Occupancy", "Accessibility", "Max RV Length", "Utilities");
			Console.WriteLine();

			for (int index = 0; index < sites.Count; index++)
			{
				DisplaySiteInfoHeader(sites);
			}

		}

		//public void SearchReservationsMenu(Campground campground)
		//{
		//    Console.Clear();

		//    ParkSqlDAL reservationsDal = new ParkSqlDAL(_dbConnectionString);
		//    List<Reservation> reservations = reservationsDal.SearchReservations(campground.ParkId, campground.CampgroundId);


		//    Console.WriteLine();
		//    Console.WriteLine(" " + campground.CampgroundName + " Camp Site Information");
		//    Console.WriteLine();

		//    for (int index = 0; index< reservations.Count; index++)
		//    {
		//        Console.WriteLine(" " + reservations[index].ReservationId.ToString() + ") " +
		//                          " " + reservations[index].ReservationName.PadRight(10) +
		//                          " " + reservations[index].StartReservationDate.ToString().PadRight(10) +
		//                          " " + reservations[index].EndReservationDate.ToString().PadRight(10) +
		//                          " " + reservations[index].CreateReservationDate.ToString());
		//    }
		//}

		public void CreateReservation(Campground campground)
		{
			Console.WriteLine();
			Console.WriteLine(" Reservation Menu");

			Console.WriteLine();
			Console.Write(" Enter a reservation start date (mm/dd/yyyy)... ");
			string startDate = Console.ReadLine();

			Console.WriteLine();
			Console.Write(" Enter a reservation end date (mm/dd/yyyy)... ");
			string endDate = Console.ReadLine();

			Console.WriteLine();
			Console.WriteLine(" " + campground.CampgroundName + " from " + startDate + " to " + endDate);
			Console.WriteLine();
			Console.WriteLine(" S - Submit");
			Console.WriteLine(" Q - Quit");
			Console.WriteLine();
			Console.Write(" Select an option... ");

			string command = Console.ReadLine();

			bool exit = false;

			Park park = new Park();
			park.ParkId = campground.ParkId;

			while (!exit)
			{
				if (command == "s" || command == "S")
				{
					Console.WriteLine();
					DisplayAvailableSites(campground, startDate, endDate);
				}
				else if (command == "q" || command == "Q")
				{
					DisplayQuitApplication();
					exit = true;
				}
			}

			Console.ReadKey();
			Console.Clear();
		}

		public void DisplayAvailableSites(Campground campground, string startDate, string endDate)
		{
			Console.Clear();

			ParkSqlDAL availableSiteDal = new ParkSqlDAL(_dbConnectionString);
			List<Site> availableSites = availableSiteDal.AvailableSitesToReserve(campground.CampgroundId, startDate, endDate);


			Console.WriteLine(" Available Sites for " + campground.CampgroundName);
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine(" {0,0} {1,15} {2,15} {3,15} {4,15}", "Site #", "Max Occupancy", "Accessibility", "Max RV Length", "Utilities");
			Console.WriteLine();

			bool exit = false;

			if (availableSites.Count > 0)
			{
				DisplaySiteInfoHeader(availableSites);
			}

			int selection;

			Console.Write(" Select a Site... ");
			string command = Console.ReadLine();
			Console.WriteLine();
			Console.Write(" Enter a reservation name... ");
			string reservationName = Console.ReadLine();

			while (!exit)
			{
				if (availableSites.Count == 0)
				{
					Console.WriteLine();
					Console.Write(" There are no sites available for " + campground.CampgroundName + ".");
					Console.Write(" Press any key to exit the application. ");
					exit = true;
				}
				else if (int.TryParse(command, out selection))
				{
					Console.WriteLine();
					if (selection > 0 && selection <= availableSites.Count)
					{
						int siteID = availableSites[selection - 1].SiteId;

						availableSiteDal.AddReservation(siteID, startDate, endDate, reservationName);
					}

					exit = true;
				}
			}

			Console.ReadKey();
			Console.Clear();

		}

		private static void DisplayQuitApplication()
		{
			Console.WriteLine(" Thank you for visiting the National Parks Reservation System.");
			Console.WriteLine();
			Console.Write(" Press any key to exit the application. ");
		}

		private static void DisplayInvalidRequest()
		{
			Console.WriteLine();
			Console.WriteLine(" Request not valid, please try again... ");
		}

		private static void DisplaySiteInfoHeader(List<Site> availableSites)
		{
			for (int index = 0; index < availableSites.Count; index++)
			{
				if (availableSites[index].SiteNumber <= 9)
				{
					Console.WriteLine("  " + availableSites[index].SiteNumber.ToString() + ") " +
									  " " + availableSites[index].MaxOccupancy.ToString().PadLeft(10) +
									  " " + availableSites[index].AccessibilityStr.PadLeft(15) +
									  " " + availableSites[index].MaxRvLength.ToString().PadLeft(15) +
									  " " + availableSites[index].UtilitiesStr.PadLeft(20));
				}
				else
				{
					Console.WriteLine(" " + availableSites[index].SiteNumber.ToString() + ") " +
									  " " + availableSites[index].MaxOccupancy.ToString().PadLeft(10) +
									  " " + availableSites[index].AccessibilityStr.PadLeft(15) +
									  " " + availableSites[index].MaxRvLength.ToString().PadLeft(15) +
									  " " + availableSites[index].UtilitiesStr.PadLeft(20));
				}
			}
		}
	}
}