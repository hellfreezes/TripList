using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripList
{
    public class Waybill
    {
        public Vehicle CurrentVehicle { get; set; }
        public TripListSheet CurrentTripListSheet { get; set; }

        

        public void ExportXLSX(string path, string filename)
        {
            Excel excel = new Excel(path + "/Blank.xlsx", 1);
            // MessageBox.Show("Прочитано: " + excel.ReadCell(4,3));

            // Лицевая сторона
            excel.WriteToCell(10, 22, CurrentVehicle.VehicleModel);
            excel.WriteToCell(11, 35, CurrentVehicle.Plate);
            excel.WriteToCell(12, 13, CurrentVehicle.DriverName);
            excel.WriteToCell(29, 58, CurrentVehicle.Gasoline);

            // Оборотная сторона
            excel.ChangeSheet(2); //перворачиваем страницу

            int sR = 5; int sC = 2; // Стартовые R - строка, C - колонка
            int i = 0;

            foreach (Waypoint wp in CurrentTripListSheet.Waypoints)
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

            excel.Save(path + "/" + filename);

            excel.Close();
        }
    }
}
