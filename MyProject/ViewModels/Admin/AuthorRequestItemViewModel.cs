using MyProject.Core_;
using MyProject.Database;
using System.Linq;
using System.Windows.Input;

namespace MyProject.ViewModels
{
    public class AuthorRequestItemViewModel : BaseViewModel
    {
        private readonly Requests _request;

        public string FiledBy => _request.Users?.Login ?? "?";
        public string Comment => _request.Comment;

        private bool _isProcessed;
        public bool IsProcessed
        {
            get => _isProcessed;
            set { _isProcessed = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsActive)); }
        }
        public bool IsActive => !IsProcessed;

        public ICommand AcceptCommand { get; }
        public ICommand RejectCommand { get; }

        public AuthorRequestItemViewModel(Requests request)
        {
            _request = request;
            IsProcessed = request.IsApproved.HasValue;

            AcceptCommand = new RelayCommand(_ => Accept(), _ => IsActive);
            RejectCommand = new RelayCommand(_ => Reject(), _ => IsActive);
        }

        private void Accept()
        {
            _request.IsApproved = true;

            var authorRole = Core.Context.Roles.FirstOrDefault(r => r.Name == "Author");
            if (authorRole != null)
            {
                var user = Core.Context.Users.Find(_request.UserId);
                if (user != null) user.RoleId = authorRole.Id;
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