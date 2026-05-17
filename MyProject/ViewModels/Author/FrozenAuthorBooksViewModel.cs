using MyProject.Core_;
using MyProject.Database;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyProject.ViewModels
{
    public class FrozenAuthorBooksViewModel : BaseViewModel
    {
        public bool HasNoBooks => Books.Count == 0;
        public ObservableCollection<FrozenBookItemViewModel> Books { get; } = new ObservableCollection<FrozenBookItemViewModel>();

        public FrozenAuthorBooksViewModel()
        {
            var books = Core.Context.Books
                .Where(b => b.AuthorId == SessionService.CurrentUser.Id && b.IsFrozen)
                .ToList();

            foreach (var b in books)
                Books.Add(new FrozenBookItemViewModel(b));
        }
    }
}