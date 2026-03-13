using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFMaster.Pages
{
    /// <summary>
    /// Логика взаимодействия для SelectPlacesPage.xaml
    /// </summary>
    public partial class SelectPlacesPage : Page
    {
        public Sessions CurSession;
        public SelectPlacesPage(Sessions session)
        {
            InitializeComponent();
            
            CurSession = session;
            LoadSeats(session.Id);
        }

        private readonly List<SeatVM> _selectedSeats = new List<SeatVM>();

        private void LoadSeats(int sessionId)
        {
            var db = Core.Context;

            var occupiedIds = db.Tickets
                .Where(t => t.IdSession == sessionId)
                .Select(t => t.IdPlace)
                .ToHashSet();

            var seats = db.Places
                .Where(s => s.IdHall == CurSession.IdHall)
                .OrderBy(s => s.Row).ThenBy(s => s.Number)
                .ToList();

            var rows = seats.GroupBy(s => s.Row).OrderBy(g => g.Key)
                .Select(g => new RowVM
                {
                    RowNumber = g.Key,
                    Seats = g.Select(s => CreateSeatVM(s.Id, s.Row, s.Number, occupiedIds.Contains(s.Id))).ToList()
                }).ToList();

            SeatsGrid.ItemsSource = rows;
        }

        private SeatVM CreateSeatVM(int id, int row, int number, bool occupied)
        {
            var vm = new SeatVM { SeatId = id, RowNumber = row, SeatNumber = number, IsOccupied = occupied };
            vm.ClickCommand = new RelayCommand(_ => ToggleSeat(vm));
            vm.Refresh();
            return vm;
        }

        private void ToggleSeat(SeatVM seat)
        {
            if (seat.IsOccupied) return;

            if (seat.IsSelected)
            {
                seat.IsSelected = false;
                _selectedSeats.Remove(seat);
            }
            else
            {
                seat.IsSelected = true;
                _selectedSeats.Add(seat);
            }

            seat.Refresh();
            RefreshSummary();
        }

        private void RefreshSummary()
        {
            int count = _selectedSeats.Count;
            TxtSelected.Text = $"Выбрано: {count} мест | Итого: {count * CurSession.Halls.Price:N0} ₽";
            BtnBook.IsEnabled = count > 0;
        }

        private void ChkHideOccupied_Changed(object sender, RoutedEventArgs e)
        {
            bool hide = ChkHideOccupied.IsChecked == true;
            foreach (var row in (List<RowVM>)SeatsGrid.ItemsSource)
                foreach (var seat in row.Seats)
                {
                    if (seat.IsOccupied)
                        seat.Visibility = hide ? Visibility.Collapsed : Visibility.Visible;
                }
        }

        private void BtnBook_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TicketConfirmationPage(CurSession, _selectedSeats));
        }

        public class RowVM
        {
            public int RowNumber { get; set; }
            public List<SeatVM> Seats { get; set; } = new List<SeatVM>();
        }

        public class SeatVM : INotifyPropertyChanged
        {
            public int SeatId { get; set; }
            public int RowNumber { get; set; }
            public int SeatNumber { get; set; }
            public bool IsOccupied { get; set; }
            public bool IsSelected { get; set; }
            public ICommand ClickCommand { get; set; }

            private Brush _bg = Brushes.LightBlue;
            public Brush Background { get => _bg; set { _bg = value; OnPropertyChanged(); } }

            private Brush _fg = Brushes.Black;
            public Brush Foreground { get => _fg; set { _fg = value; OnPropertyChanged(); } }

            private bool _enabled = true;
            public bool IsEnabled { get => _enabled; set { _enabled = value; OnPropertyChanged(); } }

            private Visibility _visibility = Visibility.Visible;
            public Visibility Visibility { get => _visibility; set { _visibility = value; OnPropertyChanged(); } }

            private string _tooltip = "";
            public string ToolTip { get => _tooltip; set { _tooltip = value; OnPropertyChanged(); } }

            public void Refresh()
            {
                if (IsOccupied)
                {
                    Background = Brushes.Gray;
                    Foreground = Brushes.DarkGray;
                    IsEnabled = false;
                    ToolTip = $"Ряд {RowNumber}, Место {SeatNumber} — Занято";
                }
                else if (IsSelected)
                {
                    Background = Brushes.Orange;
                    Foreground = Brushes.White;
                    ToolTip = $"Ряд {RowNumber}, Место {SeatNumber} — Выбрано";
                }
                else
                {
                    Background = Brushes.LightBlue;
                    Foreground = Brushes.Black;
                    ToolTip = $"Ряд {RowNumber}, Место {SeatNumber}";
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string p = null)
                => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        public class RelayCommand : ICommand
        {
            private readonly Action<object> _execute;
            public RelayCommand(Action<object> execute) => _execute = execute;
            public bool CanExecute(object p) => true;
            public void Execute(object p) => _execute(p);
            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }
        }
    }
}
