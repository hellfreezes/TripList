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
    /// Логика взаимодействия для OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public OptionsWindow()
        {
            InitializeComponent();
            UpdateUI();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            UpdateOptions();
            MainWindow.Instance.CurrentOptions.Save("config.xml");
            MainWindow.Instance.SyncVehicleFromData();
            this.Visibility = Visibility.Hidden;
        }

        private void UpdateUI()
        {
            Options o = MainWindow.Instance.CurrentOptions;
            tbInaccuracy.Text = o.Inaccuracy.ToString();
            cbBackToBase.SelectedValue = o.BackToBase.ToString();
            tbAverageSpeed.Text = o.AverageSpeed.ToString();
            tbPause.Text = o.Pause.ToString();
            tbFuelLost.Text = o.FuelLost.ToString();
            tbStartOfWorkDay.Text = o.StartOfWorkDay;
            tbEndOfWorkDay.Text = o.EndOfWorkDay;
            chkContrgent.IsChecked = o.ShowContragent;
            chkObject.IsChecked = o.ShowObject;
            chkAddress.IsChecked = o.ShowAddress;

            dgVehicles.ItemsSource = o.Vehicles;
        }

        private void UpdateOptions()
        {
            Options o = MainWindow.Instance.CurrentOptions;
            o.Inaccuracy = int.Parse(tbInaccuracy.Text);
            o.BackToBase = true; //<------------------------------------- выбор в интерфейсе не учитывается!!!!!!!!!!!!!!!
            o.AverageSpeed = int.Parse(tbAverageSpeed.Text);
            o.Pause = int.Parse(tbPause.Text);
            o.FuelLost = double.Parse(tbFuelLost.Text);
            o.StartOfWorkDay = tbStartOfWorkDay.Text;
            o.EndOfWorkDay = tbEndOfWorkDay.Text;
            o.ShowObject = Convert.ToBoolean(chkObject.IsChecked);
            o.ShowContragent = Convert.ToBoolean(chkContrgent.IsChecked);
            o.ShowAddress = Convert.ToBoolean(chkAddress.IsChecked);
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "Удалить выбранный автомобиль ("+ dgVehicles.SelectedIndex + ")?";
            string caption = "Список автомобилей";
            MessageBoxButton button = MessageBoxButton.YesNoCancel;
            MessageBoxImage icon = MessageBoxImage.Warning;

            // Display message box
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            // Process message box results
            switch (result)
            {
                case MessageBoxResult.Yes:
                    // User pressed Yes button
                    if (MainWindow.Instance.CurrentOptions.SelectedVehicle == dgVehicles.SelectedIndex)
                    {
                        MainWindow.Instance.CurrentOptions.SelectedVehicle = 0;
                    }
                    MainWindow.Instance.CurrentOptions.Vehicles.Remove(MainWindow.Instance.CurrentOptions.Vehicles[dgVehicles.SelectedIndex]);
                    break;
                case MessageBoxResult.No:
                    // User pressed No button
                    // ...
                    break;
                case MessageBoxResult.Cancel:
                    // User pressed Cancel button
                    // ...
                    break;
            }
        }
    }
}
