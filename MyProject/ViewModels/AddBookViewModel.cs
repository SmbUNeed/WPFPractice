using Microsoft.Win32;
using MyProject.Core_;
using MyProject.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyProject.ViewModels
{
    public class AddBookViewModel : BaseViewModel
    {
        private readonly AuthorViewModel _parent;
        public string BookName { get; set; }
        public string CoverPath { get; set; }
        public ObservableCollection<ChapterDraft> Chapters { get; set; } = new ObservableCollection<ChapterDraft>();
        public ICommand PickCoverCommand { get; }
        public ICommand AddChapterCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddBookViewModel(AuthorViewModel parent)
        {
            _parent = parent;
            PickCoverCommand = new RelayCommand(_ => PickCover());
            AddChapterCommand = new RelayCommand(_ => AddChapter());
            SaveCommand = new RelayCommand(_ => Save());
            CancelCommand = new RelayCommand(_ => NavigationService.Navigate(_parent));
        }

        private void PickCover()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
                CoverPath = dialog.FileName;
        }

        private void AddChapter()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
                Chapters.Add(new ChapterDraft
                {
                    Number = Chapters.Count(),
                    Name = $"Глава {Chapters.Count + 1}",
                    FilePath = dialog.FileName
                });
        }

        private void Save()
        {
            string booksDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Books", BookName);
            Directory.CreateDirectory(booksDir);

            var book = new Books
            {
                Name = BookName,
                CoverPath = CoverPath,
                AuthorId = SessionService.CurrentUser.Id,
                IsFrozen = false
            };

            Core.Context.Books.Add(book);
            Core.Context.SaveChanges();

            foreach (var ch in Chapters)
            {
                string dest = Path.Combine(booksDir, Path.GetFileName(ch.FilePath));
                File.Copy(ch.FilePath, dest, true);
                Core.Context.Chapters.Add(new Database.Chapters
                {
                    BookId = book.Id,
                    Number = ch.Number,
                    Name = ch.Name,
                    Path = dest
                });
            }

            Core.Context.SaveChanges();
            NavigationService.Navigate(new HomeViewModel());
        }
    }
    public class ChapterDraft
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }
    }
}
