using MyProject.Backend;
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
    /// Логика взаимодействия для CartPage.xaml
    /// </summary>
    public partial class CartPage : Page
    {
        public CartPage()
        {
            InitializeComponent();
            ProductsListBox.ItemsSource = GetViewModels(GetFiltered());
        }

        List<ProductCartViewModel> GetViewModels(IEnumerable<Products> products) =>
    products.Select(p => new ProductCartViewModel(p)).ToList();

        private IEnumerable<Products> GetFiltered()
        {
            List<Cart> cart = Core.Context.Cart.ToList();
            return cart.Where(c => c.UserId == Auth.CurrentUser.Id).Select(c => c.Products).OrderBy(p => p.Rate);
        }

        private void ProductsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductCartViewModel pcvm = e.AddedItems[0] as ProductCartViewModel;
            NavigationService.Navigate(new ProductInfoPage(pcvm));
        }
        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new OrderPage());
        }
    }
}
