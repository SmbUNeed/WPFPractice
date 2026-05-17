using MyProject.Core_;
using MyProject.Database;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MyProject.ViewModels
{
    public class UnfreezeRequestItemViewModel : BaseViewModel
    {
        private readonly Requests _request;
        private readonly string _type;

        public string FiledBy => _request.Users?.Login ?? "?";
        public string Comment => _request.Comment;
        public string TypeLabel => _type == "AccountUnfrozing" ? "Аккаунт" : "Книга";

        private bool _isProcessed;
        public bool IsProcessed
        {
            get => _isProcessed;
            set { _isProcessed = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsActive)); }
        }
        public bool IsActive => !IsProcessed;

        public ICommand AcceptCommand { get; }
        public ICommand RejectCommand { get; }

        public UnfreezeRequestItemViewModel(Requests request, string type)
        {
            _request = request;
            _type = type;
            IsProcessed = request.IsApproved.HasValue;

            AcceptCommand = new RelayCommand(_ => Accept(), _ => IsActive);
            RejectCommand = new RelayCommand(_ => Reject(), _ => IsActive);
        }

        private void Accept()
        {
            _request.IsApproved = true;

            if (_type == "AccountUnfrozing")
            {
                var user = Core.Context.Users.Find(_request.UserId);
                if (user != null) { user.IsFrozen = false; user.FreezeReason = null; }
            }
            else
            {
                var frozenBooks = Core.Context.Books
                    .Where(b => b.AuthorId == _request.UserId && b.IsFrozen)
                    .ToList();
                foreach (var b in frozenBooks) b.IsFrozen = false;
            }

            Core.Context.SaveChanges();
            IsProcessed = true;
        }

        private void Reject()
        {
            _request.IsApproved = false;
            Core.Context.SaveChanges();
            IsProcessed = true;
        }
    }
}