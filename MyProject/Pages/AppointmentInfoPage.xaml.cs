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
        MastersTime _masterTime;
        Services _service;
        public AppointmentInfoPage(MastersTime mt, Services service)
        {
            InitializeComponent();
            FillValues(mt, service);
        }

        private void AppointButton_Click(object sender, RoutedEventArgs e)
        {
            int? paymentMethodId = (PaymentMethodCombobox.SelectedItem as PaymentMethods)?.Id;

            if ( VisualModal.MessageIfFalse(paymentMethodId == null, "Выберите способ оплаты"))
            {
                try
                {
                    Appointments appoinment = new Appointments
                    {
                        UserId = Auth.CurrentUser.Id,
                        MasterId = _masterTime.MasterId,
                        ServiceId = _service.Id,
                        Status = "Booked",
                        DateTime = _masterTime.DateTime,
                        CreatedAt = DateTime.Now,
                        Feedback = FeedbackTextBox.Text,
                    };
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                finally
                {
                    Core.Context.MastersTime.First(mt => mt.Id == _masterTime.Id).Status = "Booked";
                    Core.Context.SaveChanges();
                    NavigationService.Navigate(Access.HomePage());
                }
            }
        }

        private void FillValues(MastersTime mt, Services service)
        {
            _masterTime = mt;
            _service = service;
            AppointmentNameTextBlock.Text = $"Услуга: {service.Name}";
            MasterTextBlock.Text = $"Мастер: {mt.Users.Fullname}";
            DateTimeTextBlock.Text = $"Дата: {mt.DateTime}";
            PaymentMethodCombobox.ItemsSource = Core.Context.PaymentMethods;
        }
    }
}
