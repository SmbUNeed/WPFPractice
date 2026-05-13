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
    public class AuthorBooksViewModel : BaseViewModel
    {
        private AuthorViewModel _parent;
        private readonly List<Books> _books = Core.Context.Books.Where(b => b.AuthorId == SessionService.CurrentUser.Id).ToList();
        public List<Books> Books { get => _books; }
        public ICommand GoToAuthor { get; }
        public AuthorBooksViewModel(AuthorViewModel parent)
        {
            _parent = parent;
            GoToAuthor = new RelayCommand(_ => NavigationService.Navigate(new AuthorViewModel()));
        }
    }
}
