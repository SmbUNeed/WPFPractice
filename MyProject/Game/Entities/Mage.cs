using MyProject.Game.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Game
{
    internal class Mage : Enemy
    {
        public Mage(int maxHp=25, int armor=2, int damage=15) : base(maxHp, armor, damage)
        {
            Name = "Маг";
            Abilities = new List<ISpecialAbility> { new FreezeAbility() };
        }
    }
}
