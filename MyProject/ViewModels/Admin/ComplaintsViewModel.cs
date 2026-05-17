using MyProject.Core_;
using MyProject.Database;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;

namespace MyProject.ViewModels
{
    public class ComplaintsViewModel : BaseViewModel
    {
        public ObservableCollection<ComplaintItemViewModel> Complaints { get; } = new ObservableCollection<ComplaintItemViewModel>();

        public ComplaintsViewModel()
        {
            var complaints = Core.Context.Complaints
                .Include(c => c.Users)
                .Include(c => c.Users1) 
                .Include(c => c.Books)
                .Include(c => c.Reviews.Users)
                .Where(c => c.IsResolved == null || c.IsResolved == false)
                .ToList();

            foreach (var c in complaints)
                Complaints.Add(new ComplaintItemViewModel(c));
        }
    }
}