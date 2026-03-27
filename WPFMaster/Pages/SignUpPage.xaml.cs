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
    /// </summary>w
    public partial class SignUpPage : Page
    {
        public SignUpPage()
        {
            InitializeComponent();
        }
        private void SignInPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SignInPage());
        }

        private void SignUp(object sender, RoutedEventArgs e)
        {
            string name = NameBox.Text;
            string login = LoginBox.Text;
            string password = PasswordBox.Text;
            string confirmPassword = ConfirmPasswordBox.Text;
            string email = EmailBox.Text;
            
            if (TrySignUp(login, password, confirmPassword, email))
            {
                Users user = new Users()
                {
                    Name = name,
                    Login = login,
                    Password = password,
                    E_mail = email
                };

                Core.Context.Users.Add(user);

                Core.Context.SaveChanges();

                Session.CurrentUser = user;

                NavigationService.Navigate(new UserPage());
            }
        }

        public bool TrySignUp(string login, string password, string confirmPassword, string email)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (Core.Context.Users.FirstOrDefault(u => u.Login == login) != null)
            {
                MessageBox.Show("Данный логин занят");
                return false;
            }
            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают");
                return false;
            }
            if (!email.Contains("@"))
            {
                MessageBox.Show("Некорректная электронная почта");
                return false;
            }
            return true;
        }
    }
}
