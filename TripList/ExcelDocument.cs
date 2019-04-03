using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace TripList
{
    public class ExcelDocument
    {
        public TripListSheet ListSheet { get; set; }
        public Options CurrOptions { get; set; }

        private Excel excel;

        public void Export(string filename)
        {

            Options options = CurrOptions;
            Vehicle vehicle = options.Vehicles[CurrOptions.SelectedVehicle]; // Выбранный автомобиль
            
            if (ListSheet != null && CurrOptions != null)
            {
                excel = new Excel(MainWindow.Instance.EXE_DIRECTORY + "/Blank.xlsx", 1);

                excel.ChangeSheet(1); //перворачиваем страницу

                // ********************** мой шаблон 
                // ЛИЦЕВАЯ СТОРОНА
                /*
                excel.WriteToCell(5, 30, ListSheet.Waypoints[0].Date.Day.ToString());
                excel.WriteToCell(5, 35, ListSheet.Waypoints[0].Date.ToString("MMMM"));
                excel.WriteToCell(5, 47, ListSheet.Waypoints[0].Date.Year.ToString());

                excel.WriteToCell(30, 31, ListSheet.StartTime.ToString("HH:mm"));
                excel.WriteToCell(35, 33, ListSheet.EndTime.ToString("HH:mm"));

                excel.WriteToCell(10, 22, vehicle.VehicleModel);
                excel.WriteToCell(11, 35, vehicle.Plate);
                excel.WriteToCell(12, 13, vehicle.DriverName);
                excel.WriteToCell(26, 68, vehicle.ShortDriverName);
                excel.WriteToCell(29, 58, vehicle.Gasoline);
                excel.WriteToCell(14, 19, vehicle.License);
                excel.WriteToCell(34, 72, ListSheet.AllFuel.ToString());
                excel.WriteToCell(37, 72, ListSheet.FuelWhenStart.ToString());
                excel.WriteToCell(38, 72, ListSheet.FuelAtTheEnd.ToString());
                excel.WriteToCell(19, 73, ListSheet.OdometerStart.ToString());
                excel.WriteToCell(45, 72, ListSheet.OdometerEnd.ToString());

                

                */ // Конец моего шаблона
                // ОБОРОТНАЯ СТОРОНА

                TimeSpan fiveMin = new TimeSpan(0, 5, 0);
                DateTime preStartCtrl = ListSheet.StartTime - fiveMin;

                excel.WriteToCell(5, 28, ListSheet.Waypoints[0].Date.ToShortDateString());

                // Предрейсовый контроль
                excel.WriteToCell(20, 46, preStartCtrl.ToString("HH:mm") + " " + ListSheet.Waypoints[0].Date.ToShortDateString());

                excel.WriteToCell(30, 31, ListSheet.StartTime.ToString("HH:mm"));
                excel.WriteToCell(36, 33, ListSheet.EndTime.ToString("HH:mm"));

                excel.WriteToCell(10, 22, vehicle.VehicleModel);
                excel.WriteToCell(11, 35, vehicle.Plate);
                excel.WriteToCell(12, 13, vehicle.DriverName);
                excel.WriteToCell(26, 68, vehicle.ShortDriverName);
                excel.WriteToCell(29, 58, vehicle.Gasoline);
                excel.WriteToCell(14, 19, vehicle.License);

                if (ListSheet.Id == 0)
                    excel.WriteToCell(35, 72, ListSheet.AllFuel.ToString());


                excel.WriteToCell(38, 72, ListSheet.FuelWhenStart.ToString());
                excel.WriteToCell(39, 72, ListSheet.FuelAtTheEnd.ToString());
                excel.WriteToCell(33, 72, ListSheet.OdometerStart.ToString());
                excel.WriteToCell(46, 72, ListSheet.OdometerEnd.ToString());

                // ОБОРОТНАЯ СТОРОНА
                excel.ChangeSheet(2); //перворачиваем страницу


                int sR = 5; // Стартовые R - строка, C - колонка
                int i = 0;

                foreach (Waypoint wp in ListSheet.Waypoints)
                {
                    excel.WriteToCell(sR + i, 2, wp.Id.ToString());
                    excel.WriteToCell(sR + i, 3, wp.Date.ToShortDateString());
                    excel.WriteToCell(sR + i, 5, wp.DepartureAddress);
                    excel.WriteToCell(sR + i, 8, wp.ArriveAddress);

                    //TODO: Решить вопрос с форматом минут. Чтобы 0 минут отображалось как 00.
                    excel.WriteToCell(sR + i, 10, wp.DepartureTime.Hour.ToString());
                    excel.WriteToCell(sR + i, 12, wp.DepartureTime.Minute.ToString());
                    excel.WriteToCell(sR + i, 13, wp.ArriveTime.Hour.ToString());
                    excel.WriteToCell(sR + i, 14, wp.ArriveTime.Minute.ToString());
                    //---

                    excel.WriteToCell(sR + i, 15, wp.Distance.ToString());

                    i++;
                }

                excel.WriteToCell(35, 5, ListSheet.Distance.ToString());

                excel.Save(filename);
                excel.Close();
            }
        }
    }
}
