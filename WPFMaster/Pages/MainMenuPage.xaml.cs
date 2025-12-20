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

namespace WPFMaster.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainMenuPage.xaml
    /// </summary>
    public partial class MainMenuPage : Page
    {
        Configuration I;
        public MainMenuPage()
        {
            InitializeComponent();
            I = Configuration.Instance;
            SetInfo();
        }

        private void SetInfo()
        {
            if (I.Model != null && I.EngineType != null)
            {
                ME_TxtB.Text = $"Модель {I.Model}\nДвигатель: {I.EngineType}";
                SelectModel.Content = "Изменить";
            }
            if (I.BodyColor != null || I._selectedOptions != null)
            {
                string s = "\n";
                foreach(string o in I._selectedOptions)
                {
                    s += o + "\n";
                }
                // CO_TxtB = 
                SelectColorAndOptions.Content = "Изменить";
            }
            if (I.CheckContactIfFull())
            {
                string[] s = I.GetContactsInfo();
                // И тд.
                ContactsButton.Content = "Перезаполнить";
            }
            // Расчет кредита?
        }

        private void ModelSelection(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ModelAndEngineSelection());
        }
        private void NewColorAndOptions(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ColorAndOptionsSelection());
        }
        private void NewContactInformation(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ContactInformationInput());
        }
        private void CreditCalculation(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CreditCalculation());
        }
    }
}
