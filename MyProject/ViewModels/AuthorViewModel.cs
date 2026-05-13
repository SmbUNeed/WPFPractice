using MyProject.Core_;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyProject.ViewModels
{
    public class AuthorViewModel : BaseViewModel
    {
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

        public ICommand GoToAddBookCommand { get; }

        public AuthorViewModel()
        {
            GoToAddBookCommand = new RelayCommand(_ => CurrentView = new AddBookViewModel(this));
        }
    }
}
