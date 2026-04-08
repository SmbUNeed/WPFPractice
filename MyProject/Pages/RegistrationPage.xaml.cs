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
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (
                    VisualModal.MessageIfFalse(PasswordBox.Text == RepeatPasswordBox.Text, "Пароли не совпадают") &&
                    VisualModal.MessageIfFalse(EmailBox.Text.Contains("@") && EmailBox.Text.Contains("."), "Невалидный email") &&
                    VisualModal.MessageIfFalse(PhoneNumberBox.Text[0] == '+' || PhoneNumberBox.Text[0] == '7' || PhoneNumberBox.Text[0] == '8', "Невалидный номер телефона")
                )
            {
                VisualModal.MessageIfFalse(Auth.Register(LoginBox.Text, NameBox.Text, EmailBox.Text, PhoneNumberBox.Text, PasswordBox.Text), "Ошибка регистрации");
                NavigationService.Navigate(new LoginPage());
                return;
            }
        }
    }
}
