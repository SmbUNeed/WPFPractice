using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Game
{
    internal class Abilities
    {
        public class AttackContext
        {
            public int Damage { get; set; }
            public bool IgnoreArmor {  get; set; }
            public bool SkipTurn { get; set; }
        }

        public interface ISpecialAbility
        {
            string Description { get; }
            void Apply(AttackContext ctx, Random random);
        }

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

        public sealed class ArmorPiercingAbility : ISpecialAbility
        {
            public string Description => "Игнорирует броню цели.";
            public void Apply(AttackContext ctx, Random random) =>
                ctx.IgnoreArmor = true;
        }

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
}
