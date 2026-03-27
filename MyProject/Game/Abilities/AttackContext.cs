using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Game.Abilities
{
    public class AttackContext
    {
        public int Damage { get; set; }
        public bool IgnoreArmor { get; set; }
        public bool SkipTurn { get; set; }
    }
}
