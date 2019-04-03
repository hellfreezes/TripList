using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
        private static readonly Regex _regex = new Regex("[^0-9-]+");

        public AddressBook GlobalAddressBook { get; set; }
        public BusinessDaysCalculator GlobalBusinessDaysCalculator { get; set; }
        public List<Vehicle> Vehicles { get; set; }
        public Options CurrentOptions { get; set; }

        private Excel excel;
        private TripListSheet tripList;
        private List<TripTicket> tripLists;
        
        private WaypointsWindow windowWaypoints;
        private BusinessDaysWindow windowBusinessDays;
        private OptionsWindow windowOptions;

        public string EXE_DIRECTORY;
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

            LoadAll();

            //GlobalAddressBook = new AddressBook();//AddressBook.LoadFromSQL();
            //GlobalBusinessDaysCalculator = new BusinessDaysCalculator();

            cbVehicle.ItemsSource = CurrentOptions.Vehicles;

            windowOptions = new OptionsWindow();
            windowWaypoints = new WaypointsWindow(this);
            windowBusinessDays = new BusinessDaysWindow(this);

            if (CurrentOptions.Vehicles.Count > 0)
            {
                SyncVehicleFromData();
                cbVehicle.SelectedIndex = CurrentOptions.SelectedVehicle;
            }

            tripList = new TripListSheet();
            tripLists = new List<TripTicket>();

            dpReceiptDate.Text = DateTime.Now.ToShortDateString();
            dgTripList.ItemsSource = tripList.Waypoints;

            EXE_DIRECTORY = GetExeDirectory();
        }

        public double GetGasLiters()
        {
            // Добавить контроль целочисленности на поле

            return double.Parse(tbLiters.Text);
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

            //vehicle.Save();
        }

        //Меню - Загрузить автомобиль
        private void MnuLoadVehicle_Click(object sender, RoutedEventArgs e)
        {
            //vehicle = Vehicle.Load("Vehicle.xml"); //TODO: че то это косяк какой то.... надо как то подругому

            SyncVehicleFromData();
        }

        //Собрать все данные из элементов формы и записать их в объект Vehicle
        private void UpdateVehicleInfo()
        {
            /*vehicle.DriverName = tbDriverName.Text;
            vehicle.VehicleModel = cbVehicle.SelectedValue.ToString();
            vehicle.Plate = tbPlate.Text;
            vehicle.Gasoline = tbGasoline.Text;
            vehicle.GasMileageSummer = float.Parse(tbGasMileageSummer.Text);
            vehicle.GasMileageWinter = float.Parse(tbGasMileageWinter.Text);*/
        }

        //Извлечь значение из Vehicle объекта в элементы формы
        public void SyncVehicleFromData()
        {
            tbDriverName.Text = CurrentOptions.Vehicles[CurrentOptions.SelectedVehicle].DriverName;
            //tbVehicleName.Text = CurrentOptions.Vehicles[CurrentOptions.SelectedVehicle].VehicleModel;
            tbPlate.Text = CurrentOptions.Vehicles[CurrentOptions.SelectedVehicle].Plate;
            tbGasoline.Text = CurrentOptions.Vehicles[CurrentOptions.SelectedVehicle].Gasoline;
            tbGasMileageSummer.Text = CurrentOptions.Vehicles[CurrentOptions.SelectedVehicle].GasMileageSummer.ToString();
            tbGasMileageWinter.Text = CurrentOptions.Vehicles[CurrentOptions.SelectedVehicle].GasMileageWinter.ToString();
            tbLicense.Text = CurrentOptions.Vehicles[CurrentOptions.SelectedVehicle].License;
            tbOdometerStart.Text = CurrentOptions.Vehicles[CurrentOptions.SelectedVehicle].Odometer.ToString();
            tbTime.Text = CurrentOptions.Time;
            tbLiters.Text = CurrentOptions.Liters.ToString();
        }

        private void MnuTest1_Click(object sender, RoutedEventArgs e)
        {
            excel = new Excel(EXE_DIRECTORY + "/Blank.xlsx", 1);
            // MessageBox.Show("Прочитано: " + excel.ReadCell(4,3));

            excel.WriteToCell(10, 22, cbVehicle.SelectedValue.ToString());
            excel.Save(EXE_DIRECTORY + "/Output.xlsx");

            excel.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveAll();

            //Попытка освободить файл. Похоже не пашет
            //TODO: перепроверить и исправить
            if (excel != null)
                excel.Close();

            windowBusinessDays.Close();
            windowWaypoints.Close();
            windowOptions.Close();
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
                CurrentVehicle = Vehicles[CurrentOptions.SelectedVehicle],
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
            windowOptions.Owner = this;
        }

        // Пользователь запросил генерацию листов по введенным данным
        // TODO: обработать правильность заполнения и контроль ошибок
        private void BtnPOIGenerate_Click(object sender, RoutedEventArgs e)
        {

            // Создаем новый лист
            TripTicket tripTicket = new TripTicket();
            // Создаем хранилище листов
            tripLists = new List<TripTicket>();
            
            // Помещаем новый лист в хранилище
            tripLists.Add(tripTicket);
            // Генерируем с сохранением выходных данных в ntt
            NextTripTicket ntt = tripTicket.Generate();
            // Записываем количество пройденных км в рамках этого листа
            int realDistance = tripTicket.TotalRealDistance;

            // ИНТЕРФЕЙС:
            // Отображаем расчетное количество км, которое можно пройти на введенном количестве топлива
            tbMustDistance.Text = tripTicket.TotalDistance.ToString();

            // Будем продолжать генерацию новых листов пока она требуется (ntt не равно null)
            while (ntt != null)
            {
                tripTicket = new TripTicket(ntt.Liters, ntt.Date);
                ntt = tripTicket.Generate();
                // Суммируем пройденный путь
                realDistance += tripTicket.TotalRealDistance;
                tripLists.Add(tripTicket);
            }

            // Показывать первый лист из хранилища
            currentSheet = 0;
            // Вызвать обновление интерфейса
            ShowTrackListSheet();

            // ИНТЕРФЕЙС:
            // Отобразисть суммарное количество пройденного пути
            tbFullDistance.Text = realDistance.ToString();
            tbOdometerEnd.Text = (int.Parse(tbOdometerStart.Text) + realDistance).ToString();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            currentSheet++;
            if (currentSheet + 1 > tripLists.Count)
                currentSheet = 0;

            ShowTrackListSheet();
        }

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            currentSheet--;
            if (currentSheet < 0)
                currentSheet = tripLists.Count - 1;

            ShowTrackListSheet();
        }

        // Обновление интерфейса таблицы маршрутного листа
        private void ShowTrackListSheet()
        {
            Random rand = new Random();

            

            tripList.Clear();

            tripList.Id = currentSheet;
            int i = 0;
            foreach (POI p in tripLists[currentSheet].GetPOIs())
            {
                i++;

                // На основании цепочки точек генерируем юзерфрендли вэйпоинты для
                // Отображения их в интерфейсе

                string next = "";
                string depa = "";
                // Тут идет проверка на то, в каком виде отображать название точки
                if (p.next != null)
                {
                    if (CurrentOptions.ShowContragent) //юр лицо
                        next += p.next.address.POIName;
                    if (CurrentOptions.ShowObject) //название объекта
                        next += " "+p.next.address.Entity;
                    if (CurrentOptions.ShowAddress) //адрес
                        next += " "+p.next.address.POIAddress;
                }
                if (depa != null)
                {
                    if (CurrentOptions.ShowContragent) //юр лицо
                        depa += p.address.POIName;
                    if (CurrentOptions.ShowObject) //название объекта
                        depa += " "+p.address.Entity;
                    if (CurrentOptions.ShowAddress) //адрес
                        depa += " "+p.address.POIAddress;
                }

                if (p.next != null)
                {
                    tripList.AddWaypoint(new Waypoint()
                    {
                        Id = i,
                        Date = p.FullDate,
                        DepartureTime = p.timeDeparture,
                        ArriveTime = p.timeArrive,
                        DepartureAddress = depa,
                        ArriveAddress = next,
                        Distance = p.distToNext
                    });
                }
            }

            // Время выезда/приезда
            TimeSpan randTime = new TimeSpan(0, rand.Next(2, 15), 0);
            tripList.StartTime = tripLists[currentSheet].GetPOIs()[0].timeDeparture - randTime;

            randTime = new TimeSpan(0, rand.Next(2, 15), 0);
            int k = tripLists[currentSheet].GetPOIs().Count - 1;

            if (tripLists[currentSheet].GetPOIs()[k].timeArrive == new DateTime(1,1,1,0,0,0))
                k--;

            tripList.EndTime = tripLists[currentSheet].GetPOIs()[k].timeArrive + randTime;
            //Console.WriteLine(tripList.EndTime.ToShortTimeString());

            // Показание одометра в конце дня по листу
            int dist = 0;
            for (int j = 0; j < currentSheet; j++)
            {
                dist += tripLists[j].TotalRealDistance;
            }

            //TODO: Косяк по начальному одометру ИСКАТЬ ТУТ!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            int homeWay = (CurrentOptions.Vehicles[CurrentOptions.SelectedVehicle].HomeWay * currentSheet);
            int odo = int.Parse(tbOdometerStart.Text) + tripLists[currentSheet].TotalRealDistance + dist + homeWay;

            tbOdometerEndDay.Text = odo.ToString();

            tripList.Distance = tripLists[currentSheet].TotalRealDistance;
            
            tripList.OdometerStart = odo - tripLists[currentSheet].TotalRealDistance; // Показание на начало дня (значение за вычетом пройденного за сегодня пути)
            tripList.OdometerEnd = odo + dist; // Показания на конец дня

            tripList.AllFuel = double.Parse(tbLiters.Text);

            // Топливо на начало дня
            if (currentSheet > 0) //это уже не первый день
            {
                tripList.FuelWhenStart = tripLists[currentSheet - 1].FuelResidude;
            } else
            {
                tripList.FuelWhenStart = tripList.AllFuel;
            }

            // Пройденный путь в рамках одного листа
            tbDistance.Text = tripLists[currentSheet].TotalRealDistance.ToString();
            // Остаток топлива на конец дня в этом листе
            tbFuelResidude.Text = tripLists[currentSheet].FuelResidude.ToString();
            tripList.FuelAtTheEnd = tripLists[currentSheet].FuelResidude;
            // Обновить интерфейс выбранного к показу листа
            UpdateSheetsCount();
        }

        private void UpdateSheetsCount()
        {
            lblSheets.Content = currentSheet + 1 + "/" + tripLists.Count;
        }

        private void MnuOptions_Click(object sender, RoutedEventArgs e)
        {
            if (windowOptions.ShowDialog() == true)
            {
                Console.WriteLine("Скрыто");
            }
        }

        private void CbVehicle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbVehicle.SelectedIndex == -1)
                cbVehicle.SelectedIndex = 0;
            else 
                CurrentOptions.SelectedVehicle = cbVehicle.SelectedIndex;

            SyncVehicleFromData();
        }

        private void SaveAll()
        {
            CurrentOptions.Save("config.xml");
            GlobalBusinessDaysCalculator.Save("business.xml");
            GlobalAddressBook.SaveXML("addresses.xml");
        }

        private void LoadAll()
        {
            CurrentOptions = Options.Load("config.xml");
            GlobalBusinessDaysCalculator = BusinessDaysCalculator.Load("business.xml");
            GlobalAddressBook = AddressBook.LoadXML("addresses.xml");
        }

        private void TbOdometerStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbOdometerStart.Text == string.Empty)
                tbOdometerStart.Text = "0";
            if (CurrentOptions != null)
                CurrentOptions.Vehicles[CurrentOptions.SelectedVehicle].Odometer = int.Parse(tbOdometerStart.Text);
        }

        private void TbTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(CurrentOptions != null)
                CurrentOptions.Time = tbTime.Text;
        }

        private void TbLiters_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbLiters.Text == string.Empty)
                tbLiters.Text = "0";
            if (CurrentOptions != null)
                CurrentOptions.Liters = double.Parse(tbLiters.Text);
        }

        private void TbLiters_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }

        private void TbOdometerStart_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }

        // Вызов окна о программе
        private void MnuAbout_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnExportXLSX_Click(object sender, RoutedEventArgs e)
        {
            // Если есть сгенерированный триплист (тот что на экране)
            if (tripList.Waypoints.Count > 0)
            {
                // Configure save file dialog box
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = CurrentOptions.Vehicles[CurrentOptions.SelectedVehicle].VehicleModel + "_" + tripList.Waypoints[0].Date.ToShortDateString(); // Default file name
                dlg.DefaultExt = ".xlsx"; // Default file extension
                dlg.Filter = "Документ Microsoft Excel (.xlsx)|*.xlsx"; // Filter files by extension

                // Show save file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process save file dialog box results
                if (result == true)
                {
                    // Save document
                    string filename = dlg.FileName;


                    ExcelDocument newDoc = new ExcelDocument()
                    {
                        CurrOptions = CurrentOptions,
                        ListSheet = tripList
                    };

                    newDoc.Export(filename);
                }

            } else
            {
                MessageBox.Show("Прежде чем экспортировать, нажмите кнопку Сгенерировать!");
            }
        }
    }
}
