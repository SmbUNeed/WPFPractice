using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyProject.Game.ViewModels
{
    public class GameOverViewModel
    {
        public ICommand RestartCommand { get; }

        public GameOverViewModel()
        {
            RestartCommand = new RelayCommand(_ =>
                Services.NavigationService.Instance.Navigate(new GameViewModel()));
        }
    }
}
