using MyProject.Core_;
using MyProject.Database;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MyProject.ViewModels
{
    public class UserAdminItemViewModel : BaseViewModel
    {
        private readonly Users _user;

        public string Login => _user.Login;
        public string Name => _user.Name;
        public string Email => _user.Email;
        public List<Roles> AllRoles { get; }

        private Roles _selectedRole;
        public Roles SelectedRole
        {
            get => _selectedRole;
            set { _selectedRole = value; OnPropertyChanged(); }
        }

        private string _newPassword = string.Empty;
        public string NewPassword
        {
            get => _newPassword;
            set { _newPassword = value; OnPropertyChanged(); }
        }

        public ICommand SaveRoleCommand { get; }
        public ICommand SavePasswordCommand { get; }

        public UserAdminItemViewModel(Users user)
        {
            _user = user;
            AllRoles = Core.Context.Roles.ToList();
            SelectedRole = AllRoles.FirstOrDefault(r => r.Id == user.RoleId);

            SaveRoleCommand = new RelayCommand(_ =>
            {
                if (SelectedRole == null) return;
                _user.RoleId = SelectedRole.Id;
                Core.Context.SaveChanges();
                MessageBox.Show($"Роль {_user.Login} изменена на {SelectedRole.Name}");
            });

            SavePasswordCommand = new RelayCommand(_ =>
            {
                if (string.IsNullOrWhiteSpace(NewPassword))
                {
                    MessageBox.Show("Введите новый пароль");
                    return;
                }
                _user.Password = NewPassword;
                Core.Context.SaveChanges();
                NewPassword = string.Empty;
                MessageBox.Show($"Пароль {_user.Login} изменён");
            });
        }
    }
}