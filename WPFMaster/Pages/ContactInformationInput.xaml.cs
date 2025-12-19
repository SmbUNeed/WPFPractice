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
    /// Логика взаимодействия для ContactInformationInput.xaml
    /// </summary>
    public partial class ContactInformationInput : Page
    {
        public ContactInformationInput()
        {
            InitializeComponent();
        }
        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            Configuration.Instance.SetContactInformation(new string[] { NameBox.Text, EmailBox.Text, PhoneBox.Text, AdressBox.Text });
            NavigationService.Navigate(new MainMenuPage());
        }
    }
}
