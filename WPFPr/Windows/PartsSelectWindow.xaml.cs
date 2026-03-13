using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BuilderPC.Data;
using BuilderPC.Models;

namespace BuilderPC.Windows;

public partial class PartsSelectWindow : Window
{
    private readonly int _partTypeId;
    private readonly string _typeName;
    public Part? SelectedPart { get; private set; }

    public PartsSelectWindow(int partTypeId, string typeName)
    {
        InitializeComponent();
        _partTypeId = partTypeId;
        _typeName = typeName;
        TxtTitle.Text = $"Выбор: {typeName}";
        Title = $"Выбор: {typeName}";
        LoadManufacturers();
        LoadParts();
    }

    private void LoadManufacturers()
    {
        try
        {
            var manufacturers = DbHelper.GetManufacturers(_partTypeId);
            CmbManufacturer.ItemsSource = manufacturers;
            CmbManufacturer.SelectedIndex = 0;
        }
        catch { /* Пропускаем если нет подключения */ }
    }

    private void LoadParts()
    {
        try
        {
            string? search = TxtSearch.Text.Trim().Length > 0 ? TxtSearch.Text.Trim() : null;
            int? mfrId = (CmbManufacturer.SelectedItem as Manufacturer)?.Id;

            var parts = DbHelper.GetParts(_partTypeId, search, mfrId);
            PartsGrid.ItemsSource = parts;
            TxtFoundCount.Text = $"Найдено: {parts.Count}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки комплектующих: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Поиск в реальном времени
        LoadParts();
    }

    private void CmbManufacturer_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        LoadParts();
    }

    private void BtnSearch_Click(object sender, RoutedEventArgs e) => LoadParts();

    private void PartsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        ConfirmSelection();
    }

    private void BtnConfirm_Click(object sender, RoutedEventArgs e) => ConfirmSelection();

    private void ConfirmSelection()
    {
        if (PartsGrid.SelectedItem is Part part)
        {
            SelectedPart = part;
            DialogResult = true;
            Close();
        }
        else
        {
            MessageBox.Show("Выберите компонент из списка.", "Ничего не выбрано",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
