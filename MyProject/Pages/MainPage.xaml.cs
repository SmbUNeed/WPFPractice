using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using MyProject.Data;

namespace MyProject.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public Dictionary<Users, List<Services>> ListMasters = new Dictionary<Users, List<Services>>();
        public List<Services> Services = new List<Services>();
        public MainPage()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            ListMasters = Core.Context.Users.Where(u => u.Roles.Name == "Master").ToDictionary(u => u, u => u.Services.ToList());
            Services = Core.Context.Services.ToList();
            AccountButton.Click += (s, e) => NavigationService.Navigate(Access.HomePage());

            if (Auth.IsAuthenticated)
                AccountButton.Content = "Личный кабинет";
            else 
                AccountButton.Content = "Войти";

            ListBoxMasters.ItemsSource = ListMasters;
            ListBoxServices.ItemsSource = Services;
            FilterComboBox.SelectedIndex = 0;
            SwitchRoleCombobox.ItemsSource = Core.Context.Roles.ToList();
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems[0] as ComboBoxItem).Content.ToString() == "По мастеру")
            {
                ListBoxMasters.Visibility = Visibility.Visible;
                ListBoxServices.Visibility = Visibility.Hidden;
            }
            else
            {
                ListBoxMasters.Visibility = Visibility.Hidden;
                ListBoxServices.Visibility = Visibility.Visible;
            }
        }

        private void ProductsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProductsPage());
        }

        private void ListBoxMasters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Users master = ((KeyValuePair <Users, List <Services>>)e.AddedItems[0]).Key;
            NavigationService.Navigate(new AppointmentsPage(master));
        }

        private void ListBoxServices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Services service = (Services)e.AddedItems[0];
            NavigationService.Navigate(new AppointmentsPage(service));
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();
            if (string.IsNullOrEmpty(searchText))
            {
                ListBoxMasters.ItemsSource = ListMasters;
                ListBoxServices.ItemsSource = Services;
            }
            else
            {
                if (ListBoxMasters.Visibility == Visibility.Visible) 
                    ListBoxMasters.ItemsSource = ListMasters.Where(p => p.Key.Fullname.ToLower().Contains(searchText)).ToList();
                else
                    ListBoxServices.ItemsSource = Services.Where(s => s.Name.ToLower().Contains(searchText)).ToList();
            }
        }

        private void SwitchRoleCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int roleId = (e.AddedItems[0] as Roles).Id;
            Users user = Core.Context.Users.FirstOrDefault(u => u.Roles.Id == roleId);
            Auth.Authorize(user.Login, user.Password);
            Initialize();
        }
    }
}
