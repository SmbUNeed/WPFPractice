using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFMaster
{
    internal class Session
    {
        public static Session Instance;
        public Session()
        {
            if (Instance == null) Instance = this;
        }
        public static Users CurrentUser;
    }
}