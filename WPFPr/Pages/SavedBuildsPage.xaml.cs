using System.Windows;
using System.Windows.Controls;
using BuilderPC.Data;
using BuilderPC.Models;

namespace BuilderPC.Pages;

public partial class SavedBuildsPage : Page
{
    private List<SavedBuild> _builds = new();

    public SavedBuildsPage()
    {
        InitializeComponent();
        LoadBuilds();
    }

    private void LoadBuilds()
    {
        try
        {
            _builds = DbHelper.GetAllAssemblies();
            BuildsList.ItemsSource = _builds;
            if (_builds.Count == 0)
            {
                TxtBuildTitle.Text = "Сохранённых сборок нет";
                TxtBuildAuthor.Text = "";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки сборок: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BuildsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (BuildsList.SelectedItem is not SavedBuild build) return;

        TxtBuildTitle.Text = build.Name;
        TxtBuildAuthor.Text = $"Автор: {build.Author}";
        PartsList.ItemsSource = build.Parts;
        TxtDetailTotal.Text = build.TotalPriceDisplay;
    }

    private void BtnRefresh_Click(object sender, RoutedEventArgs e) => LoadBuilds();

    private void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (BuildsList.SelectedItem is not SavedBuild build) return;

        var result = MessageBox.Show(
            $"Удалить сборку «{build.Name}»?",
            "Подтверждение удаления",
            MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            DbHelper.DeleteAssembly(build.Id);
            LoadBuilds();
            PartsList.ItemsSource = null;
            TxtBuildTitle.Text = "Выберите сборку";
            TxtBuildAuthor.Text = "";
            TxtDetailTotal.Text = "—";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
