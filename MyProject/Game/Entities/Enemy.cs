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
        public int Armor { get; }
        public int Damage { get; }
        
        public ISpecialAbility SpecialAbility;
    }
}
