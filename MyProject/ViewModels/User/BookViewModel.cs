using MyProject.Core_;
using MyProject.Database;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MyProject.ViewModels
{
    public class BookViewModel : BaseViewModel
    {
        private readonly Action _goBack;
        public Books Book { get; }

        public string Title => Book.Name;
        public string Author => Book.Users?.Name ?? "Неизвестно";
        public string Genres => string.Join(", ", Book.Genres.Select(g => g.Name));
        public string Cover => Book.CoverPath;
        public bool IsAdmin => SessionService.CurrentUser.Roles.Name == "Admin";

        private string _reviewText = string.Empty;
        public string ReviewText
        {
            get => _reviewText;
            set { _reviewText = value; OnPropertyChanged(); }
        }

        private string _reviewRate = string.Empty;
        public string ReviewRate
        {
            get => _reviewRate;
            set { _reviewRate = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ReviewViewModel> Reviews { get; } = new ObservableCollection<ReviewViewModel>();

        public ICommand ToCatalogCommand { get; }
        public ICommand ReadCommand { get; }
        public ICommand ComplainBookCommand { get; }
        public ICommand ComplainAuthorCommand { get; }
        public ICommand FreezeBookCommand { get; }
        public ICommand SubmitReviewCommand { get; }

        public BookViewModel(Books book, Action goBack)
        {
            Book = book;
            _goBack = goBack;

            var reviews = Core.Context.Reviews
                .Where(r => r.BookId == book.Id && r.IsFrozen != true)
                .ToList();

            foreach (var r in reviews)
                Reviews.Add(new ReviewViewModel(r));

            ToCatalogCommand = new RelayCommand(_ => _goBack());
            ReadCommand = new RelayCommand(_ => Read());
            ComplainBookCommand = new RelayCommand(_ => ComplainBook());
            ComplainAuthorCommand = new RelayCommand(_ => ComplainAuthor());
            FreezeBookCommand = new RelayCommand(_ => FreezeBook(), _ => IsAdmin);
            SubmitReviewCommand = new RelayCommand(_ => SubmitReview());
        }

        private void Read()
        {
            NavigationService.Navigate(new ReadViewModel(Book));
        }

        private void ComplainBook()
        {
            var dlg = new Views.ComplaintDialog();
            if (dlg.ShowDialog() == true)
            {
                Core.Context.Complaints.Add(new Complaints
                {
                    UserId = SessionService.CurrentUser.Id,
                    BookId = Book.Id,
                    ReasonText = dlg.ComplaintText
                });
                Core.Context.SaveChanges();
            }
        }

        private void ComplainAuthor()
        {
            var dlg = new Views.ComplaintDialog();
            if (dlg.ShowDialog() == true)
            {
                Core.Context.Complaints.Add(new Complaints
                {
                    UserId = SessionService.CurrentUser.Id,
                    AuthorId = Book.AuthorId,
                    ReasonText = dlg.ComplaintText
                });
                Core.Context.SaveChanges();
            }
        }

        private void FreezeBook()
        {
            Book.IsFrozen = true;
            Core.Context.SaveChanges();
            _goBack();
        }

        private void SubmitReview()
        {
            if (!byte.TryParse(ReviewRate, out byte rate) || rate < 1 || rate > 10)
            {
                MessageBox.Show("Оценка должна быть числом от 1 до 10");
                return;
            }
            if (string.IsNullOrWhiteSpace(ReviewText))
            {
                MessageBox.Show("Введите текст отзыва");
                return;
            }

            var review = new Reviews
            {
                UserId = SessionService.CurrentUser.Id,
                BookId = Book.Id,
                Text = ReviewText,
                Rate = rate,
                CreationDate = System.DateTime.Now,
                IsFrozen = false
            };

            Core.Context.Reviews.Add(review);
            Core.Context.SaveChanges();

            Reviews.Add(new ReviewViewModel(review));

            ReviewText = string.Empty;
            ReviewRate = string.Empty;
        }
    }
}