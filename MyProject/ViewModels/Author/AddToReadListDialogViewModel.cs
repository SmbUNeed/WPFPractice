using MyProject.Core_;
using MyProject.Database;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MyProject.ViewModels
{
    public class AddToReadListViewModel : BaseViewModel
    {
        private readonly Books _book;
        private readonly Window _window;

        public List<ReadStatuses> ReadStatuses { get; }

        private ReadStatuses _selectedStatus;
        public ReadStatuses SelectedStatus
        {
            get => _selectedStatus;
            set { _selectedStatus = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; }
        public ICommand CancelCommand { get; }

        public AddToReadListViewModel(Books book, Window window)
        {
            _book = book;
            _window = window;

            ReadStatuses = Core.Context.ReadStatuses.ToList();
            SelectedStatus = ReadStatuses.FirstOrDefault();

            AddCommand = new RelayCommand(_ => Add(), _ => SelectedStatus != null);
            CancelCommand = new RelayCommand(_ => _window.Close());
        }

        private void Add()
        {
            int userId = SessionService.CurrentUser.Id;

            var existing = Core.Context.ReadList
                .FirstOrDefault(r => r.UserId == userId && r.BookId == _book.Id);

            if (existing != null)
                existing.StatusId = SelectedStatus.Id;
            else
                Core.Context.ReadList.Add(new ReadList
                {
                    UserId = userId,
                    BookId = _book.Id,
                    StatusId = SelectedStatus.Id
                });

            Core.Context.SaveChanges();

            MessageBox.Show("Книга добавлена в список чтения", "Добавлено");
            _window.DialogResult = true;
            _window.Close();
        }
    }
}