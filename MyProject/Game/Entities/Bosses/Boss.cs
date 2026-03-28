using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyProject.Game.Abilities;

namespace MyProject.Game.Entities.Bosses
{
    internal class Boss : Enemy
    {
        public Enemy Race { get; protected set; }
        public Boss() : base(0, 0, 0)
        {

        }
    }
}
