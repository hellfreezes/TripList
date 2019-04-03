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
    public class Options
    {
        public ObservableCollection<Vehicle> Vehicles { get; set; }

        public int Inaccuracy { get; set; } //Допустимая погрешность в киллометраже +/-
        public bool BackToBase { get; set; } //Нужно ли возвращаться на базу каждый раз после посещения очередной точки
        public int AverageSpeed { get; set; } //средняя скорость движения
        public int Pause { get; set; } //время потраченное в точке назначения
        public double FuelLost { get; set; } //Остаток топлива в конце дня которое можно не переносить на следующий день (топливо не обработается вообще)
        public string EndOfWorkDay { get; set; } //конец рабочего дня 18:00
        public string StartOfWorkDay { get; set; } //начало рабочего дня 9:00
        public int SelectedVehicle { get; set; }
        public int Liters { get; set; }
        public string Time { get; set; }

        public bool ShowContragent { get; set; }
        public bool ShowObject { get; set; }
        public bool ShowAddress { get; set; }

        public Options()
        {
            Vehicles = new ObservableCollection<Vehicle>();
        }


        public void Save(string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                XmlSerializer xser = new XmlSerializer(typeof(Options));
                xser.Serialize(fs, this);
                fs.Close();
            }
        }

        public static Options Load(string filename)
        {
            Options loaded = null;

            if (File.Exists(filename))
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    XmlSerializer xser = new XmlSerializer(typeof(Options));
                    loaded = (Options)xser.Deserialize(fs);
                    fs.Close();
                }
            }
            else
            {
                loaded = new Options();
            }

            return loaded;
        }
    }
}
