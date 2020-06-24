using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NationalParkReservationSystem.Models
{
    public class Site
    {
        private string _isAccessible = "No";
        private string _hasUtilities = "No";

        public int SiteId { get; set; }
        public int CampgroundId { get; set; }
        public int SiteNumber { get; set; }
        public int MaxOccupancy { get; set; }
        public bool Accessibility { get; set; }
        public int MaxRvLength { get; set; }
        public bool Utilities { get; set; }

        public string AccessibilityStr
        {
            get
            {
                if (Accessibility == true)
                {
                    _isAccessible = "Yes";
                }

                return _isAccessible;
            }
        }

        public string UtilitiesStr
        {
            get
            {
                if (Utilities == true)
                {
                    _hasUtilities = "Yes";
                }

                return _hasUtilities;
            }
        }
    }
}
