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
    public class BusinessDaysCalculator
    {
        public ObservableCollection<BusinessDay> BusinessDays { get; set; }

        public BusinessDaysCalculator()
        {
            BusinessDays = new ObservableCollection<BusinessDay>();
        }

        public void Save(string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                XmlSerializer xser = new XmlSerializer(typeof(BusinessDaysCalculator));
                xser.Serialize(fs, this);
                fs.Close();
            }
        }

        public static BusinessDaysCalculator Load(string filename)
        {
            BusinessDaysCalculator loaded = null;

            if (File.Exists(filename))
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    XmlSerializer xser = new XmlSerializer(typeof(BusinessDaysCalculator));
                    loaded = (BusinessDaysCalculator)xser.Deserialize(fs);
                    fs.Close();
                }
            }
            else
            {
                loaded = new BusinessDaysCalculator();
            }

            return loaded;
        }

        public int IsBusinessDay(DateTime date)
        {
            foreach (BusinessDay day in BusinessDays)
            {
                if (day.Date == date)
                {
                    if (day.IsBusiness)
                    {
                        return 1; //рабочий
                    } else
                    {
                        return 2; //нерабочий
                    }
                }
            }

            return 0; // день не найден
        }
    }
}
