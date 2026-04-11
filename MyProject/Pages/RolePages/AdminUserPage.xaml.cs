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

namespace MyProject.Pages.RolePages
{
    /// <summary>
    /// Логика взаимодействия для AdminUserPage.xaml
    /// </summary>
    public partial class AdminUserPage : Page
    {
        Users _user;
        public AdminUserPage(Users user)
        {
            _user = user;
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            RolesCombobox.ItemsSource = Core.Context.Roles.ToList();
            RolesCombobox.SelectedItem = _user.Roles;
            EmailBox.Text = _user.Email;
            PhoneNumberBox.Text = _user.PhoneNumber;
            LoginBox.Text = _user.Login;
            FreezeButton.Content = _user.IsFreezed ? "Разморозить" : "Заморозить";
        }

        private void FreezeButton_Click(object sender, RoutedEventArgs e)
        {
            _user.IsFreezed = !_user.IsFreezed;
            FreezeButton.Content = _user.IsFreezed? "Разморозить" : "Заморозить";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _user.Email = EmailBox.Text;
            _user.PhoneNumber = PhoneNumberBox.Text;
            _user.Login = LoginBox.Text;
            _user.RoleId = (RolesCombobox.SelectedItem as Roles).Id;
            Core.Context.SaveChanges();
            MessageBox.Show("Изменения сохранены");
            NavigationService.Navigate(Backend.Access.HomePage());
        }
    }
}
