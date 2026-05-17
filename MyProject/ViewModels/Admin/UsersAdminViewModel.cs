using MyProject.Core_;
using MyProject.Database;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;

namespace MyProject.ViewModels
{
    public class UsersAdminViewModel : BaseViewModel
    {
        public ObservableCollection<UserAdminItemViewModel> Users { get; } = new ObservableCollection<UserAdminItemViewModel>();

        public UsersAdminViewModel()
        {
            Core.Context.Users.Include(u => u.Roles)
                .ToList()
                .ForEach(u => Users.Add(new UserAdminItemViewModel(u)));
        }
    }
}