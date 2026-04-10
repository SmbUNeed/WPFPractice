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
using System.Windows.Navigation;
using MyProject.Data;
using MyProject.Backend;

namespace MyProject.Pages
{
    public partial class AppointmentInfoPage : Page
    {
        Appointments _appointment;
        public AppointmentInfoPage(Appointments appointment)
        {
            InitializeComponent();
            _appointment = appointment;
            FillValues(appointment);
        }

        private void AppointButton_Click(object sender, RoutedEventArgs e)
        {
            if (!VisualModal.MessageIfFalse(PaymentMethodCombobox.SelectedItem != null, "Выберите способ оплаты")) return;
            int paymentMethodId = (PaymentMethodCombobox.SelectedItem as PaymentMethods).Id;
            try
            {
                string fb = FeedbackTextBox.Text;
                _appointment.PaymentMethodId = paymentMethodId;
                _appointment.Feedback = string.IsNullOrEmpty(fb) ? null : fb;
                    
                if (Auth.AppointClient(_appointment))
                {
                    MessageBox.Show("Запись успешно зарегистрирована");
                    NavigationService.Navigate(Access.HomePage());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void FillValues(Appointments appointment)
        {
            AppointmentNameTextBlock.Text = $"Услуга: {_appointment.Services.Name}";
            MasterTextBlock.Text = $"Мастер: {_appointment.Users1.Fullname}";
            DateTimeTextBlock.Text = $"Дата: {_appointment.DateTime}";
            PaymentMethodCombobox.ItemsSource = Core.Context.PaymentMethods.ToList();
        }
    }
}
