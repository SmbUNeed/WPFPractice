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
        }

        private void LoadAppointments()
        {
            var query = Core.Context.MastersTime
                .Where(a => a.Status == "Free")
                ;

            if (FilterMaster != null)
                query = query.Where(a => a.MasterId == FilterMaster.Id);

            if (FilterService != null)
                query = query.Where(a => a.Users.Services.Any(s => s == FilterService)).Include(Services => FilterService);

            if (DateFilter.SelectedDate.HasValue)
                query = query.Where(a => a.DateTime.Date == DateFilter.SelectedDate.Value.Date);

            AppointmentsList.ItemsSource = query.ToList();
        }
        private void DateFilter_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadAppointments();
        }

        private void BookButton_Click(object sender, RoutedEventArgs e)
        {
            var appointment = (Appointments)((Button)sender).Tag;
            appointment.UserId = Auth.CurrentUser.Id;
            Core.Context.SaveChanges();
            LoadAppointments();
        }
    }
}
