using MyProject.Game.Items;
using MyProject.Game.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System;

namespace MyProject.Game.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        private readonly CombatService _combatService = new CombatService();

        public int PlayerHp => _combatService.Player.Hp;
        public int PlayerMaxHp => _combatService.Player.MaxHp;
        public string CurrentWeapon => _combatService.Player.CurrentWeapon?.Name ?? "—";
        public string CurrentArmor => _combatService.Player.CurrentArmor?.Name ?? "—";
        public int CurrentDamage => _combatService.Player.CurrentWeapon?.BaseDamage ?? 0;
        public int CurrentArmorNumber => _combatService.Player.CurrentArmor?.ArmorNumber ?? 0;
        public bool IsPlayerFrozen => _combatService.Player.IsFrozen;

        private int _currentFloor;
        public int CurrentFloor
        {
            get => _currentFloor;
            private set { _currentFloor = value; OnPropertyChanged(nameof(CurrentFloor)); }
        }

        private bool _isInCombat;
        public bool IsInCombat
        {
            get => _isInCombat;
            private set { _isInCombat = value; OnPropertyChanged(nameof(IsInCombat)); }
        }

        private bool _isChestPending;
        public bool IsChestPending
        {
            get => _isChestPending;
            private set { _isChestPending = value; OnPropertyChanged(nameof(IsChestPending)); }
        }

        private IItem _pendingItem;
        public IItem PendingItem
        {
            get => _pendingItem;
            private set { _pendingItem = value; OnPropertyChanged(nameof(PendingItem)); }
        }

        private Enemy _selectedEnemy;
        public Enemy SelectedEnemy
        {
            get => _selectedEnemy;
            set { _selectedEnemy = value; OnPropertyChanged(nameof(SelectedEnemy)); }
        }

        public ObservableCollection<Enemy> CurrentEnemies { get; } = new ObservableCollection<Enemy>();
        public ObservableCollection<string> EventLog { get; } = new ObservableCollection<string>();

        public ICommand NextMoveCommand { get; }
        public ICommand AttackEnemyCommand { get; }
        public ICommand DefendCommand { get; }
        public ICommand TakeItemCommand { get; }
        public ICommand DiscardItemCommand { get; }

        public GameViewModel()
        {
            _combatService.CombatStarted      += e   => Dispatch(() => OnCombatStarted(e));
            _combatService.CombatCompleted    += ()  => Dispatch(OnCombatCompleted);
            _combatService.ChestOpened        += i   => Dispatch(() => OnChestOpened(i));
            _combatService.PlayerDied         += ()  => Dispatch(OnPlayerDied);
            _combatService.TurnChanged        += f   => Dispatch(() => { CurrentFloor = f; });
            _combatService.CombatLog          += msg => Dispatch(() => Log(msg));
            _combatService.PlayerStatsChanged += ()  => Dispatch(RefreshPlayerStats);

            NextMoveCommand    = new RelayCommand(_ => _combatService.NextMove(),   _ => !IsInCombat && !IsChestPending);
            AttackEnemyCommand = new RelayCommand(param =>
            {
                if (param is Enemy enemy)
                    _combatService.PlayerAttack(enemy);
            }, _ => IsInCombat);
            DefendCommand      = new RelayCommand(_ => _combatService.PlayerDefend(), _ => IsInCombat);
            TakeItemCommand    = new RelayCommand(_ => TakeItem(),                    _ => IsChestPending);
            DiscardItemCommand = new RelayCommand(_ => DiscardItem(),                 _ => IsChestPending);
        }

        private void OnCombatStarted(Enemy[] enemies)
        {
            CurrentEnemies.Clear();
            foreach (var e in enemies) CurrentEnemies.Add(e);
            SelectedEnemy = CurrentEnemies.Count > 0 ? CurrentEnemies[0] : null;
            IsInCombat = true;
            Log($"⚔ Встреча: {string.Join(", ", enemies.Select(e => e.Name))}");
        }

        private void OnCombatCompleted()
        {
            CurrentEnemies.Clear();
            SelectedEnemy = null;
            IsInCombat = false;
            Log("✔ Все враги побеждены!");
        }

        private void OnChestOpened(IItem item)
        {
            PendingItem = item;
            IsChestPending = true;

            string stat = item is Armor a ? $"(защита: {a.ArmorNumber})"
                        : item is Weapon w ? $"(урон: {w.BaseDamage})"
                        : "";

            string current = item is Armor
                ? $"Текущий доспех: {_combatService.Player.CurrentArmor?.Name ?? "нет"} ({_combatService.Player.CurrentArmor?.ArmorNumber ?? 0})"
                : item is Weapon
                ? $"Текущее оружие: {_combatService.Player.CurrentWeapon?.Name ?? "нет"} ({_combatService.Player.CurrentWeapon?.BaseDamage ?? 0})"
                : "";

            Log($"🎁 Найден: {item.Name} {stat}");
            if (!string.IsNullOrEmpty(current)) Log(current);
        }

        private void TakeItem()
        {
            _combatService.Player.Equip((IEquipment)PendingItem);
            Log($"✔ Экипировано: {PendingItem.Name}");
            ClosePendingItem();
            RefreshPlayerStats();
        }

        private void DiscardItem()
        {
            Log($"✖ Выброшено: {PendingItem.Name}");
            ClosePendingItem();
        }

        private void ClosePendingItem()
        {
            PendingItem = null;
            IsChestPending = false;
        }

        private void OnPlayerDied()
        {
            Log("💀 Вы погибли!");
            NavigationService.Instance.Navigate(new GameOverViewModel());
        }

        private void RefreshPlayerStats()
        {
            OnPropertyChanged(nameof(PlayerHp));
            OnPropertyChanged(nameof(PlayerMaxHp));
            OnPropertyChanged(nameof(CurrentWeapon));
            OnPropertyChanged(nameof(CurrentArmor));
            OnPropertyChanged(nameof(CurrentDamage));
            OnPropertyChanged(nameof(CurrentArmorNumber));
            OnPropertyChanged(nameof(IsPlayerFrozen));

            // Удаляем мёртвых врагов из UI-коллекции (CombatService уже убрал их из своего списка)
            var dead = CurrentEnemies.Where(e => e.Hp <= 0).ToList();
            foreach (var d in dead) CurrentEnemies.Remove(d);

            if (SelectedEnemy == null || !CurrentEnemies.Contains(SelectedEnemy))
                SelectedEnemy = CurrentEnemies.FirstOrDefault();

            // Принудительно обновляем состояние всех команд
            Application.Current?.Dispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
        }

        private void Dispatch(Action action)
            => Application.Current?.Dispatcher.Invoke(action);

        private void Log(string msg) => EventLog.Insert(0, msg);

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
