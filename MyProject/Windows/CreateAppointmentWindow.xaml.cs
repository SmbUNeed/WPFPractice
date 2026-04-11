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
using System.Windows.Shapes;

namespace MyProject.Windows
{
    /// <summary>
    /// Логика взаимодействия для CreateAppointmentWindow.xaml
    /// </summary>
    public partial class CreateAppointmentWindow : Window
    {
        private int? _clientId;

        public CreateAppointmentWindow()
        {
            InitializeComponent();
            AppDate.DisplayDateStart = DateTime.Today;

            MasterCombo.ItemsSource = Core.Context.Users
                                            .Include("Services")
                                            .Where(u => u.Services.Any())
                                            .ToList();

            PaymentCombo.ItemsSource = Core.Context.PaymentMethods.ToList();
        }

        private void SearchClient_Click(object sender, RoutedEventArgs e)
        {
            var q = ClientSearch.Text.Trim();
            if (string.IsNullOrEmpty(q)) return;

            var results = Core.Context.Users
                                .Where(u => !u.IsFreezed
                                        && (u.Fullname.Contains(q) || u.PhoneNumber.Contains(q)))
                                .ToList();

            if (results.Count == 0)
            {
                MessageBox.Show("Клиенты не найдены.");
                ClientResults.Visibility = Visibility.Collapsed;
                return;
            }

            ClientResults.ItemsSource = results;
            ClientResults.Visibility = Visibility.Visible;
        }

        private void ClientResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var u = ClientResults.SelectedItem as Users;
            if (u == null) return;

            _clientId = u.Id;
            SelectedClientLabel.Text = u.Fullname + "  " + u.PhoneNumber;
            ClientResults.Visibility = Visibility.Collapsed;
        }

        private void MasterCombo_Changed(object sender, SelectionChangedEventArgs e)
        {
            ServiceCombo.ItemsSource = null;
            if (MasterCombo.SelectedValue == null) return;

            int masterId = (int)MasterCombo.SelectedValue;
            var master = Core.Context.Users
                                .Include("Services")
                                .FirstOrDefault(u => u.Id == masterId);

            if (master != null)
                ServiceCombo.ItemsSource = master.Services.ToList();
        }

        private void AppDate_Changed(object sender, SelectionChangedEventArgs e)
        {
            TimeCombo.Items.Clear();
            if (AppDate.SelectedDate == null) return;

            for (int h = 9; h <= 20; h++)
                foreach (int m in new[] { 0, 30 })
                    TimeCombo.Items.Add(new ComboBoxItem
                    {
                        Content = h.ToString("D2") + ":" + m.ToString("D2")
                    });
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (_clientId == null) { MessageBox.Show("Выберите клиента."); return; }
            if (MasterCombo.SelectedValue == null) { MessageBox.Show("Выберите мастера."); return; }
            if (ServiceCombo.SelectedValue == null) { MessageBox.Show("Выберите услугу."); return; }
            if (AppDate.SelectedDate == null) { MessageBox.Show("Выберите дату."); return; }

            var timeItem = TimeCombo.SelectedItem as ComboBoxItem;
            if (timeItem == null) { MessageBox.Show("Выберите время."); return; }

            if (PaymentCombo.SelectedValue == null) { MessageBox.Show("Выберите способ оплаты."); return; }

            var parts = timeItem.Content.ToString().Split(':');
            var dt = AppDate.SelectedDate.Value
                            .AddHours(int.Parse(parts[0]))
                            .AddMinutes(int.Parse(parts[1]));

            var confirm = MessageBox.Show(
                "Записать клиента на " + dt.ToString("dd.MM.yyyy HH:mm") + "?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            Core.Context.Appointments.Add(new Appointments
            {
                UserId = _clientId.Value,
                MasterId = (int)MasterCombo.SelectedValue,
                ServiceId = (int)ServiceCombo.SelectedValue,
                PaymentMethodId = (int)PaymentCombo.SelectedValue,
                DateTime = dt,
                Status = "Активна",
                CreatedAt = DateTime.Now,
            });

            Core.Context.SaveChanges();
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
