using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Game.Entities.Bosses
{
    internal interface IBoss
    {
        Enemy Race { get; set; }
    }
}
