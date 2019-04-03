using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Xml.Serialization;

namespace TripList
{
    public class Vehicle
    {
        const string FILENAME = "Vehicle.xml";

        public string DriverName { get; set; }
        public string ShortDriverName { get; set; }
        public string VehicleModel { get; set; }
        public string License { get; set; }
        public string Plate { get; set; }
        public string Gasoline { get; set; }
        public float GasMileageSummer { get; set; }
        public float GasMileageWinter { get; set; }
        public int Odometer { get; set; }
        public int HomeWay { get; set; }

        public void Save()
        {
            if (File.Exists(FILENAME))
            {
                File.Delete(FILENAME);
            }

            using (FileStream fs = new FileStream(FILENAME, FileMode.Create))
            {
                XmlSerializer xser = new XmlSerializer(typeof(Vehicle));
                xser.Serialize(fs, this);
                fs.Close();
            }
        }

        public static Vehicle Load(string filename)
        {
            Vehicle loaded = null;

            if (File.Exists(FILENAME))
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    XmlSerializer xser = new XmlSerializer(typeof(Vehicle));
                    loaded = (Vehicle)xser.Deserialize(fs);
                    fs.Close();
                }
            } else
            {
                loaded = new Vehicle();
            }

            return loaded;
        }
    }
}
