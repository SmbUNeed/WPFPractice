using MyProject.Backend;
using MyProject.Data;
using MyProject.Pages.RolePages;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyProject.Pages
{
    /// <summary>
    /// Логика взаимодействия для MasterPage.xaml
    /// </summary>
    public partial class MasterPage : Page
    {
        public MasterPage()
        {
            InitializeComponent();
            UpdateLists();
        }

        private void UpdateLists()
        {
            MeAppointments.ItemsSource = Core.Context.Appointments
                .Where(a => a.MasterId == Auth.CurrentUser.Id && a.DateTime >= DateTime.Now && (a.Status.Equals("Booked") || a.Status.Equals("Completed")))
                .OrderByDescending(a => a.DateTime)
                .ToList();

            var services = Auth.CurrentUser.Services.ToList();
            MyServices.ItemsSource = services;

            var serviceIds = services.Select(s => s.Id).ToList();
            ServicesComboBox.ItemsSource = Core.Context.Services
                .Where(s => !serviceIds.Contains(s.Id))
                .ToList();
        }

        private void RemoveServiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (!((sender as Button)?.DataContext is Services service)) return;
            Auth.CurrentUser.Services.Remove(service);
            Core.Context.SaveChanges();
            UpdateLists();
        }

        private void ServicesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count < 1) return;
            Services service = e.AddedItems[0] as Services;
            Auth.CurrentUser.Services.Add(service);
            Core.Context.SaveChanges();
            UpdateLists();
        }

        private void MeAppointments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NavigationService.Navigate(new MasterAppointmentPage(e.AddedItems[0] as Appointments));
        }
    }
}
