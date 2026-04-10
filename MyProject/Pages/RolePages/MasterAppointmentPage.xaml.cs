using MyProject.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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

namespace MyProject.Pages.RolePages
{
    /// <summary>
    /// Логика взаимодействия для MasterAppointmentPage.xaml
    /// </summary>
    public partial class MasterAppointmentPage : Page
    {
        private readonly Appointments _appointment;

        public MasterAppointmentPage(Appointments appointment)
        {
            InitializeComponent();
            _appointment = appointment;

            DateTimeText.Text = appointment.DateTime.ToString("dd.MM.yyyy HH:mm");
            FullnameText.Text = appointment.Users.Fullname;
            PhoneText.Text = appointment.Users.PhoneNumber;
            ServiceText.Text = appointment.Services.Name;
            StatusText.Text = appointment.Status;

            CompleteButton.IsEnabled = appointment.Status == "Booked";
        }

        private void CompleteButton_Click(object sender, RoutedEventArgs e)
        {
            _appointment.Status = "Completed";

            using (var db = Core.Context)
            {
                db.Appointments.AddOrUpdate(_appointment);
                db.SaveChanges();
            }

            StatusText.Text = "Completed";
            CompleteButton.IsEnabled = false;
        }
    }
}
