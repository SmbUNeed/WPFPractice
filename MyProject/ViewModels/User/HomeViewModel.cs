using MyProject.Core_;
using MyProject.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MyProject.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private object _currentContent;
        public object CurrentContent
        {
            get => _currentContent;
            set {
                if (SessionService.CurrentUser.IsFrozen && !(value is ProfileViewModel))
                {
                    MessageBox.Show("Ваш аккаунт заморожен, вы можете оставить заявку на его разморозку.", "Заморозка аккаунта");
                    return;
                }
                _currentContent = value; 
                OnPropertyChanged(); }
        }
        
        public bool IsAdmin => SessionService.CurrentUser.Roles.Name == "Admin";
        public bool IsAuthor => SessionService.CurrentUser.Roles.Name == "Author" || IsAdmin;
        public bool IsFrozen => SessionService.CurrentUser.IsFrozen;
        
        public ICommand OpenAdminCommand { get; }
        public ICommand OpenAuthorCommand { get; }
        public ICommand OpenProfileCommand { get; }
        public ICommand OpenCatalogCommand { get; } 
        public ICommand OpenReadListsCommand { get; }

        public HomeViewModel()
        {
            OpenCatalogCommand = new RelayCommand(_ => CurrentContent = CreateCatalog(), _ => !IsFrozen);
            OpenReadListsCommand = new RelayCommand(_ => CurrentContent = CreateReadList(), _ => !IsFrozen);
            OpenAdminCommand = new RelayCommand(_ => CurrentContent = new AdminViewModel(), _ => !IsFrozen && IsAdmin);

            OpenProfileCommand = new RelayCommand(_ => CurrentContent = new ProfileViewModel());
            OpenAuthorCommand = new RelayCommand(_ => CurrentContent = CreateAuthor(), _ => !IsFrozen && IsAuthor);
        }

        private CatalogViewModel CreateCatalog() =>
            new CatalogViewModel(book =>
                CurrentContent = new BookViewModel(book, () =>
                    CurrentContent = CreateCatalog()));

        private AuthorViewModel CreateAuthor() =>
            new AuthorViewModel(book =>
                CurrentContent = new BookViewModel(book, () =>
                    CurrentContent = CreateAuthor()));

        private BookListsViewModel CreateReadList()
        {
            return new BookListsViewModel(book =>
                CurrentContent = new BookViewModel(book, () =>
                    CurrentContent = CreateCatalog()));
        }
    }
}
