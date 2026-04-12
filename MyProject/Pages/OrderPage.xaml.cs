using MyProject.Data;
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
using MyProject.Backend;

namespace MyProject.Pages
{
    /// <summary>
    /// Логика взаимодействия для OrderPage.xaml
    /// </summary>
    public partial class OrderPage : Page
    {
        public OrderPage()
        {
            InitializeComponent();
            FillValues();
        }

        private void FillValues()
        {
            PaymentMethodCombobox.ItemsSource = Core.Context.PaymentMethods.ToList();
            DateFilter.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-1)));
            DateFilter.BlackoutDates.Add(new CalendarDateRange(DateTime.Today.AddDays(7), DateTime.MaxValue));
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            PaymentMethods pm = PaymentMethodCombobox.SelectedItem as PaymentMethods;
            DateTime? date = DateFilter.SelectedDate;

            if (VisualModal.MessageIfFalse(pm != null, "Выберите способ оплаты") &&
                VisualModal.MessageIfFalse(date != null, "Выберите корректную дату")
                )
            {
                try
                {    Orders order = new Orders
                    {
                        UserId = Auth.CurrentUser.Id,
                        PaymentMethodId = pm.Id,
                        CreatedAt = DateTime.Today,
                        Status = "Deilvery",
                        DeliveryDate = (DateTime)date,
                    };
                    IEnumerable<Cart> cart = Core.Context.Cart.Where(c => c.UserId == Auth.CurrentUser.Id);
                    foreach(Cart c in cart)
                    {
                        OrdersProducts op = new OrdersProducts
                        {
                            ProductId = c.ProductId,
                            OrderId = order.Id,
                            Quantity = c.Quantity
                        };
                        Core.Context.OrdersProducts.Add(op);
                        Core.Context.Cart.Remove(cart.First(cr => cr.UserId == Auth.CurrentUser.Id && cr.ProductId == c.ProductId));
                    }
                    Core.Context.Orders.Add(order);
                    Core.Context.SaveChanges();
                    MessageBox.Show("Заказ принят!");
                    NavigationService.Navigate(Access.HomePage());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
