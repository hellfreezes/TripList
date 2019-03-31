using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripList
{
    public class Waypoint
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArriveTime { get; set; }
        public string DepartureAddress { get; set; }
        public string ArriveAddress { get; set; }
        public int Distance { get; set; }
    }
}
