using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyProject.Game.Abilities;

namespace MyProject.Game.Entities.Bosses
{
    internal class Boss : Entity
    {
        public int Armor { get; protected set; }
        public int Damage { get; protected set; }
        public string Name { get; protected set; }
        public Enemy Race { get; protected set; }
        public IEnumerable<ISpecialAbility> Abilities { get; protected set; }
        
        public AttackContext ResolveAttack(Random random)
        {
            var ctx = new AttackContext();
            foreach(var ability in Abilities)
            {
                ability.Apply(ctx, random);
            }
            return ctx;
        }


    }
}
