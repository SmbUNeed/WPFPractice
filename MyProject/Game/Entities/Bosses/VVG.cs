using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Game.Entities.Bosses
{
    internal class VVG : Enemy, IBoss
    {
        public Enemy Race { get; set; } = new Goblin();
        public VVG(int maxHp, int armor, int damage) : base(maxHp, armor, damage)
        {
            MaxHp = Race.MaxHp * 2;
            Hp = MaxHp;
            Armor = (int)(Race.Armor * 1.2);
            Damage = (int)(Race.Damage * 1.5);
        }
    }
}
