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
        public MainPage()
        {
            InitializeComponent();
            FilmsListBox.ItemsSource = Core.Context.Films.OrderBy(f => f.Name).ToList();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox) == null) return;
            switch ((sender as ComboBox).SelectedIndex)
            {
                case 0:
                    FilmsListBox.ItemsSource = Core.Context.Films.OrderBy(f => f.Name).ToList();
                    break;
                case 1:
                    FilmsListBox.ItemsSource = Core.Context.Films.OrderBy(f => f.RateFilm).ToList();
                    break;
            }
        }
    }
}
