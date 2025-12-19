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
using System.Xml.Linq;

namespace WPFMaster.Pages
{
    /// <summary>
    /// Логика взаимодействия для ModelAndEngineSelection.xaml
    /// </summary>
    public partial class ModelAndEngineSelection : Page
    {
        Configuration I;
        public ModelAndEngineSelection()
        {
            InitializeComponent();
            I = Configuration.Instance;
            ModelList.ItemsSource = Configuration.Instance.GetAvaliable("models");
            EngineList.ItemsSource = Configuration.Instance.GetAvaliable("engines");
            SumUp();
            if (I.Model != null) ModelTextChange(I.Model);
            if (I.EngineType != null) EngineTextChange(I.EngineType);
        }

        private void ModelList_Selection(object sender, SelectionChangedEventArgs e)
        {
            string item = (string)e.AddedItems[0];
            I.SetModel(item);
            ModelTextChange(item);
            SumUp();
        }

        private void EngineList_Selection(object sender, SelectionChangedEventArgs e)
        {
            string item = (string)e.AddedItems[0];
            I.SetEngineType(item);
            EngineTextChange(item);
            SumUp();
        }

        private void ModelTextChange(string name)
        {
            string price = I.GetPrice( "models", name ).ToString("N0");
            ModelText.Text = $"Модель: {name}\nЦена: {price}";
        }

        private void EngineTextChange(string name)
        {
            string price = I.GetPrice("engines", name).ToString("N0");
            EngineText.Text = $"Тюнинг: {name}\nЦена: {price}";
        }

        private void SumUp()
        {
            SumBox.Text = $"Итого: {I.CalculateTotalCost().ToString("N0")}";
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainMenuPage());
        }
    }
}
