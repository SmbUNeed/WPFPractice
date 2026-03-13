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
    /// <summary>
    /// Логика взаимодействия для FilmPage.xaml
    /// </summary>
    public partial class FilmPage : Page
    {
        Films CurrentFilm;

        public FilmPage(Films film)
        {
            InitializeComponent();
            CurrentFilm = film;
            SessionListBox.ItemsSource = Core.Context.Sessions.Where(s => s.IdFilm == film.Id).ToList().OrderBy(s => s.Halls.Name);
            DataContext = film;
        }

        private void SessionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (Session.CurrentUser == null)
                NavigationService.Navigate(new SignInPage());
            else 
                NavigationService.Navigate(new SelectPlacesPage((Sessions)e.AddedItems[0]));
        }
    }
}
