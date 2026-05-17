using MyProject.Core_;
using MyProject.Database;
using System.Windows.Input;

namespace MyProject.ViewModels
{
    public class ReviewViewModel : BaseViewModel
    {
        private readonly Reviews _review;

        public int Id => _review.Id;
        public string Username => _review.Users?.Name ?? "Неизвестно";
        public byte Rate => _review.Rate;
        public string Date => _review.CreationDate.ToString("dd.MM.yyyy");
        public string Text => _review.Text;
        public bool IsAdmin => SessionService.CurrentUser.Roles.Name == "Admin";
        public bool CanFreeze => IsAdmin && _review.IsFrozen != true;

        public ICommand ComplainCommand { get; }
        public ICommand FreezeCommand { get; }

        public ReviewViewModel(Reviews review)
        {
            _review = review;

            ComplainCommand = new RelayCommand(_ => Complain());
            FreezeCommand = new RelayCommand(_ => Freeze(), _ => CanFreeze);
        }

        private void Complain()
        {
            // TODO: создать жалобу на отзыв
        }

        private void Freeze()
        {
            _review.IsFrozen = true;
            Core.Context.SaveChanges();
            OnPropertyChanged(nameof(CanFreeze));
        }
    }
}