using System.Windows;
using System.Windows.Controls;
using BuilderPC.Data;
using BuilderPC.Models;
using BuilderPC.Windows;

namespace BuilderPC.Pages;

public partial class BuilderPage : Page
{
    // Текущая сборка: TypeName -> Part
    private readonly Dictionary<string, Part> _build = new();
    // Список типов комплектующих из БД
    private List<PartType> _partTypes = new();

    public BuilderPage()
    {
        InitializeComponent();
        LoadPartTypes();
        RefreshUI();
    }

    private void LoadPartTypes()
    {
        try
        {
            _partTypes = DbHelper.GetPartTypes();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки типов: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // ─── Обновление отображения ──────────────────────────────────────────────────

    public void RefreshUI()
    {
        var rows = new List<ComponentRow>();

        foreach (var type in _partTypes)
        {
            _build.TryGetValue(type.Name, out Part? selected);
            rows.Add(new ComponentRow
            {
                TypeId = type.Id,
                TypeName = type.Name,
                PartName = selected?.Name ?? "— не выбрано —",
                Specs = selected?.SpecsSummary ?? "",
                Price = selected?.PriceDisplay ?? "",
                RemoveVisible = selected != null ? Visibility.Visible : Visibility.Collapsed
            });
        }

        ComponentsList.ItemsSource = rows;

        // Итоговая сумма
        decimal total = _build.Values.Sum(p => p.Price);
        TxtTotalPrice.Text = $"{total:N0} ₽";
        TxtPartsCount.Text = $"Компонентов выбрано: {_build.Count} / {_partTypes.Count}";

        // Совместимость
        UpdateCompatibility();
    }

    private void UpdateCompatibility()
    {
        if (_build.Count < 2)
        {
            WarningsList.ItemsSource = null;
            TxtCompatOk.Text = "Добавьте хотя бы 2 компонента для проверки";
            TxtCompatOk.Foreground = System.Windows.Media.Brushes.Gray;
            TxtCompatOk.Visibility = Visibility.Visible;
            return;
        }

        try
        {
            var warnings = DbHelper.CheckCompatibility(_build);
            if (warnings.Count == 0)
            {
                WarningsList.ItemsSource = null;
                TxtCompatOk.Text = "✓ Нет проблем с совместимостью";
                TxtCompatOk.Foreground = System.Windows.Media.Brushes.Green;
                TxtCompatOk.Visibility = Visibility.Visible;
            }
            else
            {
                TxtCompatOk.Visibility = Visibility.Collapsed;
                WarningsList.ItemsSource = warnings;
            }
        }
        catch (Exception ex)
        {
            TxtCompatOk.Text = $"Ошибка проверки: {ex.Message}";
            TxtCompatOk.Foreground = System.Windows.Media.Brushes.OrangeRed;
            TxtCompatOk.Visibility = Visibility.Visible;
        }
    }

    // ─── Обработчики кнопок ─────────────────────────────────────────────────────

    private void BtnSelectPart_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        int typeId = (int)btn.Tag;
        var type = _partTypes.FirstOrDefault(t => t.Id == typeId);
        if (type == null) return;

        var wnd = new PartsSelectWindow(typeId, type.Name);
        wnd.Owner = Window.GetWindow(this);
        if (wnd.ShowDialog() == true && wnd.SelectedPart != null)
        {
            _build[type.Name] = wnd.SelectedPart;
            RefreshUI();
        }
    }

    private void BtnRemovePart_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        int typeId = (int)btn.Tag;
        var type = _partTypes.FirstOrDefault(t => t.Id == typeId);
        if (type != null && _build.ContainsKey(type.Name))
        {
            _build.Remove(type.Name);
            RefreshUI();
        }
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        if (_build.Count == 0)
        {
            MessageBox.Show("Выберите хотя бы один компонент перед сохранением.",
                "Сборка пуста", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var dlg = new SaveBuildWindow();
        dlg.Owner = Window.GetWindow(this);
        if (dlg.ShowDialog() != true) return;

        try
        {
            DbHelper.SaveAssembly(dlg.BuildName, dlg.AuthorName,
                _build.Values.Select(p => p.Id).ToList());
            MessageBox.Show("Сборка успешно сохранена!", "Готово",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BtnClear_Click(object sender, RoutedEventArgs e)
    {
        if (_build.Count == 0) return;
        var result = MessageBox.Show("Очистить текущую сборку?", "Подтверждение",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            _build.Clear();
            RefreshUI();
        }
    }
}

// Вспомогательный класс для строки в ItemsControl
public class ComponentRow
{
    public int TypeId { get; set; }
    public string TypeName { get; set; } = "";
    public string PartName { get; set; } = "";
    public string Specs { get; set; } = "";
    public string Price { get; set; } = "";
    public Visibility RemoveVisible { get; set; }
}
