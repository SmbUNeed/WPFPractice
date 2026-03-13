using System.Windows;

namespace BuilderPC.Windows;

public partial class SaveBuildWindow : Window
{
    public string BuildName { get; private set; } = "";
    public string AuthorName { get; private set; } = "";

    public SaveBuildWindow()
    {
        InitializeComponent();
        TxtBuildName.Focus();
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtBuildName.Text))
        {
            MessageBox.Show("Укажите название сборки.", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtBuildName.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(TxtAuthor.Text))
        {
            MessageBox.Show("Укажите имя автора.", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtAuthor.Focus();
            return;
        }

        BuildName = TxtBuildName.Text.Trim();
        AuthorName = TxtAuthor.Text.Trim();
        DialogResult = true;
        Close();
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
