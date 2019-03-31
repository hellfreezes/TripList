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
        public int Id { get; set; }
        public Address address { get; set; }
        public POI next { get; set; }
        public POI prev { get; set; }
        public int distToNext { get; set; }
        public TimeSpan toNextPOI { get; set; }
        public string Date { get; set; }
        public DateTime FullDate { get; set; }
        public DateTime timeDeparture { get; set; }
        public DateTime timeArrive { get; set; }
        public double FuelResidude { get; set; }
    }
}
