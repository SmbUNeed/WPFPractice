using MyProject.Game.Abilities;
using MyProject.Game.Entities.Bosses;
using MyProject.Game.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyProject.Game.Services
{
    internal class CombatService
    {
        private readonly Player _player;
        private readonly Random _random = new Random();
        private readonly EventGeneratorService _eventGeneratorService = new EventGeneratorService();
        private List<Enemy> _currentEnemies = new List<Enemy>();

        public Player Player => _player;

        public event Action<Enemy[]> CombatStarted;
        public event Action CombatCompleted;
        public event Action<IItem> ChestOpened;
        public event Action PlayerDied;
        public event Action<int> TurnChanged;
        public event Action<string> CombatLog;
        public event Action PlayerStatsChanged;

        public CombatService()
        {
            _player = new Player { MaxHp = 100, Hp = 100 };
            _player.Equip(new Armor("Рубашка", 20));
            _player.Equip(new Weapon("Палка", 30));
        }

        public void NextMove()
        {
            EventType eventType = _eventGeneratorService.NextTurn(_random);
            TurnChanged?.Invoke(_eventGeneratorService.CurrentTurn);
            switch (eventType)
            {
                case EventType.Enemy:
                    StartCombat(EnemyFactory.GetRandomEnemies(_random, 3).ToArray());
                    break;
                case EventType.Boss:
                    StartCombat(EnemyFactory.CreateRandomBoss(_random));
                    break;
                case EventType.Chest:
                    OpenChest();
                    break;
            }
        }

        public void StartCombat(params Enemy[] enemies)
        {
            _currentEnemies = enemies.ToList();
            CombatStarted?.Invoke(enemies);
        }

        public void OpenChest()
        {
            IItem item = LootService.GetRandomItem(_random);
            if (item is HealthPotion potion)
            {
                potion.Use(_player);
                CombatLog?.Invoke($"Вы нашли зелье и выпили его. HP: {_player.Hp}/{_player.MaxHp}");
                PlayerStatsChanged?.Invoke();
            }
            else
            {
                ChestOpened?.Invoke(item);
            }
        }

        public void PlayerAttack(Enemy target)
        {
            if (_player.IsFrozen)
            {
                _player.IsFrozen = false;
                CombatLog?.Invoke("Вы заморожены и пропускаете ход!");
                EnemyCounterAttack(false);
                return;
            }

            int dmg = Math.Max(1, _player.CurrentWeapon.BaseDamage - target.Armor);
            target.Hp -= dmg;
            CombatLog?.Invoke($"Вы атакуете {target.Name}: -{dmg} HP. (осталось {Math.Max(0, target.Hp)})");

            if (target.Hp <= 0)
            {
                _currentEnemies.Remove(target);
                CombatLog?.Invoke($"{target.Name} повержен!");
            }

            if (_currentEnemies.Count == 0)
            {
                CombatLog?.Invoke("Все враги побеждены!");
                CombatCompleted?.Invoke();
                return;
            }

            EnemyCounterAttack(false);
        }

        public void PlayerDefend()
        {
            if (_player.IsFrozen)
            {
                _player.IsFrozen = false;
                CombatLog?.Invoke("Вы заморожены и пропускаете ход!");
                EnemyCounterAttack(false);
                return;
            }

            CombatLog?.Invoke("Вы принимаете оборонительную стойку.");
            EnemyCounterAttack(true);
        }

        private void EnemyCounterAttack(bool defending)
        {
            foreach (var enemy in _currentEnemies.ToList())
            {
                if (_player.Hp <= 0) break;

                AttackContext ctx = enemy.ResolveAttack(_random);

                if (defending && _random.NextDouble() < 0.40)
                {
                    if (ctx.SkipTurn) _player.IsFrozen = true;
                    CombatLog?.Invoke($"Вы уклонились от атаки {enemy.Name}!");
                    continue;
                }

                int armorVal = ctx.IgnoreArmor ? 0 : (_player.CurrentArmor?.ArmorNumber ?? 0);
                int damage;

                if (defending)
                {
                    double blockPct = 0.70 + _random.NextDouble() * 0.30;
                    damage = Math.Max(0, ctx.Damage - (int)(armorVal * blockPct));
                }
                else
                {
                    damage = Math.Max(1, ctx.Damage - armorVal);
                }

                _player.IsFrozen = ctx.SkipTurn;
                _player.Hp = Math.Max(0, _player.Hp - damage);

                string freezeNote = ctx.SkipTurn ? " [Заморозка!]" : "";
                string ignoreNote = ctx.IgnoreArmor ? " [Игнор брони]" : "";
                CombatLog?.Invoke($"{enemy.Name} бьёт: -{damage} HP{ignoreNote}{freezeNote}. (HP: {_player.Hp}/{_player.MaxHp})");
            }

            PlayerStatsChanged?.Invoke();

            if (_player.Hp <= 0)
                PlayerDied?.Invoke();
        }
    }
}
