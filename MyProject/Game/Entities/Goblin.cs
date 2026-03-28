using MyProject.Game.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Game
{
    internal class Goblin : Enemy
    {
        public Goblin(int maxHp=30, int armor=3, int damage=12) : base(maxHp, armor, damage)
        {
            Name = "Гоблин";
            Abilities = new List<ISpecialAbility>{ new CriticalHitAbility() };
        }
    }
}
