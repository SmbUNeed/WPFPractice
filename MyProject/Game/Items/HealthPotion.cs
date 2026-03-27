using MyProject.Game.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Game
{
    internal class HealthPotion : IConsumable
    {
        public void Use(Player player) =>
            player.Hp = player.MaxHp;
    }
}
