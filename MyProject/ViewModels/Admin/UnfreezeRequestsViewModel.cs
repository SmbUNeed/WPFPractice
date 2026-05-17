using MyProject.Core_;
using MyProject.Database;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;

namespace MyProject.ViewModels
{
    public class UnfreezeRequestsViewModel : BaseViewModel
    {
        public ObservableCollection<UnfreezeRequestItemViewModel> Requests { get; } = new ObservableCollection<UnfreezeRequestItemViewModel>();

        public UnfreezeRequestsViewModel()
        {
            var types = Core.Context.RequestTypes
                .Where(rt => rt.TypeName == "AccountUnfrozing" || rt.TypeName == "BookUnfrozing")
                .ToList();

            var typeIds = types.Select(t => t.Id).ToList();

            var requests = Core.Context.Requests
                .Include(r => r.Users)
                .Include(r => r.RequestTypes)
                .Where(r => typeIds.Contains(r.TypeId) && r.IsApproved == null)
                .ToList();

            foreach (var r in requests)
                Requests.Add(new UnfreezeRequestItemViewModel(r, r.RequestTypes.TypeName));
        }
    }
}