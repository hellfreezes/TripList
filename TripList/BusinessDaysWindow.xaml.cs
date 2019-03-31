using System;
using System.Collections.Generic;
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
    }
}
