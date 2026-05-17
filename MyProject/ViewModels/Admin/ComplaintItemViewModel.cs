using MyProject.Core_;
using MyProject.Database;
using System.Data.Entity;
using System.Windows.Input;

namespace MyProject.ViewModels
{
    public class ComplaintItemViewModel : BaseViewModel
    {
        private readonly Complaints _complaint;

        public string FiledBy => _complaint.Users?.Login ?? "?";
        public string ReasonText => _complaint.ReasonText;
        public string Target { get; }

        private bool _isResolved;
        public bool IsResolved
        {
            get => _isResolved;
            set { _isResolved = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsActive)); }
        }
        public bool IsActive => !IsResolved;

        public ICommand AcceptCommand { get; }
        public ICommand RejectCommand { get; }

        public ComplaintItemViewModel(Complaints complaint)
        {
            _complaint = complaint;
            IsResolved = complaint.IsResolved == true;

            if (complaint.BookId != null)
                Target = $"Книга: {complaint.Books?.Name}";
            else if (complaint.ReviewId != null)
                Target = $"Отзыв пользователя: {complaint.Reviews?.Users?.Login}";
            else if (complaint.AuthorId != null)
                Target = $"Автор: {complaint.Users1?.Login}";
            else
                Target = "Неизвестно";

            AcceptCommand = new RelayCommand(_ => Resolve(true), _ => IsActive);
            RejectCommand = new RelayCommand(_ => Resolve(false), _ => IsActive);
        }

        private void Resolve(bool accept)
        {
            _complaint.IsResolved = accept;
            Core.Context.SaveChanges();
            IsResolved = true;
        }
    }
}