using NationalParkReservationSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalParkReservationSystem.DAL;

namespace NationalParkReservationSystem.Menus
{
	public class Menus_CLI
	{
		const string _dbConnectionString = @"Data Source=DESKTOP-D5LJCMG;Initial Catalog=dbNationalParkReservation;Integrated Security=true;";

		public void MenuTitle(string menuName)
		{
			Console.Clear();
			Console.WriteLine();
			Console.WriteLine(" " + menuName);
			Console.WriteLine();
		}

		public void MainMenu(List<Park> parks)
		{
			Console.WriteLine(" View Parks ");
			Console.WriteLine();

			for (int index = 0; index < parks.Count; index++)
			{
				Console.WriteLine(" " + (index + 1) + ") " + parks[index].ParkName);
			}

			Console.WriteLine();
			Console.WriteLine(" Q - Quit ");
			Console.Write(" Select a park or Quit application... ");

		}

		public void QuitMenu()
		{
			Console.WriteLine();
			Console.WriteLine(" Thank you for visiting the National Parks Reservation System. ");
			Console.WriteLine();
			Console.Write(" Press any key to exit the application ");
		}

		public void InvalidEntry()
		{
			Console.WriteLine();
			Console.WriteLine(" There was a problem with  your request. Please check the entry... ");
		}

		public void SelectedParkInfo(Park park)
		{
			Console.WriteLine("{0,22} {1,0}", " Location: ", park.Location.ToString());
			Console.WriteLine("{0,22} {1,0}", " Established: ", park.EstablishDate.ToString("d"));
			Console.WriteLine("{0,22} {1,0}", " Area: ", park.Area.ToString("N"));
			Console.WriteLine("{0,0} {1,0}", " Annual Vistor Count: ", String.Format("{0:n0}", park.AnnualVisitorCount));
			Console.WriteLine("{0,22} {1,0}", " Description: ", park.Description.ToString());
			Console.WriteLine();
		}

		public void ParkOptionMenu()
		{
			Console.WriteLine();
			Console.WriteLine(" Menu Options ");
			Console.WriteLine();
			Console.WriteLine(" V - View Park Campgrounds ");
			Console.WriteLine(" M - Main Parks Menu ");
			Console.WriteLine();
			Console.WriteLine(" Q - Quit");
			Console.WriteLine();
			Console.Write(" Select an option... ");
		}

		public void SelectedParkCampgroundInfo(Park park)
		{
			Console.WriteLine();
			Console.WriteLine(" " + park.ParkName + " Campgrounds ");
			Console.WriteLine();
			Console.WriteLine(" {0,0} {1,5} {2,40} {3,11} {4,15}", " ", "Name", "Opens", "Closes", "Daily Fee");
			Console.WriteLine();
		}

		public void ParkCampgroundsInfo(List<Campground> campgrounds)
		{
			for (int index = 0; index < campgrounds.Count; index++)
			{
				Console.WriteLine(" " + (index + 1) + ") " + campgrounds[index].CampgroundName.PadRight(39) +
								  " " + campgrounds[index].MonthOpenStr.PadRight(10) +
								  " " + campgrounds[index].MonthCloseStr.PadRight(12) +
								  " " + campgrounds[index].DailyFee.ToString("c"));
			}

			Console.WriteLine();
			Console.WriteLine(" Menu Options");
			Console.WriteLine();
			Console.WriteLine(" Select a campground to view site information ");
			Console.WriteLine(" M - Return to Main Menu");
			Console.WriteLine(" Q - Quit");
			Console.WriteLine();
			Console.Write(" Select an option... ");
		}

		public void DisplaySitesInfo(Campground campground)
		{
			Console.Clear();

			ParkSqlDAL campSitesDal = new ParkSqlDAL(_dbConnectionString);
			List<Site> sites = campSitesDal.GetSites(campground.CampgroundId);

			Console.WriteLine();
			Console.WriteLine(" " + campground.CampgroundName + " Camp Site Information");
			Console.WriteLine();
			Console.WriteLine(" {0,0} {1,15} {2,15} {3,15} {4,15}", "Site #", "Max Occupancy", "Accessibility", "Max RV Length", "Utilities");
			Console.WriteLine();

			for (int index = 0; index < sites.Count; index++)
			{
				string siteInfo = sites[index].SiteNumber.ToString() + ") " +
								  " " + sites[index].MaxOccupancy.ToString().PadLeft(10) +
								  " " + sites[index].AccessibilityStr.PadLeft(15) +
								  " " + sites[index].MaxRvLength.ToString().PadLeft(15) +
								  " " + sites[index].UtilitiesStr.PadLeft(20);

				if (sites[index].SiteNumber <= 9)
				{
					Console.WriteLine("  " + siteInfo);
				}
				else
				{
					Console.WriteLine(" " + siteInfo);
				}
			}
		}

		public void SelectedCampgroundMenu()
		{
			Console.WriteLine();
			Console.WriteLine(" C - Return to Campgrounds");
			Console.WriteLine(" N - Create New Reservation");
			Console.WriteLine(" Q - Quit");
			Console.WriteLine();
			Console.Write(" Select an option... ");
		}

		public List<string> CampgroundReservation(Campground campground)
		{
			List<string> reservationDates = new List<string>();

			Console.WriteLine();
			Console.Write(" Enter a reservation start date (mm/dd/yyyy)... ");
			string startDate = Console.ReadLine();

			Console.WriteLine();
			Console.Write(" Enter a reservation end date (mm/dd/yyyy)... ");
			string endDate = Console.ReadLine();

			Console.WriteLine();
			Console.WriteLine(" " + campground.CampgroundName + " from " + startDate + " to " + endDate);
			Console.WriteLine();
			Console.WriteLine(" S - Submit ");
			Console.WriteLine(" Q - Quit ");
			Console.WriteLine();
			Console.Write(" Select an option... ");

			reservationDates.Add(startDate);
			reservationDates.Add(endDate);

			return reservationDates;
		}

		public void DisplayAvailableSitesMenu(Campground campground)
		{
			Console.Clear();

			Console.WriteLine(" Available Sites for " + campground.CampgroundName);
			Console.WriteLine();
			Console.WriteLine(" {0,0} {1,15} {2,15} {3,15} {4,15} {5,15} ", "Site ID", "Site Number", "Max Occupancy", "Accessibility", "Max RV Length", "Utilities");
			Console.WriteLine();
		}

		public List<string> CompleteReservationMenu()
		{
			List<string> completeReservationInfo = new List<string>();

			Console.WriteLine();
			Console.Write(" Enter a Site ID... ");
			string siteId = Console.ReadLine();

			Console.WriteLine();
			Console.Write(" Enter a name for your reservation... ");
			string reservationName = Console.ReadLine();

			completeReservationInfo.Add(siteId);
			completeReservationInfo.Add(reservationName);

			return completeReservationInfo;
		}

		public string ConfirmedReservationMenu(Campground campground, int siteNumber, string startDate, string endDate, string reservationName)
		{
			Console.WriteLine();
			Console.WriteLine(" Please confirm the following reservation information... ");
			Console.WriteLine();
			Console.WriteLine(" Campground Site: " + siteNumber + " at " + campground.CampgroundName + " Campground.");
			Console.WriteLine(" Start Date: " + startDate);
			Console.WriteLine(" End Date: " + endDate);
			Console.WriteLine(" Name: " + reservationName);

			Console.WriteLine();
			Console.WriteLine(" Confirm Reservation ");
			Console.WriteLine();
			Console.WriteLine(" R - Reservation Confirmation ");
			Console.WriteLine(" C - Campground Info ");
			Console.WriteLine(" M - Main Parks Menu ");
			Console.WriteLine(" Q - Quit ");
			Console.WriteLine();
			Console.Write(" Select an option... ");

			string command = Console.ReadLine();

			return command;
		}

		public string ConfirmationMenu()
		{
			Console.Clear();
			Console.WriteLine();
			Console.WriteLine(" Your reservation request has been successfully submitted. ");
			Console.WriteLine();
			Console.WriteLine(" R - Reservation Details ");
			Console.WriteLine(" M - Main Parks Menu ");
			Console.WriteLine(" Q - Quit ");
			Console.WriteLine();
			Console.Write(" Select an option... ");

			string command = Console.ReadLine();

			return command;
		}

		public string ConfirmedReservationDetails(List<Reservation> confirmedReservation)
		{
			Console.Clear();
			Console.WriteLine();
			Console.WriteLine(" Reservation Name: " + confirmedReservation[0].ReservationName);
			Console.WriteLine(" Start Date: " + confirmedReservation[0].StartReservationDate.ToString());
			Console.WriteLine(" End Date: " + confirmedReservation[0].EndReservationDate.ToString());
			Console.WriteLine(" Reservation ID: " + confirmedReservation[0].ReservationId.ToString());

			Console.WriteLine();
			Console.WriteLine(" M - Main Parks Menu ");
			Console.WriteLine(" Q - Quit ");
			Console.WriteLine();
			Console.Write(" Select an option... ");

			string command = Console.ReadLine();

			return command;
		}
	}
}
