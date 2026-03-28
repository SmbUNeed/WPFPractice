using MyProject.Game.Abilities;
using MyProject.Game.Entities.Bosses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyProject.Game.Entities.Bosses
{
    internal class ArchimageCPP : Boss
    {
        public ArchimageCPP()
        {
            Name = "Архимаг C++";
            Race = new Mage();

            MaxHp = (int)(Race.MaxHp * 1.8);
            Damage = (int)(Race.Damage * 1.6);
            Armor = (int)(Race.Armor * 1.1);

            Hp = MaxHp;

            Abilities = new ISpecialAbility[]
            {
                new FreezeAbility(0.25)
            };
        }
    }
}
