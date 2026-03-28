using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyProject.Game.Abilities;

namespace MyProject.Game
{
    public abstract class Enemy : Entity
    {
        public int Armor { get; set; }
        public int Damage { get; set; }
        public IEnumerable<ISpecialAbility> Abilities { get; protected set; }

        public Enemy(int maxHp, int armor, int damage)
        {
            MaxHp = maxHp;
            Hp    = maxHp;
            Armor = armor;
            Damage = damage;
        }

        public AttackContext ResolveAttack(Random random)
        {
            var ctx = new AttackContext { Damage = this.Damage };
            foreach (var ability in Abilities)
            {
                ability.Apply(ctx, random);
            }
            return ctx;
        }
    }
}
