using Microsoft.Win32;
using MyProject.Core_;
using System.IO;
using System.Windows.Input;

namespace MyProject.ViewModels
{
    public class ChapterEditViewModel : BaseViewModel
    {
        public int? Id { get; set; }

        private int _number;
        public int Number
        {
            get => _number;
            set { _number = value; OnPropertyChanged(); }
        }

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private string _filePath;
        public string FilePath
        {
            get => _filePath;
            set { _filePath = value; OnPropertyChanged(); OnPropertyChanged(nameof(FileName)); }
        }

        public string FileName => string.IsNullOrEmpty(FilePath)
            ? "Файл не выбран"
            : Path.GetFileName(FilePath);

        public ICommand BrowseCommand { get; }

        public ChapterEditViewModel()
        {
            BrowseCommand = new RelayCommand(_ => Browse());
        }

        private void Browse()
        {
            var dialog = new OpenFileDialog { Filter = "Текстовые файлы|*.txt" };
            if (dialog.ShowDialog() == true)
                FilePath = dialog.FileName;
        }
    }
}