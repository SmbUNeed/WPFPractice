using System.Windows;

namespace BuilderPC.Windows
{
    public partial class SaveBuildWindow : Window
    {
        public string AssemblyName { get; private set; }
        public string AuthorName   { get; private set; }

        public SaveBuildWindow()
        {
            InitializeComponent();
            TxtName.Focus();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string name   = TxtName.Text.Trim();
            string author = TxtAuthor.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(author))
            {
                TxtError.Text       = "Заполните оба поля.";
                TxtError.Visibility = Visibility.Visible;
                return;
            }

            AssemblyName = name;
            AuthorName   = author;
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
            => DialogResult = false;
    }
}
