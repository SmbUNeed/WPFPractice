using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyProject.Game.Abilities;

namespace MyProject.Game
{
    internal abstract class Enemy : Entity
    {
        public int Armor;
        public int Damage;
        public ISpecialAbility SpecialAbility;

        public Enemy(int maxHp, int armor, int damage)
        {
            MaxHp = maxHp;
            Armor = armor;
            Damage = damage;
        }

        public AttackContext ResolveAttack(Random random)
        {
            var ctx = new AttackContext();
            SpecialAbility.Apply(ctx, random);
            return ctx;
        }
    }
}
