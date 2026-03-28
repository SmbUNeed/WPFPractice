using MyProject.Game.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Game
{
    internal class Skeleton : Enemy
    {
        public Skeleton(int maxHp=40, int armor=5, int damage=10) : base(maxHp, armor, damage)
        {
            Name = "Скелет";
            Abilities = new List<ISpecialAbility> { new ArmorPiercingAbility() };
        }
    }
}
