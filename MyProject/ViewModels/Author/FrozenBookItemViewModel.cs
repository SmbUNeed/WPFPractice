using MyProject.Core_;
using MyProject.Database;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MyProject.ViewModels
{
    public class FrozenBookItemViewModel : BaseViewModel
    {
        private readonly Books _book;
        public string Name => _book.Name;

        private string _appealComment = string.Empty;
        public string AppealComment
        {
            get => _appealComment;
            set { _appealComment = value; OnPropertyChanged(); }
        }

        private bool _hasPendingAppeal;
        public bool HasPendingAppeal
        {
            get => _hasPendingAppeal;
            set { _hasPendingAppeal = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanAppeal)); }
        }
        public bool CanAppeal => !HasPendingAppeal;

        public ICommand AppealCommand { get; }

        public FrozenBookItemViewModel(Books book)
        {
            _book = book;

            var appealTypeId = Core.Context.RequestTypes
                .FirstOrDefault(rt => rt.TypeName == "BookUnfrozing")?.Id;

            HasPendingAppeal = appealTypeId.HasValue && Core.Context.Requests
                .Any(r => r.UserId == SessionService.CurrentUser.Id
                       && r.TypeId == appealTypeId
                       && r.IsApproved == null);

            AppealCommand = new RelayCommand(_ => Appeal(), _ => CanAppeal);
        }

        private void Appeal()
        {
            var appealTypeId = Core.Context.RequestTypes
                .FirstOrDefault(rt => rt.TypeName == "BookUnfrozing")?.Id;
            if (appealTypeId == null) { MessageBox.Show("Тип заявки не найден"); return; }

            Core.Context.Requests.Add(new Requests
            {
                TypeId = appealTypeId.Value,
                UserId = SessionService.CurrentUser.Id,
                Comment = AppealComment,
                IsApproved = null
            });
            Core.Context.SaveChanges();

            HasPendingAppeal = true;
            AppealComment = string.Empty;
            MessageBox.Show("Заявка на разморозку отправлена");
        }
    }
}