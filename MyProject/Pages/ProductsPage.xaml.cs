using MyProject.Data;
using MyProject.ViewModels;
using MyProject.Windows;
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

namespace MyProject.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        Manufacturers _manufacturerFilter;
        ProductTypes _productTypeFilter;
        IEnumerable<Products> Products => Core.Context.Products.OrderBy(p => p.Rate);
        public ProductsPage()
        {
            InitializeComponent();
            CartButton.Visibility = Backend.Auth.IsAuthenticated ? Visibility.Visible : Visibility.Hidden;
            ProductsListBox.ItemsSource = GetViewModels(Products);
        }

        List<ProductCartViewModel> GetViewModels(IEnumerable<Products> products) =>
    products.Select(p => new ProductCartViewModel(p)).ToList();

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterModal filterModal = new FilterModal();

            filterModal.ManufacturersComboBox.SelectedItem = Core.Context.Manufacturers.FirstOrDefault(m => m.Id == _manufacturerFilter.Id);
            filterModal.ProductTypesComboBox.SelectedItem = Core.Context.ProductTypes.FirstOrDefault(pt => pt.Id == _productTypeFilter.Id);
            if (filterModal.ShowDialog() == true)
            {
                _manufacturerFilter = filterModal.ManufacturersComboBox.SelectedItem as Manufacturers;
                _productTypeFilter = filterModal.ProductTypesComboBox.SelectedItem as ProductTypes;

                ProductsListBox.ItemsSource = GetViewModels(GetFiltered());
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ProductsListBox.ItemsSource = GetViewModels(GetFiltered()).Where(p => p.Product.Name.ToLower().Contains(SearchBox.Text.ToLower()));
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CartPage());
        }

        private List<Products> GetFiltered()
        {
            return Products.Where(p =>
                _manufacturerFilter == null ? true : p.ManufacturerId == _manufacturerFilter.Id &&
                _productTypeFilter == null ? true : p.ProductTypeId == _productTypeFilter.Id).ToList();
        }

        private void ProductsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductCartViewModel pcvm = e.AddedItems[0] as ProductCartViewModel;
            NavigationService.Navigate(new ProductInfoPage(pcvm));
        }
    }
}