using MyProject.Game.Interfaces;
using MyProject.Game.Items;
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
        public void Equip(IEquipment equipment)
        {
            if (equipment is Weapon weapon)
            {
                CurrentWeapon = weapon;
            }
            else if (equipment is Armor armor)
            {
                CurrentArmor = armor;
            }
        }

        public void ApplyAttackContext(Abilities.AttackContext ctx)
        {
            int armorValue = ctx.IgnoreArmor ? 0 : (CurrentArmor?.ArmorNumber ?? 0);
            int damage = Math.Max(1, ctx.Damage - armorValue);
            IsFrozen = ctx.SkipTurn;
            Hp -= damage;
        }
    }
}
