using System.ComponentModel;

namespace MyProject.Game
{
    public abstract class Entity : INotifyPropertyChanged
    {
        public int MaxHp { get; set; }

        private int _hp;
        public int Hp
        {
            get => _hp;
            set { _hp = value; OnPropertyChanged(nameof(Hp)); }
        }

        public int SkipTurn { get; set; }
        public string Name { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
