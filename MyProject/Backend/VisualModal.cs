using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyProject.Backend
{
    public static class VisualModal
    {
        public static bool MessageIfFalse(bool condition, string message, string title="Предупреждение", MessageBoxImage image=MessageBoxImage.Error)
        {
            if (!condition)
            {
                MessageBox.Show(message, title, MessageBoxButton.OK, image);
                return false;
            }
            return true;
        }
    }
}
