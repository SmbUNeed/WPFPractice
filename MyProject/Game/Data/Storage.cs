using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyProject.Game.Items;

namespace MyProject.Game.Data
{
    internal class Storage
    {
        public IItem GetRandomItem(Random random) =>
            Items.ElementAt(random.Next(Items.Count()));

        public IEnumerable<IItem> Items { get; } = new List<IItem> 
        { 
            new Armor("Деревянная кираса", 5),
            new Armor("Кожаная куртка", 2),
            new Armor("Древние латы", 7),
            new Armor("Рыцарские доспехи", 10),
            new Armor("Нагрудник героя", 15),
            
            new Weapon("Деревянный меч", 5),
            new Weapon("Арматура", 7),
            new Weapon("Кристальный резец", 10),
            new Weapon("Аганим", 15),
            new Weapon("Меч Короля Артура", 17),

            new HealthPotion(),
            new HealthPotion(),
            new HealthPotion(),
        };
    }
}
