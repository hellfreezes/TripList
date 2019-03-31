using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripList
{
    //Point of interest
    public class POI
    {
        public Address address { get; set; }
        public POI next { get; set; }
        public POI prev { get; set; }
        public int distToNext { get; set; }
        public TimeSpan toNextPOI { get; set; }
        public string Date { get; set; }
        public DateTime timeDeparture { get; set; }
        public DateTime timeArrive { get; set; }
    }
}
