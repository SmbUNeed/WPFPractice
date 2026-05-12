using MyProject.Core_;
using MyProject.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MyProject.ViewModels
{
    internal class RegisterViewModel : BaseViewModel
    {
        private string _login;
        private string _name;
        private string _email;

        public string Login
        {
            get => _login;
            set { _login = value; OnPropertyChanged(); }
        }
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }
        public ICommand ToAuthCommand { get; }

        public RegisterViewModel()
        {
            ToAuthCommand = new RelayCommand(_ => NavigationService.Navigate(new LoginViewModel()));
        }

        public void DoRegister(string password, string repeatPassword)
        {
            ValidRegister(password, repeatPassword);

            Users user = new Users
            {
                Name = Name,
                Login = Login,
                Email = Email,
                Password = password,
                IsFrozen = false,
                RegistationDate = DateTime.Now,
                RoleId = Core.Context.Roles.First(r => r.Name == "Reader").Id
            };

            Core.Context.Users.Add(user);
            Core.Context.SaveChanges();

            NavigationService.Navigate(new LoginViewModel());
        }

        private bool ValuesIsNullOrEmpty(params string[] values)
        {
            foreach(string value in values)
            {
                if (string.IsNullOrEmpty(value)) return true;
            }
            return false;
        }

        private void ValidRegister(string password, string repeatPassword)
        {
            if (ValuesIsNullOrEmpty(Login, Name, Email, password, repeatPassword))
            {
                MessageBox.Show("Не все поля заполнены", "Ошибка ввода");
                return;
            }
            if (!Email.Contains('@') || !Email.Contains('.'))
            {
                MessageBox.Show("Введите корректный e-mail", "Ошибка ввода");
                return;
            }
            if (password != repeatPassword)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка ввода");
                return;
            }
            if (Core.Context.Users.Any(u => u.Login == Login))
            {
                MessageBox.Show("Пользователь с таким логином уже зарегистрирован", "Ошибка ввода");
                return;
            }
            if (Core.Context.Users.Any(u => u.Email == Email))
            {
                MessageBox.Show("Пользователь с таким e-mail уже зарегистрирован", "Ошибка ввода");
                return;
            }
        }
    }
}
