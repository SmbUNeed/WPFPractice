using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class UserPage : Page
    {
        Users user;

        public UserPage()
        {
            InitializeComponent();
            user = Session.CurrentUser;
            InfoBlock.Text = $"{user.Login}\n{user.Name}\n{user.E_mail}";
            LoadTickets();
        }

        private void LoadTickets()
        {
            var db = Core.Context;

            var tickets = db.Tickets
                .Where(t => t.IdUser == user.Id)
                .Select(t => new
                {
                    Film = t.Sessions.Films.Name,
                    Hall = t.Sessions.Halls.Name,
                    DateTime = t.Sessions.DateTime,
                    Row = t.Places.Row,
                    Number = t.Places.Number,
                    Price = t.Sessions.Halls.Price
                })
                .ToList();

            TicketsList.ItemsSource = tickets.Select(t => new
            {
                Film = t.Film,
                Hall = $"Зал: {t.Hall}",
                DateTime = t.DateTime.ToString(),
                Seat = $"Ряд {t.Row}, Место {t.Number}",
                Price = $"{t.Price:N0} ₽"
            }).ToList();
        }
    }
}
