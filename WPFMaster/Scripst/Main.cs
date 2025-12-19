using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFMaster
{
    internal class Main
    {
        public static Main Instance;

        public static void Initialize()
        {
            if (Instance == null)
            {
                Instance = new Main();
            }
        }

        public static Configuration CurrentConfiguration;
        
        public void NewConfiguration()
        {
            CurrentConfiguration = new Configuration();
        }
    }
}
