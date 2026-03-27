using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Game.Items
{
    internal interface IConsumable
    {
        void Use(Player player);
    }
}
