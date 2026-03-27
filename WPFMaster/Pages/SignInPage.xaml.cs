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
using WPFMaster.Pages;

namespace WPFMaster.Pages
{
    /// <summary>
    /// Логика взаимодействия для SignInPage.xaml
    /// </summary>
    public partial class SignInPage : Page
    {
        public SignInPage()
        {
            InitializeComponent();
        }

        private void SignIn(object sender, RoutedEventArgs e)
        {
            Auth(LoginBox.Text, PasswordBox.Text);
        }

        public bool Auth(string login, string password)
        {
            if (AuthService.Auth(login, password))
            {
                Session.CurrentUser = Core.Context.Users.FirstOrDefault(u => u.Login == login);
                NavigationService.GoBack();
                return true;
            }
            return false;
        }

        private void RegistrationPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SignUpPage());
        }
    }
}
