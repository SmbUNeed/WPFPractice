using System.Windows;
using BuilderPC.Pages;

namespace BuilderPC
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new PartsPage());
        }

        private void BtnParts_Click(object sender, RoutedEventArgs e) =>
            MainFrame.Navigate(new PartsPage());

        private void BtnBuild_Click(object sender, RoutedEventArgs e) =>
            MainFrame.Navigate(new BuildPage());

        private void BtnSaved_Click(object sender, RoutedEventArgs e) =>
            MainFrame.Navigate(new SavedBuildsPage());
    }
}
