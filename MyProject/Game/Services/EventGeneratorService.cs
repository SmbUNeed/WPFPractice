using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Game.Services
{
    enum EventType
    {
        Enemy,
        Boss,
        Chest
    }
    internal class EventGeneratorService
    {

        public int CurrentTurn { get; private set; } = 0;
        public EventType NextTurn(Random random)
        {
            if (++CurrentTurn % 10 == 0)
            {
                return EventType.Boss;
            }
            if (random.Next(2) == 0) return EventType.Chest;
            else return EventType.Enemy;
        }
    }
}
