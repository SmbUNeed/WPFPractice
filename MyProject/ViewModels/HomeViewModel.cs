using MyProject.Core_;
using MyProject.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyProject.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private object _currentTab;
        public object CurrentTab
        {
            get => _currentTab;
            set { _currentTab = value; OnPropertyChanged(); }
        }

        private readonly List<Books> _books = Core.Context.Books.Where(b => !b.IsFrozen).ToList();
        public List<Books> Books => _books;
        
        public bool IsAdmin => SessionService.CurrentUser.Roles.Name == "Admin";
        public bool IsAuthor => SessionService.CurrentUser.Roles.Name == "Author";
        public bool IsFrozen => SessionService.CurrentUser.IsFrozen;
        
        public ICommand OpenAdminCommand { get; }
        public ICommand OpenAuthorCommand { get; }
        public ICommand OpenProfileCommand { get; }
        public ICommand OpenCatalogCommand { get; } 
        public ICommand OpenReadListsCommand { get; }

        public HomeViewModel()
        {
            OpenAdminCommand = new RelayCommand(_ => CurrentTab = new AdminViewModel());
            OpenAuthorCommand = new RelayCommand(_ => CurrentTab = new AuthorViewModel());
            OpenProfileCommand = new RelayCommand(_ => CurrentTab = new ProfileViewModel());
            OpenCatalogCommand = new RelayCommand(_ => CurrentTab = new CatalogViewModel());
            OpenReadListsCommand = new RelayCommand(_ => CurrentTab = new ReadListsViewModel());
        }
    }
}
