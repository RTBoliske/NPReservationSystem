using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NationalParkReservationSystem.Models
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public int SiteId { get; set; }
        public string ReservationName { get; set; }
        public DateTime StartReservationDate { get; set; }
        public DateTime EndReservationDate { get; set; }
        public DateTime CreateReservationDate { get; set; }
    }
}
