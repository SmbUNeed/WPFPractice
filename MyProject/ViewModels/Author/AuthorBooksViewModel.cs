using MyProject.Core_;
using MyProject.Database;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows.Input;

namespace MyProject.ViewModels
{
    public class AuthorBooksViewModel : BaseViewModel
    {
        private readonly Action<Books> _editBook;
        private readonly Action<Books> _openBook;

        public ObservableCollection<Books> Books { get; } = new ObservableCollection<Books>();

        public ICommand EditCommand { get; }
        public ICommand OpenCommand { get; }

        public AuthorBooksViewModel(Action<Books> editBook, Action<Books> openBook)
        {
            _editBook = editBook;
            _openBook = openBook;

            var books = Core.Context.Books
                .Include(b => b.Genres)
                .Where(b => b.AuthorId == SessionService.CurrentUser.Id && !b.IsFrozen)
                .ToList();

            foreach (var b in books) Books.Add(b);

            EditCommand = new RelayCommand(b => _editBook((Books)b));
            OpenCommand = new RelayCommand(b => _openBook((Books)b));
        }
    }
}