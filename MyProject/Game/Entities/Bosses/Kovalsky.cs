using MyProject.Game.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Game.Entities.Bosses
{
    internal class Kovalsky : Boss
    {
        public Kovalsky()
        {
            Name = "Ковальский";
            Race = new Skeleton();

            MaxHp = (int)(Race.MaxHp * 2.5);
            Damage = (int)(Race.Damage * 1.3);
            Armor = (int)(Race.Armor * 1.4);

            Hp = MaxHp;

            Abilities = new ISpecialAbility[]
            {
                new ArmorPiercingAbility()
            };
        }
    }
}
