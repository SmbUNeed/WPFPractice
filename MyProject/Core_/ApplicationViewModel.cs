using MyProject.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyProject.Core_
{
    public class ApplicationViewModel : BaseViewModel
    {
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

        public ICommand NavigateCommand { get; }
        public ApplicationViewModel()
        {
            NavigationService.NavigationChanged += vm => CurrentView = vm;
            NavigateCommand = new RelayCommand(_ => Navigate());
            Navigate();
        }

        private void Navigate()
        {
            if (!SessionService.LoggedIn)
                CurrentView = new LoginViewModel();
            else
            {
                CurrentView = new HomeViewModel();
            }
        }
    }
}
