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
    /// Логика взаимодействия для CategoryPage.xaml
    /// </summary>
    public partial class CategoryPage : Page
    {
        List<basepart_> bps = new List<basepart_>();
        public CategoryPage(parttype_ prt)
        {
            InitializeComponent();
            bps = Core.Context.basepart_.ToList().Where(bsp => bsp.parttype_ == prt).ToList();
            ComponentList.ItemsSource = bps;
            CategoryName.Text = prt.name;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text;
            if (string.IsNullOrEmpty(searchText)) ComponentList.ItemsSource = bps;
            ComponentList.ItemsSource = bps.Where(bp => bp.name.Contains(searchText)).ToList();
        }
    }
}
