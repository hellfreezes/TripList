using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TripList
{
    public class TripListSheet
    {
        public int Id { get; set; }
        public ObservableCollection<Waypoint> Waypoints;
        public double AllFuel { get; set; }
        public double FuelWhenStart { get; set; }
        public double FuelAtTheEnd { get; set; }
        public int OdometerStart { get; set; }
        public int OdometerEnd { get; set; }
        public int Distance { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }


        public TripListSheet()
        {
            Waypoints = new ObservableCollection<Waypoint>();
            StartTime = new DateTime(1, 1, 1, 0, 0, 0);
            EndTime = new DateTime(1, 1, 1, 0, 0, 0);
        }

        public void AddWaypoint(Waypoint wp)
        {
            Waypoints.Add(wp);
        }

        public void Clear()
        {
            Waypoints.Clear();
        }

        public void Save(string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                XmlSerializer xser = new XmlSerializer(typeof(TripListSheet));
                xser.Serialize(fs, this);
                fs.Close();
            }
        }

        public static TripListSheet Load(string filename)
        {
            TripListSheet loaded = null;

            if (File.Exists(filename))
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    XmlSerializer xser = new XmlSerializer(typeof(TripListSheet));
                    loaded = (TripListSheet)xser.Deserialize(fs);
                    fs.Close();
                }
            }
            else
            {
                loaded = new TripListSheet();
            }

            return loaded;
        }
    }
}
