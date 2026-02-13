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
    /// Логика взаимодействия для SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        public SignUpPage()
        {
            InitializeComponent();
        }
        private void SignInPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SignUpPage());
        }

        private bool SignUp(object sender, RoutedEventArgs e)
        {
            if (Core.Context.Users.FirstOrDefault(u => u.Login == LoginBox.Text) != null)
            {
                MessageBox.Show("Данный логин занят");
                return false;
            }
            if (PasswordBox.Text != ConfirmPasswordBox.Text)
            {
                MessageBox.Show("Пароли не совпадают");
                return false;
            }
            if (!EmailBox.Text.Contains("@"))
            {
                MessageBox.Show("Некорректная электронная почта");
                return false;
            }
            return true;
        }
    }
}
