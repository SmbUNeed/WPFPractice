using MyProject.Core_;
using MyProject.Database;
using MyProject.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace MyProject.ViewModels
{
    public class BookListsViewModel : BaseViewModel
    {
        private Action<Books> _openBook;
        public List<ReadStatuses> ReadStatuses { get; }
        private readonly ObservableCollection<Books> _books;
        public ICollectionView BooksView { get; }
        private ReadStatuses _selectedStatus;
        public ReadStatuses SelectedStatus
        {
            get => _selectedStatus;
            set { _selectedStatus = value; OnPropertyChanged(); BooksView.Refresh(); }
        }
        public ICommand ChangeStatusCommand { get; }
        public ICommand OpenBookCommand { get; }
        public ICommand AddToReadListCommand { get; }
        public BookListsViewModel(Action<Books> openBook)
        {
            _openBook = openBook;
            ReadStatuses = Core.Context.ReadStatuses.ToList();

            _books = new ObservableCollection<Books>(
                Core.Context.Books
                .Include(b => b.ReadList)
                .Include(b => b.Users)
                .Where(b => b.ReadList.Any(rl => rl.UserId == SessionService.CurrentUser.Id))
            );

            BooksView = CollectionViewSource.GetDefaultView(_books);
            BooksView.Filter = obj =>
            {
                if (SelectedStatus == null) return true;
                return (obj as Books).ReadList.Any(rl =>
                    rl.UserId == SessionService.CurrentUser.Id && rl.StatusId == SelectedStatus.Id);
            };

                ChangeStatusCommand = new RelayCommand(status => SelectedStatus = (ReadStatuses)status);

            OpenBookCommand = new RelayCommand(book => _openBook((Books)book));
            AddToReadListCommand = new RelayCommand(book =>
            {
                var dialog = new AddToReadListDialog();
                dialog.DataContext = new AddToReadListViewModel((Books)book, dialog);
                dialog.Owner = Application.Current.MainWindow;
                dialog.ShowDialog();
                BooksView.Refresh();
            });

            SelectedStatus = ReadStatuses.First(rs => rs.Name == "В планах");
        }
    }
}
