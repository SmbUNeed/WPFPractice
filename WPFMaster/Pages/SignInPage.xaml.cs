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
            Users user = Core.Context.Users.FirstOrDefault(u => u.Login == LoginBox.Text);
            if (user?.Password == PasswordBox.Text)
            {
                Session.Instance.CurrentUser = user;
                NavigationService.Navigate(new UserPage());
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль");
            }
        }

        private void Registration(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SignUpPage());
        }
    }
}
