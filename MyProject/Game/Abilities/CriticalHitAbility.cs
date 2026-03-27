using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Game.Abilities
{
    public sealed class CriticalHitAbility : ISpecialAbility
    {
        private readonly double _chance;
        private readonly double _multiplier;

        public CriticalHitAbility(double chance = 0.20, double multiplier = 2)
        {
            _chance = chance;
            _multiplier = multiplier;
        }

        public string Description => $"Критический удар: шанс: {_chance * 100}%. {_multiplier}-кратный урон.";
        public void Apply(AttackContext ctx, Random random)
        {
            if (random.NextDouble() < _chance)
                ctx.Damage = (int)(ctx.Damage * _multiplier);
        }
    }
}
