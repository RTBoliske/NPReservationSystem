using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NationalParkReservationSystem.Models
{
    public class Campground
    {
        private Dictionary<int, string> _months = new Dictionary<int, string>
        {
            {1, "January" },
            {2, "February" },
            {3, "March" },
            {4, "April" },
            {5, "May" },
            {6, "June" },
            {7, "July" },
            {8, "August" },
            {9, "September" },
            {10, "October" },
            {11, "November" },
            {12, "December" },
        };

        public int CampgroundId { get; set; }
        public int ParkId { get; set; }
        public string CampgroundName { get; set; }
        public int MonthOpen { get; set; }
        public int MonthClose { get; set; }
        public decimal DailyFee { get; set; }

        public string MonthOpenStr
        {
            get
            {
                return _months[MonthOpen];
            }
        }

        public string MonthCloseStr
        {
            get
            {
                return _months[MonthClose];
            }
        }
    }
}
