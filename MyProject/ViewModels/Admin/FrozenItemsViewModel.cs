using MyProject.Core_;
using MyProject.Database;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows.Input;

namespace MyProject.ViewModels
{
    public class FrozenUserAdminViewModel : BaseViewModel
    {
        private readonly Users _user;
        public string Login => _user.Login;
        public string Name => _user.Name;
        public string Reason => _user.FreezeReason ?? "Без причины";
        public ICommand UnfreezeCommand { get; }
        public FrozenUserAdminViewModel(Users user)
        {
            _user = user;
            UnfreezeCommand = new RelayCommand(_ =>
            {
                user.IsFrozen = false;
                user.FreezeReason = null;
                Core.Context.SaveChanges();
            });
        }
    }

    public class FrozenBookAdminViewModel : BaseViewModel
    {
        private readonly Books _book;
        public string Name => _book.Name;
        public string AuthorName => _book.Users?.Login ?? "?";
        public ICommand UnfreezeCommand { get; }
        public FrozenBookAdminViewModel(Books book)
        {
            _book = book;
            UnfreezeCommand = new RelayCommand(_ =>
            {
                book.IsFrozen = false;
                Core.Context.SaveChanges();
            });
        }
    }

    public class FrozenReviewAdminViewModel : BaseViewModel
    {
        private readonly Reviews _review;
        public string UserLogin => _review.Users?.Login ?? "?";
        public string BookName => _review.Books?.Name ?? "?";
        public string Text => _review.Text;
        public ICommand UnfreezeCommand { get; }
        public FrozenReviewAdminViewModel(Reviews review)
        {
            _review = review;
            UnfreezeCommand = new RelayCommand(_ =>
            {
                review.IsFrozen = false;
                Core.Context.SaveChanges();
            });
        }
    }

    public class FrozenItemsViewModel : BaseViewModel
    {
        public ObservableCollection<FrozenUserAdminViewModel> Users { get; } = new ObservableCollection<FrozenUserAdminViewModel>();
        public ObservableCollection<FrozenBookAdminViewModel> Books { get; } = new ObservableCollection<FrozenBookAdminViewModel>();
        public ObservableCollection<FrozenReviewAdminViewModel> Reviews { get; } = new ObservableCollection<FrozenReviewAdminViewModel>();

        public FrozenItemsViewModel()
        {
            Core.Context.Users.Where(u => u.IsFrozen)
                .ToList().ForEach(u => Users.Add(new FrozenUserAdminViewModel(u)));

            Core.Context.Books.Include(b => b.Users).Where(b => b.IsFrozen)
                .ToList().ForEach(b => Books.Add(new FrozenBookAdminViewModel(b)));

            Core.Context.Reviews.Include(r => r.Users).Include(r => r.Books)
                .Where(r => r.IsFrozen == true)
                .ToList().ForEach(r => Reviews.Add(new FrozenReviewAdminViewModel(r)));
        }
    }
}