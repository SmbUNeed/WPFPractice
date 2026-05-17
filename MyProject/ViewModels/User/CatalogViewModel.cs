using MyProject.Core_;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyProject.Database;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Data.Entity;
using MyProject.Views;
using System.Windows;

namespace MyProject.ViewModels
{
    public class CatalogViewModel : BaseViewModel
    {
        private readonly Action<Books> _openBook;
        private readonly ObservableCollection<Books> _books;
        public ICollectionView BooksView { get; }

        private string _selectedGenre = "Все жанры";
        public string SelectedGenre
        {
            get => _selectedGenre;
            set { _selectedGenre = value; OnPropertyChanged(); BooksView.Refresh(); }
        }
        public List<string> Genres { get; }
        public List<string> SearchTypes { get; } = new List<string> { "Название", "Автор" };

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); BooksView.Refresh(); }
        }

        private string _selectedSearchType = "Название";
        public string SelectedSearchType
        {
            get => _selectedSearchType;
            set { _selectedSearchType = value; OnPropertyChanged(); BooksView.Refresh(); }
        }

        public ICommand OpenBookCommand { get; }
        public ICommand AddToReadListCommand { get; }
        public CatalogViewModel(Action<Books> openBook)
        {
            _openBook = openBook;

            _books = new ObservableCollection<Books>(
                Core.Context.Books
                .Include(b => b.Genres)
                .Include(b => b.Users)
                .Where(b => !b.IsFrozen));

            BooksView = CollectionViewSource.GetDefaultView(_books);
            BooksView.Filter = FilterBooks;

            Genres = Core.Context.Genres.Select(g => g.Name).ToList();
            Genres.Insert(0, "Все жанры");

            OpenBookCommand = new RelayCommand(book => _openBook((Books)book));
            AddToReadListCommand = new RelayCommand(book =>
            {
                var dialog = new AddToReadListDialog();
                dialog.DataContext = new AddToReadListViewModel((Books)book, dialog);
                dialog.Owner = Application.Current.MainWindow;
                dialog.ShowDialog();
            });
        }

        private bool FilterBooks(object obj)
        {
            Books book = (Books)obj;

            bool matchesGenre = SelectedGenre == "Все жанры" || (book.Genres != null && book.Genres.Any(g => g.Name == SelectedGenre));

            bool matchesSearch = string.IsNullOrEmpty(SearchText) ||
                (SelectedSearchType == "Название" && book.Name.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (SelectedSearchType == "Автор" && book.Users?.Name.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0);

            return matchesGenre && matchesSearch;
        }
    }
}
