using NationalParkReservationSystem.DAL;
using NationalParkReservationSystem.Menus;
using NationalParkReservationSystem.Models;
using System;
using System.Collections.Generic;

namespace NationalParkReservationSystem
{
	public class NPRS_CLI
    {
        const string _dbConnectionString = @"Data Source=DESKTOP-D5LJCMG;Initial Catalog=dbNationalParkReservation;Integrated Security=true;";
		Menus_CLI menus = new Menus_CLI();

        public void DisplayMenu()
        {
            ParkSqlDAL parkDal = new ParkSqlDAL(_dbConnectionString);

            List<Park> parks = parkDal.GetParks();

			menus.MenuTitle("National Parks Reservation System");

            bool exit = false;

            while (!exit)
            {
				menus.MainMenu(parks);

				string command = Console.ReadLine();
                int selection;

                if (command == "q" || command == "Q")
                {
					menus.QuitMenu();
                    exit = true;
                }
                else if (int.TryParse(command, out selection))
                {
                    if (selection > 0 && selection <= parks.Count)
                    {
                        DisplayParkInfo(parks[selection - 1]);
                    }
                }
                else
                {
					menus.InvalidEntry();
                }

                Console.ReadKey();
                Console.Clear();
            }
        }

		private void DisplayParkInfo(Park park)
        {
			menus.MenuTitle(park.ParkName);

			menus.SelectedParkInfo(park);

            bool exit = false;

            while (!exit)
            {
				menus.ParkOptionMenu();

                string command = Console.ReadLine();

                if (command == "q" || command == "Q")
                {
					menus.QuitMenu();
                    exit = true;
                }
                else if (command == "v" || command == "V")
                {
                    DisplayCampgroundInfo(park);
                }
                else if (command == "m" || command == "M")
                {
                    DisplayMenu();
                }
                else
                {
					menus.InvalidEntry();
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

			menus.SelectedParkCampgroundInfo(park);

            bool exit = false;

            while (!exit)
            {
				menus.ParkCampgroundsInfo(campgrounds);

                string command = Console.ReadLine();
                int selection;

                if (command == "q" || command == "Q")
                {
					menus.QuitMenu();
                    exit = true;
                }
                else if (command == "m" || command == "M")
                {
                    DisplayMenu();
                }
				else if (int.TryParse(command, out selection))
				{
					if (selection > 0 && selection <= campgrounds.Count)
					{
						Console.Clear();
						DisplaySiteInfoForSelectedCampground(campgrounds[selection - 1]);
					}
					else
					{
						menus.InvalidEntry();
					}
				}
				else
                {
					menus.InvalidEntry();
                }

                Console.ReadKey();
                Console.Clear();
            }
        }

        private void DisplaySiteInfoForSelectedCampground(Campground campground)
        {
			menus.DisplaySitesInfo(campground);

			int selectedCampground = campground.CampgroundId;

            Park park = new Park();
            park.ParkId = campground.ParkId;

            bool exit = false;

            while (!exit)
            {
				menus.SelectedCampgroundMenu();

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
					menus.QuitMenu();
                    exit = true;
                }
                else
                {
					menus.InvalidEntry();
                }

                Console.ReadKey();
                Console.Clear();
            }
        }

        private void CreateReservation(Campground campground)
        {
			menus.MenuTitle(" Reservation Menu ");

			List<string> reservationList = menus.CampgroundReservation(campground);

            string command = Console.ReadLine();

            bool exit = false;

            while (!exit)
            {
                if (command == "s" || command == "S")
                {
                    DisplayAvailableSitesBySelectedCampground(campground, reservationList[0], reservationList[1]);
                }
                else if (command == "q" || command == "Q")
                {
					menus.QuitMenu();
                    exit = true;
                }
                else
                {
					menus.InvalidEntry();
                }

                Console.ReadKey();
                Console.Clear();
            }
        }

        private void DisplayAvailableSitesBySelectedCampground(Campground campground, string startDate, string endDate)
        {
			menus.DisplayAvailableSitesMenu(campground);

            ParkSqlDAL availableSitesDal = new ParkSqlDAL(_dbConnectionString);
            List<Site> availableSites = availableSitesDal.AvailableSitesToReserve(campground.CampgroundId, startDate, endDate);

            bool exit = false;

            if (availableSites.Count > 0)
            {
                for (int index = 0; index < availableSites.Count; index++)
                {
					string availableSiteInfo = (index + 1) + ") " +
										       " " + availableSites[index].SiteNumber.ToString().PadLeft(15) +
										       " " + availableSites[index].MaxOccupancy.ToString().PadLeft(10) +
										       " " + availableSites[index].AccessibilityStr.PadLeft(15) +
										       " " + availableSites[index].MaxRvLength.ToString().PadLeft(15) +
										       " " + availableSites[index].UtilitiesStr.PadLeft(20);

					if (index <= 9)
                    {
                        Console.WriteLine("  " + availableSiteInfo);
                    }
                    else
                    {
                        Console.WriteLine(" "  + availableSiteInfo);
                    }
                }
            }
            else
            {
				menus.InvalidEntry();
            }

            int selection;

			List<string> completeReservationInfo = menus.CompleteReservationMenu();

            while (!exit)
            {

                if (availableSites.Count == 0)
                {
                    Console.WriteLine();
                    Console.Write(" There are no sites available for " + campground.CampgroundName + ".");
                    Console.Write(" Press any key to exit the application.");
                    exit = true;
                }
                else if (int.TryParse(completeReservationInfo[0], out selection))
                {
                    if (selection > 0 && selection <= availableSites.Count)
                    {
                        int siteID = availableSites[selection - 1].SiteId;
						int siteNumber = availableSites[selection - 1].SiteNumber;
                        DisplaySubmittedReservationInfo(campground, siteID, siteNumber, startDate, endDate, completeReservationInfo[1]);
                    }
                    else
                    {
						menus.InvalidEntry();
                    }

                    exit = true;
                }

                Console.ReadKey();
                Console.Clear();
            }
        }

        private void DisplaySubmittedReservationInfo(Campground campground, int siteID, int siteNumber, string startDate, string endDate, string reservationName)
        {
            Console.Clear();

            ParkSqlDAL reservationDal = new ParkSqlDAL(_dbConnectionString);

            bool exit = false;

            while (!exit)
            {
				string command = menus.ConfirmedReservationMenu(campground, siteNumber, startDate, endDate, reservationName);

                Park park = new Park();
                park.ParkId = campground.ParkId;

                if (command == "R" || command == "r")
                {
                    int result = reservationDal.AddReservation(siteID, startDate, endDate, reservationName);

                    if (result > 0)
                    {
                        DisplayReservationConfirmation(result);
                    }
                }
                else if (command == "C" || command == "c")
                {
                    DisplayCampgroundInfo(park);
                }
                else if (command == "M" || command == "m")
                {
                    DisplayMenu();
                }              
                else if (command == "Q" || command == "q")
                {
					menus.QuitMenu();
                    exit = true;
                }
                else
                {
					menus.InvalidEntry();
                }

                Console.ReadKey();
                Console.Clear();
            }
        }

        private void DisplayReservationConfirmation(int result)
        {
            string command = menus.ConfirmationMenu();

			ParkSqlDAL reservationDal = new ParkSqlDAL(_dbConnectionString);

			bool exit = false;

			while (!exit)
			{
				if (command == "R" || command == "r")
				{
					DisplayReservationDetails(result);
				}
				else if (command == "M" || command == "m")
				{
					DisplayMenu();
				}
				else if (command == "Q" || command == "q")
				{
					menus.QuitMenu();
					exit = true;
				}
				else
				{
					menus.InvalidEntry();
				}

				Console.ReadKey();
				Console.Clear();
			}
        }

		private void DisplayReservationDetails(int reservationId)
		{
			Console.Clear();

			ParkSqlDAL reservationDal = new ParkSqlDAL(_dbConnectionString);
			List<Reservation> confirmedReservation = reservationDal.GetConfirmedReservationInfo(reservationId);

			Console.WriteLine();

			bool exit = false;

			while (!exit)
			{
				string command = menus.ConfirmedReservationDetails(confirmedReservation);

				if (command == "M" || command == "m")
				{
					DisplayMenu();
				}
				else if (command == "Q" || command == "q")
				{
					menus.QuitMenu();
				}
				else
				{
					menus.InvalidEntry();
					exit = true;
				}

				Console.ReadKey();
				Console.Clear();
			}

		}
    }
}
