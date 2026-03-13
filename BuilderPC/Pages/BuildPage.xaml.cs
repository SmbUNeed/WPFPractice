using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BuilderPC.Windows;
using BuilderPC.Data;

namespace BuilderPC.Pages
{
    public partial class BuildPage : Page
    {
        public BuildPage()
        {
            InitializeComponent();
            BuildState.Changed += Refresh;
            Refresh();
        }

        private void Refresh()
        {
            var parts = BuildState.GetAll().ToList();
            LvBuild.ItemsSource = null;
            LvBuild.ItemsSource = parts;

            TxtTotal.Text = BuildState.GetTotal().ToString("N2") + " руб.";

            // Предупреждения совместимости
            List<string> issues = BuildState.GetCompatibilityIssues();
            if (issues.Count > 0)
            {
                LstWarnings.ItemsSource  = issues;
                LstWarnings.Visibility   = Visibility.Visible;
            }
            else
            {
                LstWarnings.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is int typeId)
            {
                var r = MessageBox.Show("Убрать из сборки?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (r == MessageBoxResult.Yes)
                    BuildState.Remove(typeId);
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            var r = MessageBox.Show("Очистить всю сборку?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (r == MessageBoxResult.Yes)
                BuildState.Clear();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!BuildState.GetAll().Any())
            {
                MessageBox.Show("Сборка пуста.", "Сохранение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var issues = BuildState.GetCompatibilityIssues();
            if (issues.Count > 0)
            {
                string msg = "Есть проблемы совместимости:\n\n" + string.Join("\n", issues)
                             + "\n\nВсё равно сохранить?";
                var r = MessageBox.Show(msg, "Совместимость",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (r != MessageBoxResult.Yes) return;
            }

            var win = new SaveBuildWindow { Owner = Window.GetWindow(this) };
            if (win.ShowDialog() == true)
            {
                try
                {
                    var asm = new assembly_
                    {
                        name   = win.AssemblyName,
                        author = win.AuthorName
                    };
                    Core.Context.assembly_.Add(asm);
                    Core.Context.SaveChanges();

                    foreach (int pid in BuildState.GetPartIds())
                        Core.Context.partassembly_.Add(new partassembly_ { assemblyid = asm.id, partid = pid });

                    Core.Context.SaveChanges();
                    MessageBox.Show("Сборка сохранена!", "Готово",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Ошибка сохранения: " + ex.Message, "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
