using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Game.Services
{
    public class NavigationService
    {
        public static NavigationService Instance { get; } = new NavigationService();
        public event Action<object> NavigateTo;
        public void Navigate(object viewModel) => NavigateTo?.Invoke(viewModel);
    }
}
