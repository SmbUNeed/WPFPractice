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

namespace MyProject.ViewModels
{
    public class CatalogViewModel : BaseViewModel
    {
        private readonly ObservableCollection<Books> _books;
        public ICollectionView BooksView { get; }
        public List<Genres> Genres { get; }
        public List<string> SearchTypes { get; } = new List<string> { "Название", "Автор" };

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); BooksView.Refresh(); }
        }
        private Genres _selectedGenre;
        public Genres SelectedGenre
        {
            get => _selectedGenre;
            set { _selectedGenre = value; OnPropertyChanged(); BooksView.Refresh(); }
        }

        private string _selectedSearchType = "Название";
        public string SelectedSearchType
        {
            get => _selectedSearchType;
            set { _selectedSearchType = value; OnPropertyChanged(); BooksView.Refresh(); }
        }

        public ICommand OpenBookCommand { get; }
        public CatalogViewModel()
        {
            _books = new ObservableCollection<Books>(
                Core.Context.Books
                .Include(b => b.Genres)
                .Include(b => b.Users)
                .Where(b => !b.IsFrozen));
            Genres = Core.Context.Genres.ToList();

            BooksView = CollectionViewSource.GetDefaultView(_books);
            BooksView.Filter = FilterBooks;
            OpenBookCommand = new RelayCommand(book => NavigationService.Navigate(new BookViewModel(this, (Books)book)));
        }

        private bool FilterBooks(object obj)
        {
            Books book = (Books)obj;

            bool matchesGenre = SelectedGenre == null || (book.Genres != null && book.Genres.Any(g => g.Id == SelectedGenre.Id));

            bool matchesSearch = string.IsNullOrEmpty(SearchText) ||
                (SelectedSearchType == "Название" && book.Name.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (SelectedSearchType == "Автор" && book.Users?.Name.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0);

            return matchesGenre && matchesSearch;
        }
    }
}
