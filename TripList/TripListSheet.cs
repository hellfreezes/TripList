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
        public ObservableCollection<Waypoint> Waypoints;

        public TripListSheet()
        {
            Waypoints = new ObservableCollection<Waypoint>();
        }

        public void AddWaypoint(Waypoint wp)
        {
            Waypoints.Add(wp);
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
