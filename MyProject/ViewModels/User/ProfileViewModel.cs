using MyProject.Core_;
using MyProject.Database;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MyProject.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly Users _user;

        public string Name => _user.Name;
        public string Login => _user.Login;
        public string Email => _user.Email;
        public string Role => _user.Roles?.Name ?? "—";

        public bool IsFrozen => _user.IsFrozen;
        public string FreezeReason => _user.FreezeReason ?? "Причина не указана";

        private bool _hasPendingAppeal;
        public bool HasPendingAppeal
        {
            get => _hasPendingAppeal;
            set { _hasPendingAppeal = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanAppeal)); }
        }

        private bool _hasPendingAuthorRequest;
        public bool HasPendingAuthorRequest
        {
            get => _hasPendingAuthorRequest;
            set { _hasPendingAuthorRequest = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanRequestAuthor)); }
        }

        public bool CanAppeal => IsFrozen && !HasPendingAppeal;
        public bool CanRequestAuthor => _user.Roles?.Name != "Author" && !HasPendingAuthorRequest;

        private string _appealComment = string.Empty;
        public string AppealComment
        {
            get => _appealComment;
            set { _appealComment = value; OnPropertyChanged(); }
        }

        private string _authorComment = string.Empty;
        public string AuthorComment
        {
            get => _authorComment;
            set { _authorComment = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Reviews> Reviews { get; } = new ObservableCollection<Reviews>();

        public ICommand SubmitAppealCommand { get; }
        public ICommand SubmitAuthorRequestCommand { get; }

        public ProfileViewModel()
        {
            _user = SessionService.CurrentUser;

            var reviews = Core.Context.Reviews
                .Include(r => r.Books)
                .Where(r => r.UserId == _user.Id && r.IsFrozen != true)
                .ToList();

            foreach (var r in reviews) Reviews.Add(r);

            var appealTypeId = Core.Context.RequestTypes
                .FirstOrDefault(rt => rt.TypeName == "AccountUnfrozing")?.Id;
            var authorTypeId = Core.Context.RequestTypes
                .FirstOrDefault(rt => rt.TypeName == "Author")?.Id;

            HasPendingAppeal = appealTypeId.HasValue && Core.Context.Requests
                .Any(r => r.UserId == _user.Id && r.TypeId == appealTypeId && r.IsApproved == null);

            HasPendingAuthorRequest = authorTypeId.HasValue && Core.Context.Requests
                .Any(r => r.UserId == _user.Id && r.TypeId == authorTypeId && r.IsApproved == null);

            SubmitAppealCommand = new RelayCommand(_ => SubmitAppeal(), _ => CanAppeal);
            SubmitAuthorRequestCommand = new RelayCommand(_ => SubmitAuthorRequest(), _ => CanRequestAuthor);
        }

        private void SubmitAppeal()
        {
            var appealTypeId = Core.Context.RequestTypes
                .FirstOrDefault(rt => rt.TypeName == "AccountUnfrozing")?.Id;
            if (appealTypeId == null) { MessageBox.Show("Тип заявки не найден"); return; }

            Core.Context.Requests.Add(new Requests
            {
                TypeId = appealTypeId.Value,
                UserId = _user.Id,
                Comment = AppealComment,
                IsApproved = null
            });
            Core.Context.SaveChanges();

            HasPendingAppeal = true;
            AppealComment = string.Empty;
            MessageBox.Show("Заявка на оспаривание отправлена");
        }

        private void SubmitAuthorRequest()
        {
            var authorTypeId = Core.Context.RequestTypes
                .FirstOrDefault(rt => rt.TypeName == "Author")?.Id;
            if (authorTypeId == null) { MessageBox.Show("Тип заявки не найден"); return; }

            Core.Context.Requests.Add(new Requests
            {
                TypeId = authorTypeId.Value,
                UserId = _user.Id,
                Comment = AuthorComment,
                IsApproved = null
            });
            Core.Context.SaveChanges();

            HasPendingAuthorRequest = true;
            AuthorComment = string.Empty;
            MessageBox.Show("Заявка на роль Автора отправлена");
        }
    }
}