using MyProject.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Core_
{
    public static class SessionService
    {
        public static Users CurrentUser { get; set; }
        public static bool LoggedIn => CurrentUser != null;
    }
}
