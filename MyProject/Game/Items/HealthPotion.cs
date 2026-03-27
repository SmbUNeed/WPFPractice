using MyProject.Game.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Game
{
    internal class HealthPotion : IConsumable, IItem
    {
        public string Name { get; } = "Зелье исцеления";
        public void Use(Player player) =>
            player.Hp = player.MaxHp;
    }
}
