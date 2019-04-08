using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripList
{
    public class NextTripTicket
    {
        public int NextIndex { get; set; }
        public double Liters { get; set; }
        public int OdometerEndDay { get; set; }
        public DateTime Date { get; set; }
        public int NumberPOI { get; set; }
    }
}
