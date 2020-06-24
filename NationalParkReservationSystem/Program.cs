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
            NPRS_CLI cli = new NPRS_CLI();
            cli.DisplayMenu();

            Console.ReadKey();
        }
    }
}
