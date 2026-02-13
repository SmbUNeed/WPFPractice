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
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        List<Films> FilmsList = new List<Films>();
        public MainPage()
        {
            InitializeComponent();
            FilmsList = Core.Context.Films.OrderBy(f => f.Name).ToList();
            FilmsListBox.ItemsSource = FilmsList;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateFilms();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateFilms();
        }

        private void UpdateFilms()
        {
            if (FilmsListBox == null) return;
            FilmsList = Core.Context.Films.ToList();
            SearchFilms();
            OrderFilms();
            FilmsListBox.ItemsSource = FilmsList;
        }

        private void SearchFilms()
        {
            if (SearchBox != null && SearchBox?.Text != "")
            {
                FilmsList = FilmsList.Where(f => f.Name.ToLower().Contains(SearchBox.Text.ToLower())).ToList();
            }
        }

        private void OrderFilms()
        {
            switch(OrderComboBox.SelectedIndex)
            {
                case 0:
                    FilmsList = FilmsList.OrderBy(f => f.Name).ToList();
                    break;
                case 1:
                    FilmsList = FilmsList.OrderByDescending(f => f.RateFilm).ToList();
                    break;
            }
        }
    }
}
