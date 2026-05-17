using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Core_
{
    public class NavigationService
    {
        private static Stack<object> _history = new Stack<object>();
        public static object Current { get; private set; }
        public static event Action<object> NavigationChanged;

        public static void Navigate(object viewModel)
        {
            if (Current != null)
                _history.Push(Current);

            Current = viewModel;
            NavigationChanged?.Invoke(Current);
        }

        public static void GoBack()
        {
            if (_history.Count == 0) return;
            Current = _history.Pop();
            NavigationChanged?.Invoke(Current);
        }

        public static bool CanGoBack() => _history.Count() > 0;
    }
}
