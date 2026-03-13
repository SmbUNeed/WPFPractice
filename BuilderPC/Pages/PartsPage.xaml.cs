using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BuilderPC.Data;

namespace BuilderPC.Pages
{
    public partial class PartsPage : Page
    {
        private parttype_ _currentType;
        private bool _loadingFilters;

        public PartsPage()
        {
            InitializeComponent();
            LoadTypes();
        }

        private void LoadTypes()
        {
            try
            {
                LstTypes.ItemsSource = Core.Context.parttype_.OrderBy(t => t.name).ToList();
                if (LstTypes.Items.Count > 0)
                    LstTypes.SelectedIndex = 0;
            }
            catch (Exception ex) { Error(ex); }
        }

        private void LstTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentType = LstTypes.SelectedItem as parttype_;
            if (_currentType == null) return;

            LoadManufacturers();
            LoadParts();
        }

        private void LoadManufacturers()
        {
            _loadingFilters = true;

            var mfrs = Core.Context.basepart_
                .Where(b => b.parttypeid == _currentType.id)
                .Select(b => b.manufacturer_)
                .Distinct()
                .OrderBy(m => m.name)
                .ToList();

            var all = new List<manufacturer_> { new manufacturer_ { id = 0, name = "Все производители" } };
            all.AddRange(mfrs);

            CmbManufacturer.ItemsSource   = all;
            CmbManufacturer.SelectedIndex = 0;

            _loadingFilters = false;
        }

        private void Filter_Changed(object sender, EventArgs e)
        {
            if (_loadingFilters || _currentType == null) return;
            LoadParts();
        }

        private void LoadParts()
        {
            if (_currentType == null) return;

            string search = TxtSearch.Text.Trim().ToLower();
            int mfrId = (CmbManufacturer.SelectedItem as manufacturer_)?.id ?? 0;

            try
            {
                var query = Core.Context.basepart_
                    .Where(b => b.parttypeid == _currentType.id);

                if (!string.IsNullOrEmpty(search))
                    query = query.Where(b => b.name.ToLower().Contains(search));

                if (mfrId != 0)
                    query = query.Where(b => b.manufacturerid == mfrId);

                var parts = query.OrderBy(b => b.name).ToList();

                LvParts.ItemsSource = parts;
            }
            catch (Exception ex) { Error(ex); }
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is basepart_ part)
            {
                BuildState.Set(part);

                var issues = BuildState.GetCompatibilityIssues();
                if (issues.Count > 0)
                {
                    string msg = "Добавлено, но есть проблемы совместимости:\n\n" + string.Join("\n", issues);
                    MessageBox.Show(msg, "Совместимость", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show($"«{part.name}» добавлено в сборку.", "Готово",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void Error(Exception ex) =>
            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
