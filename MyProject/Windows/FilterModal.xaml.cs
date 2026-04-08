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
using System.Windows.Shapes;
using MyProject.Data;

namespace MyProject.Windows
{
    /// <summary>
    /// Логика взаимодействия для FilterModal.xaml
    /// </summary>
    public partial class FilterModal : Window
    {
        public FilterModal()
        {
            InitializeComponent();
            ManufacturersComboBox.ItemsSource = Core.Context.Manufacturers.ToList();
            ProductTypesComboBox.ItemsSource = Core.Context.ProductTypes.ToList();
        }

        private void ApplyFilters(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        private void RestoreFilters(object sender, RoutedEventArgs e)
        {
            ManufacturersComboBox.SelectedItem = null;
            ProductTypesComboBox.SelectedItem = null;
        }
    }
}
