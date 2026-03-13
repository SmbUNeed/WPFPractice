using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WPFMaster;
using static WPFMaster.Pages.SelectPlacesPage;

namespace WPFMaster.Pages
{
    public partial class TicketConfirmationPage : Page
    {
        private readonly Sessions _session;
        private readonly List<SeatVM> _seats;

        public TicketConfirmationPage(Sessions session, List<SeatVM> seats)
        {
            InitializeComponent();
            _session = session;
            _seats = seats;

            // Подгружаем Halls и Films если не загружены
            if (_session.Halls == null)
                Core.Context.Entry(_session).Reference(s => s.Halls).Load();
            if (_session.Films == null)
                Core.Context.Entry(_session).Reference(s => s.Films).Load();

            FillInfo();
        }

        private void FillInfo()
        {
            TxtFilm.Text = _session.Films.Name;
            TxtHall.Text = _session.Halls.Name;
            TxtDateTime.Text = _session.DateTime.ToString();
            TxtPrice.Text = $"{_session.Halls.Price:N0} ₽";

            SeatsList.ItemsSource = _seats
                .Select(s => $"Ряд {s.RowNumber}, Место {s.SeatNumber} — {_session.Halls.Price:N0} ₽")
                .ToList();

            decimal total = _seats.Count * _session.Halls.Price ?? 0;
            TxtTotal.Text = $"{total:N0} ₽";
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            var db = Core.Context;

            foreach (var seat in _seats)
            {
                db.Tickets.Add(new Tickets
                {
                    IdSession = _session.Id,
                    IdPlace = seat.SeatId,
                    IdUser = Session.CurrentUser.Id
                });
            }

            db.SaveChanges();

            MessageBox.Show("Билеты успешно оформлены!", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);

            // Возврат на главную
            var frame = NavigationService;
            while (frame.CanGoBack)
                frame.RemoveBackEntry();

            NavigationService.Navigate(new MainPage());
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}