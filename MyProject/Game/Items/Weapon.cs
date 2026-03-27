using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyProject.Game.Abilities;
using MyProject.Game.Items;

namespace MyProject.Game
{
    internal class Weapon : IEquipment, IItem
    {
        public string Name { get; }
        public int BaseDamage { get; }
        public Weapon(string name, int baseDamage)
        {
            Name = name;
            BaseDamage = baseDamage;
        }
    }
}
