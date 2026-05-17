using MyProject.Core_;
using System.Windows.Input;

namespace MyProject.ViewModels
{
    public class AdminViewModel : BaseViewModel
    {
        private object _currentTab;
        public object CurrentTab
        {
            get => _currentTab;
            set { _currentTab = value; OnPropertyChanged(); }
        }

        public ICommand OpenComplaintsCommand { get; }
        public ICommand OpenUnfreezeCommand { get; }
        public ICommand OpenAuthorRequestsCommand { get; }
        public ICommand OpenFrozenCommand { get; }
        public ICommand OpenUsersCommand { get; }

        public AdminViewModel()
        {
            CurrentTab = new ComplaintsViewModel();

            OpenComplaintsCommand = new RelayCommand(_ => CurrentTab = new ComplaintsViewModel());
            OpenUnfreezeCommand = new RelayCommand(_ => CurrentTab = new UnfreezeRequestsViewModel());
            OpenAuthorRequestsCommand = new RelayCommand(_ => CurrentTab = new AuthorRequestsViewModel());
            OpenFrozenCommand = new RelayCommand(_ => CurrentTab = new FrozenItemsViewModel());
            OpenUsersCommand = new RelayCommand(_ => CurrentTab = new UsersAdminViewModel());
        }
    }
}