using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BuilderPC.Data;
namespace BuilderPC.Pages
{
    public partial class SavedBuildsPage : Page
    {
        public SavedBuildsPage()
        {
            InitializeComponent();
            LoadAssemblies();
        }

        private void LoadAssemblies()
        {
            try
            {
                LstAssemblies.ItemsSource = Core.Context.assembly_
                    .OrderByDescending(a => a.id)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LstAssemblies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstAssemblies.SelectedItem is assembly_ asm)
            {
                TxtHint.Visibility   = Visibility.Collapsed;
                PnlDetail.Visibility = Visibility.Visible;

                TxtName.Text   = asm.name;
                TxtAuthor.Text = "Автор: " + asm.author;

                // Загружаем детали комплектующих этой сборки
                var partIds = Core.Context.partassembly_
                    .Where(pa => pa.assemblyid == asm.id)
                    .Select(pa => pa.partid)
                    .ToList();

                var parts = Core.Context.basepart_
                    .Where(b => partIds.Contains(b.id))
                    .OrderBy(b => b.parttype_.name)
                    .ToList();

                DgParts.ItemsSource = parts;

                decimal total = parts.Sum(p => (decimal)p.price);
                TxtTotal.Text = "Стоимость: " + total.ToString("N2") + " руб.";
            }
            else
            {
                TxtHint.Visibility   = Visibility.Visible;
                PnlDetail.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            Core.Context = new PCBuilderEntities();   // обновляем контекст
            LoadAssemblies();
            TxtHint.Visibility   = Visibility.Visible;
            PnlDetail.Visibility = Visibility.Collapsed;
        }
    }
}
