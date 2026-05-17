using MyProject.Core_;
using MyProject.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MyProject.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _login;
        private string _password;
        public string Login
        {
            get => _login;
            set { _login = value; OnPropertyChanged(); }
        }
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }
        public ICommand ToRegisterCommand { get; }
        public ICommand LoginCommand { get; }
        public LoginViewModel()
        {
            ToRegisterCommand = new RelayCommand(_ => NavigationService.Navigate(new RegisterViewModel()));
            LoginCommand = new RelayCommand(DoLogin);
        }
        private void DoLogin(object parameter)
        {
            string password = (parameter as System.Windows.Controls.PasswordBox)?.Password;

            if (string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните все поля!", "Незаполненные поля");
                return;
            }

            Users user = Core.Context.Users.FirstOrDefault(u => u.Login == Login && u.Password == password);
            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль", "Ввод");
                return;
            }

            SessionService.CurrentUser = user;
            NavigationService.Navigate(new HomeViewModel());
        }
    }
}
