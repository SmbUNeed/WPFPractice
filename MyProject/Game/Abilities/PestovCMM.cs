using MyProject.Game.Entities.Bosses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyProject.Game.Abilities
{
    internal class PestovCMM : Boss
    {
        public PestovCMM()
        {
            Name = "Пестов C--";
            Race = new Skeleton();

            MaxHp = (int)(Race.MaxHp * 1.3);
            Damage = (int)(Race.Damage * 1.8);
            Armor = (int)(Race.Armor * 0.6);

            Hp = MaxHp;

            Abilities = new ISpecialAbility[]
            {
                new CriticalHitAbility(),
                new FreezeAbility(0.15)
            };
        }
    }
}
