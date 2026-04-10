using MyProject.Data;
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
using MyProject.Backend;
using System.Data.Entity;

namespace MyProject.Pages
{
    public partial class AppointmentsPage : Page
    {
        Users FilterMaster;
        Services FilterService;
        public AppointmentsPage(object filter)
        {
            InitializeComponent();
            Initialize(filter);
            LoadAppointments();
        }

        private void Initialize(object filter)
        {
            if (filter is Users)
            {
                FilterMaster = (Users)filter;
            }
            else if (filter is Services)
            {
                FilterService = (Services)filter;
            }
            DateFilter.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-1)));
            DateFilter.BlackoutDates.Add(new CalendarDateRange(Core.Context.Appointments.Max(mt => mt.DateTime).AddDays(1), DateTime.MaxValue));
        }

        private void LoadAppointments()
        {
            var query = Core.Context.Appointments
                .Where(a => a.Status == "Free" && a.DateTime >= DateTime.Now);

            if (FilterMaster != null)
            {
                query = query.Where(q => q.MasterId == FilterMaster.Id);
            }

            if (FilterService != null)
            {
                query = query.Where(q => q.Services.Id == FilterService.Id);
            }

            if (DateFilter.SelectedDate.HasValue)
            {
                DateTime nextDay = DateFilter.SelectedDate.Value.AddDays(1);
                query = query.Where(q => q.DateTime >= DateFilter.SelectedDate.Value && q.DateTime < nextDay);
            }

            AppointmentsList.ItemsSource = query.ToList();
        }
        private void DateFilter_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadAppointments();
        }

        private void BookButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Auth.IsAuthenticated)
            {
                MessageBox.Show("Войдити, прежде чем записаться на услугу");
                NavigationService.Navigate(new LoginPage());
                return;
            }
            var appointment = (Appointments)((Button)sender).Tag;
            NavigationService.Navigate(new AppointmentInfoPage(appointment));
            LoadAppointments();
        }
    }
}
