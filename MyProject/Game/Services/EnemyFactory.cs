using MyProject.Game.Abilities;
using MyProject.Game.Entities;
using MyProject.Game.Entities.Bosses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Game.Services
{
    internal static class EnemyFactory
    {
        public static List<Enemy> GetRandomEnemies(Random random, int maxValue)
        {
            List<Enemy> enemies = new List<Enemy>();
            
            enemies.Add(CreateRandomEnemy(random));
            maxValue--;
            for(int i = 0; i < maxValue; i++)
            {
                if (random.Next(3) == 0)
                {
                    enemies.Add(CreateRandomEnemy(random));
                }
            }
            return enemies;
        }
        public static Enemy CreateRandomEnemy(Random random)
        {
            switch (random.Next(3))
            {
                case 0: return new Skeleton();
                case 1: return new Mage();
                case 2: return new Goblin();
            }
            return new Skeleton();
        }

        public static Boss CreateRandomBoss(Random random)
        {
            switch (random.Next(4))
            {
                case 0: return new VVG();
                case 1: return new Kovalsky();
                case 2: return new ArchimageCPP();
                case 3: return new PestovCMM();
            }
            return new VVG();
        }
    }
}
