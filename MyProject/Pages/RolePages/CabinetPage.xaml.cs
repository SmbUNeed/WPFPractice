using MyProject.Backend;
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
    /// Логика взаимодействия для CabinetPage.xaml
    /// </summary>
    public partial class CabinetPage : Page
    {
        public CabinetPage()
        {
            if (!Auth.IsAuthenticated) { NavigationService.Navigate(new MainPage()); return; }
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            UsernameBox.Text = Auth.CurrentUser.Fullname;
            MyAppointmentsListBox.ItemsSource = Data.Core.Context.Appointments.Where(a => a.UserId == Auth.CurrentUser.Id).ToList();
            MyOrdersListBox.ItemsSource = Data.Core.Context.Orders.Where(a => a.UserId == Auth.CurrentUser.Id).ToList();
        }
    }
}
