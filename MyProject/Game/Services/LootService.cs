using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyProject.Game.Items;

namespace MyProject.Game.Services
{
    internal static class LootService
    {
        public static IItem GetRandomItem(Random random) =>
            _items.ElementAt(random.Next(_items.Count()));

        private static IEnumerable<IItem> _items { get; } = new List<IItem> 
        { 
            new Armor("Кожаная куртка", 7),
            new Armor("Деревянная кираса", 9),
            new Armor("Древние латы", 12),
            new Armor("Рыцарские доспехи", 15),
            new Armor("Нагрудник героя", 20),
            
            new Weapon("Деревянный меч", 10),
            new Weapon("Арматура", 12),
            new Weapon("Кристальный резец", 14),
            new Weapon("Аганим", 17),
            new Weapon("Меч Короля Артура", 20),

            new HealthPotion(),
            new HealthPotion(),
            new HealthPotion(),
        };
    }
}
