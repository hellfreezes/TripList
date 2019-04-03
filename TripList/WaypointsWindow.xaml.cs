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
    /// Логика взаимодействия для WaypointsWindow.xaml
    /// </summary>
    public partial class WaypointsWindow : Window
    {
        //Ссылка на главное окно
        private MainWindow mainWindow;

        public WaypointsWindow(MainWindow mainWindow)
        {
            InitializeComponent();

            this.mainWindow = mainWindow;
            dgAddresses.ItemsSource = mainWindow.GlobalAddressBook.addresses;
        }

        private void BtnImportSQL_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.GlobalAddressBook = AddressBook.LoadFromSQL();
            dgAddresses.ItemsSource = mainWindow.GlobalAddressBook.addresses;
        }

        private void WinWaypoints_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }

        private void BtnWaypointRemove_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.GlobalAddressBook.addresses.Remove(mainWindow.GlobalAddressBook.addresses[dgAddresses.SelectedIndex]);
        }
    }
}
