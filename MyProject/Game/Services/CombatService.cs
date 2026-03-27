using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Game.Services
{
    internal class CombatService
    {
        private Player _player;
        private Random _random = new Random();
        public CombatService()
        {
            _player = new Player { MaxHp = 100, Hp = 100 };
            _player.Equip(new Armor("Рубашка", 2));
            _player.Equip(new Weapon("Палка", 2));
        }
        public void NextMove()
        {
            switch (_random.Next(2))
            {
                case 0:
                    OpenChest(); break;
                case 1:
                    Fight(); break;
            }
        }

        public void OpenChest()
        {

        }

        public void Fight()
        {

        }
    }
}
