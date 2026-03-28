using MyProject.Game.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Game.Entities.Bosses
{
    internal class VVG : Boss
    {
        public VVG() 
        {
            Name = "ВВГ";
            Race = new Goblin();

            MaxHp = Race.MaxHp * 2;
            Damage = (int)(Race.Damage * 1.5);
            Armor = (int)(Race.Armor * 1.2);

            Hp = MaxHp;

            Abilities = new ISpecialAbility[]
            {
                new CriticalHitAbility(0.3)
            };
        }
    }
}
