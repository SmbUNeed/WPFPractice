using MyProject.Game.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Game
{
    internal class Armor : IEquipment, IItem
    {
        public string Name { get; }
        public int ArmorNumber { get; }
        public Armor(string name, int armorNumber)
        {
            Name = name;
            ArmorNumber = armorNumber;
        }
    }
}
