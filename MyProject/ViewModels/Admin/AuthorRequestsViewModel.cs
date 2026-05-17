using MyProject.Core_;
using MyProject.Database;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;

namespace MyProject.ViewModels
{
    public class AuthorRequestsViewModel : BaseViewModel
    {
        public ObservableCollection<AuthorRequestItemViewModel> Requests { get; } = new ObservableCollection<AuthorRequestItemViewModel>();

        public AuthorRequestsViewModel()
        {
            var authorTypeId = Core.Context.RequestTypes
                .FirstOrDefault(rt => rt.TypeName == "Author")?.Id;

            if (authorTypeId == null) return;

            var requests = Core.Context.Requests
                .Include(r => r.Users)
                .Where(r => r.TypeId == authorTypeId && r.IsApproved == null)
                .ToList();

            foreach (var r in requests)
                Requests.Add(new AuthorRequestItemViewModel(r));
        }
    }
}