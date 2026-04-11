using MyProject.Data;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyProject.Pages
{
    public class AppointmentRow
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string ClientName { get; set; }
        public string MasterName { get; set; }
        public string ServiceName { get; set; }
        public string PaymentName { get; set; }
        public string Status { get; set; }
        public string Feedback { get; set; }
    }

    public class OrderRow
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string PaymentName { get; set; }
        public string Status { get; set; }
        public decimal Total { get; set; }
    }

    public class ProductRow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Sale { get; set; }
        public string TypeName { get; set; }
        public string ManufName { get; set; }
        public double Rate { get; set; }
        public bool IsFreezed { get; set; }
        public string FreezeText { get { return IsFreezed ? "Заморожен" : "Активен"; } }
    }

    public partial class ManagerPage : Page
    {
        private int? _editProductId;
        private int? _editManufId;
        private int? _editProdTypeId;
        private int? _editServiceId;

        public ManagerPage()
        {
            InitializeComponent();
            LoadAppointments();
            LoadOrders();
            LoadProducts();
            LoadManufacturers();
            LoadProductTypes();
            LoadServices();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void LoadAppointments(DateTime? date = null)
        {
            var q = Core.Context.Appointments
                        .Include("Users")
                        .Include("Users1")
                        .Include("Services")
                        .Include("PaymentMethods")
                        .AsQueryable();

            if (date.HasValue)
                q = q.Where(a => System.Data.Entity.DbFunctions.TruncateTime(a.DateTime) == date.Value.Date);

            if (AppointmentsGrid == null) return;

            var appointmentRows = q.ToList().Select(a => new AppointmentRow
            {
                Id = a.Id,
                DateTime = a.DateTime,
                ClientName = a.Users?.Fullname ?? "(не указан)",
                MasterName = a.Users1?.Fullname ?? "(не указан)",
                ServiceName = a.Services?.Name ?? "(не указана)",
                PaymentName = a.PaymentMethods?.Name ?? "(не указан)",
                Status = a.Status,
                Feedback = a.Feedback,
            }).ToList();

            AppointmentsGrid.ItemsSource = appointmentRows;
        }

        private void AppointmentDateFilter_Changed(object sender, SelectionChangedEventArgs e)
        {
            LoadAppointments(AppointmentDateFilter.SelectedDate);
        }

        private void ResetAppointmentFilter_Click(object sender, RoutedEventArgs e)
        {
            AppointmentDateFilter.SelectedDate = null;
            LoadAppointments();
        }

        private void OpenCreateAppointment_Click(object sender, RoutedEventArgs e)
        {
            var win = new CreateAppointmentWindow();
            if (win.ShowDialog() == true)
                LoadAppointments(AppointmentDateFilter.SelectedDate);
        }

        private void CancelAppointment_Click(object sender, RoutedEventArgs e)
        {
            var row = AppointmentsGrid.SelectedItem as AppointmentRow;
            if (row == null) return;
            if (row.Status == "Отменена") { MessageBox.Show("Запись уже отменена."); return; }

            var res = MessageBox.Show("Отменить запись?", "Подтверждение",
                                        MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res != MessageBoxResult.Yes) return;

            var a = Core.Context.Appointments.Find(row.Id);
            a.Status = "Отменена";
            Core.Context.SaveChanges();
            LoadAppointments(AppointmentDateFilter.SelectedDate);
        }

        private void RescheduleAppointment_Click(object sender, RoutedEventArgs e)
        {
            var row = AppointmentsGrid.SelectedItem as AppointmentRow;
            if (row == null) return;
            if (row.Status == "Отменена" || row.Status == "Выполнена")
            {
                MessageBox.Show("Нельзя перенести завершённую или отменённую запись.");
                return;
            }

            NewTime.Items.Clear();
            for (int h = 9; h <= 20; h++)
                foreach (int m in new[] { 0, 30 })
                    NewTime.Items.Add(new ComboBoxItem { Content = h.ToString("D2") + ":" + m.ToString("D2") });

            NewDate.SelectedDate = row.DateTime.Date;
            ReschedulePanel.Visibility = Visibility.Visible;
        }

        private void SaveReschedule_Click(object sender, RoutedEventArgs e)
        {
            var row = AppointmentsGrid.SelectedItem as AppointmentRow;
            if (row == null) return;
            if (NewDate.SelectedDate == null) { MessageBox.Show("Выберите дату."); return; }

            var timeItem = NewTime.SelectedItem as ComboBoxItem;
            if (timeItem == null) { MessageBox.Show("Выберите время."); return; }

            var parts = timeItem.Content.ToString().Split(':');
            var newDt = NewDate.SelectedDate.Value
                            .AddHours(int.Parse(parts[0]))
                            .AddMinutes(int.Parse(parts[1]));

            var a = Core.Context.Appointments.Find(row.Id);
            a.DateTime = newDt;
            Core.Context.SaveChanges();

            ReschedulePanel.Visibility = Visibility.Collapsed;
            LoadAppointments(AppointmentDateFilter.SelectedDate);
        }

        private void CloseReschedulePanel_Click(object sender, RoutedEventArgs e)
        {
            ReschedulePanel.Visibility = Visibility.Collapsed;
        }

        private void LoadOrders()
        {
            var orders = Core.Context.Orders
                                .Include("Users")
                                .Include("PaymentMethods")
                                .Include("OrdersProducts")
                                .Include("OrdersProducts.Products")
                                .ToList();

            var filterItem = OrderStatusFilter.SelectedItem as ComboBoxItem;
            var filter = filterItem != null ? filterItem.Content.ToString() : "Все";

            var rows = orders.Select(o => new OrderRow
            {
                Id = o.Id,
                ClientName = o.Users.Fullname,
                CreatedAt = o.CreatedAt,
                DeliveryDate = o.DeliveryDate,
                PaymentName = o.PaymentMethods.Name,
                Status = o.Status,
                Total = o.OrdersProducts.Sum(op =>
                                    op.Products.Price * (1 - (decimal)op.Products.Sale / 100) * op.Quantity),
            }).AsQueryable();

            if (filter == "Открытые") rows = rows.Where(o => o.Status != "Закрыт");
            else if (filter == "Закрытые") rows = rows.Where(o => o.Status == "Закрыт");

            if (rows.Count() == 0) return;
            OrdersGrid.ItemsSource = rows.ToList();
        }

        private void OrderStatusFilter_Changed(object sender, SelectionChangedEventArgs e)
        {
            LoadOrders();
        }

        private void CloseOrder_Click(object sender, RoutedEventArgs e)
        {
            var row = OrdersGrid.SelectedItem as OrderRow;
            if (row == null) return;
            if (row.Status == "Закрыт") { MessageBox.Show("Заказ уже закрыт."); return; }

            var res = MessageBox.Show("Закрыть заказ №" + row.Id + "?", "Подтверждение", MessageBoxButton.YesNo);
            if (res != MessageBoxResult.Yes) return;

            var o = Core.Context.Orders.Find(row.Id);
            o.Status = "Закрыт";
            Core.Context.SaveChanges();
            LoadOrders();
        }

        private void LoadProducts(string search = null)
        {
            var q = Core.Context.Products
                        .Include("ProductTypes")
                        .Include("Manufacturers")
                        .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(p => p.Name.Contains(search));

            ProductsGrid.ItemsSource = q.ToList().Select(p => new ProductRow
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Sale = (int)p.Sale,
                TypeName = p.ProductTypes.Name,
                ManufName = p.Manufacturers.Name,
                Rate = (int)p.Rate,
                IsFreezed = p.IsFreezed,
            }).ToList();
        }

        private void ProductSearch_Changed(object sender, TextChangedEventArgs e)
        {
            LoadProducts(ProductSearch.Text);
        }

        private void ProductsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var row = ProductsGrid.SelectedItem as ProductRow;
            if (row != null)
                BtnFreeze.Content = row.IsFreezed ? "Разморозить" : "Заморозить";
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            _editProductId = null;
            ProductEditTitle.Text = "Добавить товар";
            ProdName.Text = "";
            ProdPrice.Text = "";
            ProdSale.Text = "0";
            ProdDescription.Text = "";
            ProdTypeCombo.ItemsSource = Core.Context.ProductTypes.ToList();
            ProdManufCombo.ItemsSource = Core.Context.Manufacturers.ToList();
            ProdTypeCombo.SelectedIndex = -1;
            ProdManufCombo.SelectedIndex = -1;
            ProductEditPanel.Visibility = Visibility.Visible;
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            var row = ProductsGrid.SelectedItem as ProductRow;
            if (row == null) { MessageBox.Show("Выберите товар."); return; }

            _editProductId = row.Id;
            ProductEditTitle.Text = "Изменить товар";

            var p = Core.Context.Products.Find(row.Id);
            ProdName.Text = p.Name;
            ProdPrice.Text = p.Price.ToString();
            ProdSale.Text = p.Sale.ToString();
            ProdDescription.Text = p.Description;
            ProdTypeCombo.ItemsSource = Core.Context.ProductTypes.ToList();
            ProdManufCombo.ItemsSource = Core.Context.Manufacturers.ToList();
            ProdTypeCombo.SelectedValue = p.ProductTypeId;
            ProdManufCombo.SelectedValue = p.ManufacturerId;
            ProductEditPanel.Visibility = Visibility.Visible;
        }

        private void ToggleFreezeProduct_Click(object sender, RoutedEventArgs e)
        {
            var row = ProductsGrid.SelectedItem as ProductRow;
            if (row == null) return;

            var p = Core.Context.Products.Find(row.Id);
            p.IsFreezed = !p.IsFreezed;
            Core.Context.SaveChanges();
            LoadProducts(ProductSearch.Text);
        }

        private void SaveProduct_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProdName.Text)) { MessageBox.Show("Введите название."); return; }

            decimal price;
            if (!decimal.TryParse(ProdPrice.Text, out price) || price < 0)
            { MessageBox.Show("Некорректная цена."); return; }

            int sale;
            if (!int.TryParse(ProdSale.Text, out sale) || sale < 0 || sale > 100)
            { MessageBox.Show("Скидка от 0 до 100."); return; }

            if (ProdTypeCombo.SelectedValue == null) { MessageBox.Show("Выберите тип товара."); return; }
            if (ProdManufCombo.SelectedValue == null) { MessageBox.Show("Выберите производителя."); return; }

            if (_editProductId.HasValue)
            {
                var p = Core.Context.Products.Find(_editProductId.Value);
                p.Name = ProdName.Text.Trim();
                p.Price = price;
                p.Sale = sale;
                p.Description = ProdDescription.Text.Trim();
                p.ProductTypeId = (int)ProdTypeCombo.SelectedValue;
                p.ManufacturerId = (int)ProdManufCombo.SelectedValue;
            }
            else
            {
                Core.Context.Products.Add(new Products
                {
                    Name = ProdName.Text.Trim(),
                    Price = price,
                    Sale = sale,
                    Description = ProdDescription.Text.Trim(),
                    ProductTypeId = (int)ProdTypeCombo.SelectedValue,
                    ManufacturerId = (int)ProdManufCombo.SelectedValue,
                    IsFreezed = false,
                });
            }

            Core.Context.SaveChanges();
            ProductEditPanel.Visibility = Visibility.Collapsed;
            LoadProducts(ProductSearch.Text);
        }

        private void CloseProductPanel_Click(object sender, RoutedEventArgs e)
        {
            ProductEditPanel.Visibility = Visibility.Collapsed;
        }

        private void LoadManufacturers()
        {
            ManufacturersGrid.ItemsSource = Core.Context.Manufacturers.ToList();
        }

        private void AddManufacturer_Click(object sender, RoutedEventArgs e)
        {
            _editManufId = null;
            ManufEditTitle.Text = "Добавить производителя";
            ManufNameBox.Text = "";
            ManufEditPanel.Visibility = Visibility.Visible;
        }

        private void EditManufacturer_Click(object sender, RoutedEventArgs e)
        {
            var m = ManufacturersGrid.SelectedItem as Manufacturers;
            if (m == null) { MessageBox.Show("Выберите производителя."); return; }

            _editManufId = m.Id;
            ManufEditTitle.Text = "Изменить производителя";
            ManufNameBox.Text = m.Name;
            ManufEditPanel.Visibility = Visibility.Visible;
        }

        private void SaveManufacturer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ManufNameBox.Text)) { MessageBox.Show("Введите название."); return; }

            if (_editManufId.HasValue)
            {
                var m = Core.Context.Manufacturers.Find(_editManufId.Value);
                m.Name = ManufNameBox.Text.Trim();
            }
            else
            {
                Core.Context.Manufacturers.Add(new Manufacturers { Name = ManufNameBox.Text.Trim() });
            }

            Core.Context.SaveChanges();
            ManufEditPanel.Visibility = Visibility.Collapsed;
            LoadManufacturers();
        }

        private void CloseManufPanel_Click(object sender, RoutedEventArgs e)
        {
            ManufEditPanel.Visibility = Visibility.Collapsed;
        }

        private void LoadProductTypes()
        {
            ProductTypesGrid.ItemsSource = Core.Context.ProductTypes.ToList();
        }

        private void AddProductType_Click(object sender, RoutedEventArgs e)
        {
            _editProdTypeId = null;
            ProdTypeEditTitle.Text = "Добавить тип товара";
            ProdTypeNameBox.Text = "";
            ProdTypeEditPanel.Visibility = Visibility.Visible;
        }

        private void EditProductType_Click(object sender, RoutedEventArgs e)
        {
            var pt = ProductTypesGrid.SelectedItem as ProductTypes;
            if (pt == null) { MessageBox.Show("Выберите тип товара."); return; }

            _editProdTypeId = pt.Id;
            ProdTypeEditTitle.Text = "Изменить тип товара";
            ProdTypeNameBox.Text = pt.Name;
            ProdTypeEditPanel.Visibility = Visibility.Visible;
        }

        private void SaveProductType_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProdTypeNameBox.Text)) { MessageBox.Show("Введите название."); return; }

            if (_editProdTypeId.HasValue)
            {
                var pt = Core.Context.ProductTypes.Find(_editProdTypeId.Value);
                pt.Name = ProdTypeNameBox.Text.Trim();
            }
            else
            {
                Core.Context.ProductTypes.Add(new ProductTypes { Name = ProdTypeNameBox.Text.Trim() });
            }

            Core.Context.SaveChanges();
            ProdTypeEditPanel.Visibility = Visibility.Collapsed;
            LoadProductTypes();
        }

        private void CloseProdTypePanel_Click(object sender, RoutedEventArgs e)
        {
            ProdTypeEditPanel.Visibility = Visibility.Collapsed;
        }

        private void LoadServices()
        {
            ServicesGrid.ItemsSource = Core.Context.Services.ToList();
        }

        private void AddService_Click(object sender, RoutedEventArgs e)
        {
            _editServiceId = null;
            ServiceEditTitle.Text = "Добавить услугу";
            ServiceNameBox.Text = "";
            ServiceEditPanel.Visibility = Visibility.Visible;
        }

        private void EditService_Click(object sender, RoutedEventArgs e)
        {
            var svc = ServicesGrid.SelectedItem as Services;
            if (svc == null) { MessageBox.Show("Выберите услугу."); return; }

            _editServiceId = svc.Id;
            ServiceEditTitle.Text = "Изменить услугу";
            ServiceNameBox.Text = svc.Name;
            ServiceEditPanel.Visibility = Visibility.Visible;
        }

        private void SaveService_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ServiceNameBox.Text)) { MessageBox.Show("Введите название."); return; }

            if (_editServiceId.HasValue)
            {
                var svc = Core.Context.Services.Find(_editServiceId.Value);
                svc.Name = ServiceNameBox.Text.Trim();
            }
            else
            {
                Core.Context.Services.Add(new Services { Name = ServiceNameBox.Text.Trim() });
            }

            Core.Context.SaveChanges();
            ServiceEditPanel.Visibility = Visibility.Collapsed;
            LoadServices();
        }

        private void CloseServicePanel_Click(object sender, RoutedEventArgs e)
        {
            ServiceEditPanel.Visibility = Visibility.Collapsed;
        }
    }
}