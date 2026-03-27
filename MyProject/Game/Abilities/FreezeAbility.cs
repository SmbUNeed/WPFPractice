using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Game.Abilities
{
    public sealed class FreezeAbility : ISpecialAbility
    {
        public readonly double _chance;

        public FreezeAbility(double chance = 0.15) =>
            _chance = chance;
        public string Description => $"Замораживает цель c шансом {_chance * 100}, пропуская её следующий ход.";

        public void Apply(AttackContext ctx, Random random)
        {
            if (random.NextDouble() < _chance)
                ctx.SkipTurn = true;
        }
    }
}
