using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Game.Abilities
{
    public interface ISpecialAbility
    {
        string Description { get; }
        void Apply(AttackContext ctx, Random random);
    }
}
