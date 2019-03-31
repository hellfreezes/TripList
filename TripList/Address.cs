using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripList
{
    public class Address
    {
        public int Id { get; set; }
        public string Entity { get; set; }
        public string POIName { get; set; }
        public string POIAddress { get; set; }
        public int Distance { get; set; }
        public bool IsBase { get; set; }
        public bool IsUse { get; set; }
        public string Comment { get; set; }
    }
}
