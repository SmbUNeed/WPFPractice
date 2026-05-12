using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Core_
{
    internal class NavigationService
    {
        public static Action<object> Navigate { get; set; }
    }
}
