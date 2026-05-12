using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyProject.Core
{
    internal class ApplicationViewModel : INotifyPropertyChanged
    {
        public static bool IsLoggedIn { get; set; } = false;
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

        public ICommand NavigateCommand { get; }
        public ApplicationViewModel()
        {
            NavigateCommand = new RelayCommand(_ => Navigate());
            Navigate();
        }

        private void Navigate()
        {
            if (!IsLoggedIn)
                CurrentView = new ViewModels.LoginViewModel();
            else
                CurrentView = new ViewModels.HomeViewModel();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
