using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Xml.Serialization;

namespace TripList
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public AddressBook GlobalAddressBook { get; set; }
        public BusinessDaysCalculator GlobalBusinessDaysCalculator { get; set; }

        private Vehicle vehicle;
        private Excel excel;
        private TripListSheet tripList;
        private List<TripTicket> tripLists;
        
        private WaypointsWindow windowWaypoints;
        private BusinessDaysWindow windowBusinessDays;

        private string EXE_DIRECTORY;
        // Нужно ли создавать новый маршрутный лист если день закончен а топливо еще осталось
        public const bool DIVIDE_TRIPTICKETS = true;

        private int currentSheet = 0;
        private static MainWindow instance;
        public static MainWindow Instance
        {
            get { return instance; }
        }

        public MainWindow()
        {
            instance = this;

            InitializeComponent();

            GlobalAddressBook = new AddressBook();
            GlobalBusinessDaysCalculator = new BusinessDaysCalculator();

            windowWaypoints = new WaypointsWindow(this);
            windowBusinessDays = new BusinessDaysWindow(this);

            vehicle = new Vehicle();
            tripList = new TripListSheet();
            tripLists = new List<TripTicket>();

            dpReceiptDate.Text = DateTime.Now.ToShortDateString();
            dgTripList.ItemsSource = tripList.Waypoints;

            EXE_DIRECTORY = GetExeDirectory();
        }

        public int GetGasLiters()
        {
            // Добавить контроль целочисленности на поле

            return int.Parse(tbLiters.Text);
        }

        public double GetGasRateSummer()
        {
            // Добавить числа на поле

            return double.Parse(tbGasMileageSummer.Text);
        }

        public double GetGasRateWinter()
        {
            // Добавить числа на поле

            return double.Parse(tbGasMileageWinter.Text);
        }

        public DateTime GetReceiptDate()
        {
            DateTime result = DateTime.Parse(dpReceiptDate.Text); //Дата
            TimeSpan time = TimeSpan.Parse(tbTime.Text);          //Время

            return result.Add(time);
        }

        //Меню - Выход
        private void MnuExit_Click(object sender, RoutedEventArgs e)
        {
            //Application.Current.Shutdown();
            this.Close();
        }


        //Меню - Сохранить автомобиль
        private void MnuSaveVehicle_Click(object sender, RoutedEventArgs e)
        {
            UpdateVehicleInfo();

            vehicle.Save();
        }

        //Меню - Загрузить автомобиль
        private void MnuLoadVehicle_Click(object sender, RoutedEventArgs e)
        {
            vehicle = vehicle.Load(); //TODO: че то это косяк какой то.... надо как то подругому

            SyncVehicleFromData();
        }

        //Собрать все данные из элементов формы и записать их в объект Vehicle
        private void UpdateVehicleInfo()
        {
            vehicle.DriverName = tbDriverName.Text;
            vehicle.VehicleModel = tbVehicleName.Text;
            vehicle.Plate = tbPlate.Text;
            vehicle.Gasoline = tbGasoline.Text;
            vehicle.GasMileageSummer = float.Parse(tbGasMileageSummer.Text);
            vehicle.GasMileageWinter = float.Parse(tbGasMileageWinter.Text);
        }

        //Извлечь значение из Vehicle объекта в элементы формы
        private void SyncVehicleFromData()
        {
            tbDriverName.Text = vehicle.DriverName;
            tbVehicleName.Text = vehicle.VehicleModel;
            tbPlate.Text = vehicle.Plate;
            tbGasoline.Text = vehicle.Gasoline;
            tbGasMileageSummer.Text = vehicle.GasMileageSummer.ToString();
            tbGasMileageWinter.Text = vehicle.GasMileageWinter.ToString();
        }

        private void MnuTest1_Click(object sender, RoutedEventArgs e)
        {
            excel = new Excel(EXE_DIRECTORY + "/Blank.xlsx", 1);
            // MessageBox.Show("Прочитано: " + excel.ReadCell(4,3));

            excel.WriteToCell(10, 22, vehicle.VehicleModel);
            excel.Save(EXE_DIRECTORY + "/Output.xlsx");

            excel.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {   
            //Попытка освободить файл. Похоже не пашет
            //TODO: перепроверить и исправить
            if (excel != null)
                excel.Close();

            windowBusinessDays.Close();
            windowWaypoints.Close();
        }

        private void MnuTest2_Click(object sender, RoutedEventArgs e)
        {
            Waypoint wp = new Waypoint()
            {
                Id = 1,
                Date = DateTime.Now,
                DepartureTime = DateTime.Now,
                ArriveTime = DateTime.Now,
                DepartureAddress = "Абытаевская, 6",
                ArriveAddress = "78 Добр бр, 14А",
                Distance = 5
            };

            tripList.AddWaypoint(wp);

            //dgTripList.Items.Add(wp);

            wp = new Waypoint()
            {
                Id = 2,
                Date = DateTime.Now,
                DepartureTime = DateTime.Now,
                ArriveTime = DateTime.Now,
                DepartureAddress = "78 Добр бр, 14А",
                ArriveAddress = "Никитина, 8а",
                Distance = 10
            };

            tripList.AddWaypoint(wp);

            //dgTripList.Items.Add(wp);

            dgTripList.ItemsSource = tripList.Waypoints;


            
            //tripList.Save(EXE_DIRECTORY + "/tripList.xml");
        }

        //Получить путь к папке с exe файлом
        static private string GetExeDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            path = System.IO.Path.GetDirectoryName(path);
            return path;
        }

        // Изменено значение в ячейке
        private void DgTripList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            
        }

        // Кнопка добавляет пустую строчку в список вэйпоинтов маршрутного листа для последующего ее редактирования
        private void BtnTripListSheetAddEmpty_Click(object sender, RoutedEventArgs e)
        {
            tripList.AddWaypoint(new Waypoint());
        }

        private void MnuLoadTripListSheet_Click(object sender, RoutedEventArgs e)
        {
            tripList = TripListSheet.Load(EXE_DIRECTORY + "/tripList.xml");
            dgTripList.ItemsSource = tripList.Waypoints;
        }

        private void MnuSaveTripListSheet_Click(object sender, RoutedEventArgs e)
        {
            tripList.Save(EXE_DIRECTORY + "/tripList.xml");
        }

        private void MnuExportEXCEL_Click(object sender, RoutedEventArgs e)
        {
            Waybill wb = new Waybill()
            {
                CurrentVehicle = vehicle,
                CurrentTripListSheet = tripList
            };

            wb.ExportXLSX(EXE_DIRECTORY, "Output.xlsx");
        }

        private void MnuWaypoints_Click(object sender, RoutedEventArgs e)
        {
            if (windowWaypoints.ShowDialog() == true)
            {

            }
        }

        private void MnuBusinessDays_Click(object sender, RoutedEventArgs e)
        {
            if (windowBusinessDays.ShowDialog() == true)
            {

            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            windowWaypoints.Owner = this;
            windowBusinessDays.Owner = this;
        }

        private void BtnPOIGenerate_Click(object sender, RoutedEventArgs e)
        {
            TripTicket tripTicket = new TripTicket();

            tripLists = new List<TripTicket>();

            tripLists.Add(tripTicket);

            NextTripTicket ntt = tripTicket.Generate();

            while (ntt != null)
            {
                tripTicket = new TripTicket(ntt.Liters, ntt.Date);
                ntt = tripTicket.Generate();
                tripLists.Add(tripTicket);
            }

            currentSheet = 0;
            ShowTrackListSheet();

        }

        private void ShowTrackListSheet()
        {
            tripList.Clear();
            int i = 0;
            foreach (POI p in tripLists[currentSheet].GetPOIs())
            {
                i++;

                string next = "";
                if (p.next != null)
                {
                    next = p.next.address.POIAddress.ToString();
                }

                if (p.next != null)
                {
                    tripList.AddWaypoint(new Waypoint()
                    {
                        Id = i,
                        Date = p.FullDate,
                        DepartureTime = p.timeDeparture,
                        ArriveTime = p.timeArrive,
                        DepartureAddress = p.address.POIAddress.ToString(),
                        ArriveAddress = next,
                        Distance = p.distToNext
                    });
                }
            }

            tbDistance.Text = tripLists[currentSheet].TotalRealDistance.ToString();
        }
    }
}
