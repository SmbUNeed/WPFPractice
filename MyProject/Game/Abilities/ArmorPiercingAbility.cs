using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Game.Abilities
{
    public sealed class ArmorPiercingAbility : ISpecialAbility
    {
        public string Description => "Игнорирует броню цели.";
        public void Apply(AttackContext ctx, Random random) =>
            ctx.IgnoreArmor = true;
    }
}
