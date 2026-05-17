using MyProject.Core_;
using MyProject.Database;
using System;
using System.Windows.Input;

namespace MyProject.ViewModels
{
    public class AuthorViewModel : BaseViewModel
    {
        private readonly Action<Books> _openBook;

        private object _currentTab;
        public object CurrentTab
        {
            get => _currentTab;
            set { _currentTab = value; OnPropertyChanged(); }
        }

        public ICommand OpenBooksCommand { get; }
        public ICommand OpenAddBookCommand { get; }
        public ICommand OpenFrozenCommand { get; }

        public AuthorViewModel(Action<Books> openBook)
        {
            _openBook = openBook;
            CurrentTab = CreateBooksTab();

            OpenBooksCommand = new RelayCommand(_ => CurrentTab = CreateBooksTab());
            OpenAddBookCommand = new RelayCommand(_ => CurrentTab = CreateAddEditTab(null));
            OpenFrozenCommand = new RelayCommand(_ => CurrentTab = new FrozenAuthorBooksViewModel());
        }

        private AuthorBooksViewModel CreateBooksTab() =>
            new AuthorBooksViewModel(
                editBook: book => CurrentTab = CreateAddEditTab(book),
                openBook: _openBook);

        private AddEditBookViewModel CreateAddEditTab(Books book) =>
            new AddEditBookViewModel(book, () => CurrentTab = CreateBooksTab());
    }
}