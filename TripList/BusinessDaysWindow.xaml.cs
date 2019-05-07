using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TripList
{
    /// <summary>
    /// Логика взаимодействия для BusinessDaysWindow.xaml
    /// </summary>

    public partial class BusinessDaysWindow : Window
    {
        private MainWindow mainWindow;

        public BusinessDaysWindow(MainWindow mainWindow)
        {
            InitializeComponent();

            this.mainWindow = mainWindow;
            dgBusinessDays.ItemsSource = mainWindow.GlobalBusinessDaysCalculator.BusinessDays;

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }

        private void BtnBusinessDayRemove_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.GlobalBusinessDaysCalculator.BusinessDays.Remove(mainWindow.GlobalBusinessDaysCalculator.BusinessDays[dgBusinessDays.SelectedIndex]);
        }

        private void btnBusinessDayImport_Click(object sender, RoutedEventArgs e)
        {
            // прочесть файл построчно
            string[] dataBase = File.ReadAllLines("days.csv");
            // сюда будут записаны года
            List<List<string>> baseData = new List<List<string>>();
            // перебираем все строчки с годами
            int index = 0;

            foreach (string baseLine in dataBase)
            {
                if (index == 0)
                {// пропускаем первую строчку (это заголовки)
                    index++;
                    continue;
                }

                
                // разбиваем их на месяца
                string[] line = baseLine.Split('"');
                // сюда будут записаны все месяца
                List<string> lineList = new List<string>();
                // чистим все строки от лишних символов
                foreach (string l in line)
                {
                    string s = l.Trim().TrimStart(',').TrimEnd(',');
                    // если в строке что то осталось то записываем ее в месяц
                    if (s.Length > 0)
                    {
                        lineList.Add(s);
                    }
                }
                // записываем месяца в год
                baseData.Add(lineList);
            }

            // чистим список
            mainWindow.GlobalBusinessDaysCalculator.BusinessDays.Clear();

            // перебираем все месяца для вычленения дней
            foreach (List<string> line in baseData)
            {
                int year = int.Parse(line[0]); //ГОД
                for (int i = 1; i < line.Count-1; i++)
                { //Перебираем все блоки месяцев
                    String[] days = line[i].Split(','); //Получаем дни месяца
                    for (int j = 0; j < days.Length; j++)
                    {
                        int day;
                        bool isDayOff = false;
                        if (days[j].Contains("*")) //Если это день перед праздником и он рабочий
                            isDayOff = true;
                        days[j] = days[j].Replace('*', ' '); //удаляем лишние символы
                        days[j] = days[j].Replace('+', ' ');
                        days[j] = days[j].Trim();
                        day = int.Parse(days[j]); //конвертируем строку в число

                        //Добавляем элемент в базу
                        BusinessDay element = new BusinessDay();//DayOff(day, i, year, isDayOff); //Создаем выходной
                        element.Date = new DateTime(year, i, day);
                        element.IsBusiness = isDayOff;
                        //element.setAuto(1);

                        // записываем полученный день в список выходных

                        //Console.WriteLine("Новая запись: " + element);
                        mainWindow.GlobalBusinessDaysCalculator.BusinessDays.Add(element);
                        //base.add(element); //добавляем элемент в базу данных
                    }
                }
            }
        }
    }
}
