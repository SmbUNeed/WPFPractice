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

namespace WPFPr.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            CategoriesList.ItemsSource = Core.Context.parttype_.ToList();
        }

        private void CategoriesList_Selected(object sender, RoutedEventArgs e)
        {
            object si = CategoriesList.SelectedItem;
            Console.WriteLine(si);
            if (si == null) return;
            NavigationService.Navigate(new CategoryPage(si as parttype_));
        }
    }
}
