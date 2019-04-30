using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NationalParkReservationSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            NationalParkReservationCLI cli = new NationalParkReservationCLI();

            cli.DisplayMainMenu();

            Console.ReadKey();
        }
    }
}
