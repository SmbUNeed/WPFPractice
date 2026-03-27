using MyProject.Game.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Game
{
    internal class Player : Entity, IFreezable
    {
        public bool IsFrozen { get; set; }
        public Weapon CurrentWeapon { get; private set; }
        public Armor CurrentArmor { get; private set; }
        public void Equip(Armor armor) =>
            CurrentArmor = armor;
        public void Equip(Weapon weapon) => 
            CurrentWeapon = weapon;
    }
}
