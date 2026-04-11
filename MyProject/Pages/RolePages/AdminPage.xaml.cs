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

namespace MyProject.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
            UsersList.ItemsSource = Core.Context.Users.ToList();
        }

        private void UsersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UsersList.SelectedItem == null) return;
            NavigationService.Navigate(new RolePages.AdminUserPage(UsersList.SelectedItem as Users));
            UsersList.SelectedItem = null;
        }

        private void NewUserButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RolePages.NewUserPage());
        }
    }
}
