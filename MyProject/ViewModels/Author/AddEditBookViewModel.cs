using Microsoft.Win32;
using MyProject.Core_;
using MyProject.Database;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MyProject.ViewModels
{
    public class AddEditBookViewModel : BaseViewModel
    {
        private readonly Books _book;
        private readonly Action _goBack;
        private readonly bool _isEdit;

        private static readonly string CoversDir =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Covers");
        private static readonly string AssetsDir =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");

        public string PageTitle => _isEdit ? "Редактировать книгу" : "Новая книга";

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        private string _coverPath;
        public string CoverPath
        {
            get => _coverPath;
            set { _coverPath = value; OnPropertyChanged(); }
        }

        public ObservableCollection<GenreCheckViewModel> Genres { get; } = new ObservableCollection<GenreCheckViewModel>();
        public ObservableCollection<ChapterEditViewModel> Chapters { get; } = new ObservableCollection<ChapterEditViewModel>();

        public ICommand BrowseCoverCommand { get; }
        public ICommand AddChapterCommand { get; }
        public ICommand RemoveChapterCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddEditBookViewModel(Books book, Action goBack)
        {
            _goBack = goBack;
            _isEdit = book != null;
            _book = book;

            // Жанры
            var allGenres = Core.Context.Genres.ToList();
            var bookGenreIds = _isEdit
                ? Core.Context.Books.Include(b => b.Genres)
                    .First(b => b.Id == book.Id).Genres
                    .Select(g => g.Id).ToHashSet()
                : new System.Collections.Generic.HashSet<int>();

            foreach (var g in allGenres)
                Genres.Add(new GenreCheckViewModel(g, bookGenreIds.Contains(g.Id)));

            if (_isEdit)
            {
                Title = book.Name;
                CoverPath = book.CoverPath;

                var chapters = Core.Context.Chapters
                    .Where(c => c.BookId == book.Id)
                    .OrderBy(c => c.Number)
                    .ToList();

                foreach (var c in chapters)
                    Chapters.Add(new ChapterEditViewModel
                    {
                        Id = c.Id,
                        Number = c.Number,
                        Name = c.Name,
                        FilePath = c.Path
                    });
            }

            BrowseCoverCommand = new RelayCommand(_ => BrowseCover());
            AddChapterCommand = new RelayCommand(_ => AddChapter());
            RemoveChapterCommand = new RelayCommand(ch => RemoveChapter((ChapterEditViewModel)ch));
            SaveCommand = new RelayCommand(_ => Save());
            CancelCommand = new RelayCommand(_ => _goBack());
        }

        private void BrowseCover()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp"
            };
            if (dialog.ShowDialog() != true) return;

            Directory.CreateDirectory(CoversDir);
            string dest = Path.Combine(CoversDir, Path.GetFileName(dialog.FileName));
            File.Copy(dialog.FileName, dest, overwrite: true);
            CoverPath = dest;
        }

        private void AddChapter()
        {
            Chapters.Add(new ChapterEditViewModel { Number = Chapters.Count + 1 });
        }

        private void RemoveChapter(ChapterEditViewModel chapter)
        {
            Chapters.Remove(chapter);
            for (int i = 0; i < Chapters.Count; i++)
                Chapters[i].Number = i + 1;
        }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                MessageBox.Show("Введите название книги");
                return;
            }
            if (Chapters.Any(c => string.IsNullOrWhiteSpace(c.FilePath)))
            {
                MessageBox.Show("Для каждой главы выберите файл");
                return;
            }

            Directory.CreateDirectory(CoversDir);

            Books bookEntity;

            if (_isEdit)
            {
                bookEntity = Core.Context.Books.Include(b => b.Genres).First(b => b.Id == _book.Id);
                bookEntity.Name = Title;
                bookEntity.CoverPath = CoverPath;
            }
            else
            {
                bookEntity = new Books
                {
                    Name = Title,
                    CoverPath = CoverPath,
                    AuthorId = SessionService.CurrentUser.Id,
                    IsFrozen = false
                };
                Core.Context.Books.Add(bookEntity);
                Core.Context.SaveChanges();
            }

            bookEntity.Genres.Clear();
            foreach (var g in Genres.Where(g => g.IsSelected))
                bookEntity.Genres.Add(Core.Context.Genres.Find(g.Genre.Id));

            if (_isEdit)
            {
                var keptIds = Chapters.Where(c => c.Id.HasValue).Select(c => c.Id.Value).ToList();
                var toDelete = Core.Context.Chapters
                    .Where(c => c.BookId == bookEntity.Id && !keptIds.Contains(c.Id))
                    .ToList();
                Core.Context.Chapters.RemoveRange(toDelete);
            }

            foreach (var ch in Chapters)
            {
                string destFile = ch.FilePath;

                if (!ch.FilePath.StartsWith(AssetsDir))
                {
                    // Создаём папку chapters\{bookId}\
                    string bookChaptersDir = Path.Combine(AssetsDir, "chapters", bookEntity.Id.ToString());
                    Directory.CreateDirectory(bookChaptersDir);

                    string fileName = $"ch{ch.Number}.txt";
                    string fullDest = Path.Combine(bookChaptersDir, fileName);
                    File.Copy(ch.FilePath, fullDest, overwrite: true);

                    // В БД храним относительный путь как у тебя
                    destFile = Path.Combine("chapters", bookEntity.Id.ToString(), fileName);
                }

                if (ch.Id.HasValue)
                {
                    var existing = Core.Context.Chapters.Find(ch.Id.Value);
                    existing.Name = ch.Name;
                    existing.Number = ch.Number;
                    existing.Path = destFile;
                }
                else
                {
                    Core.Context.Chapters.Add(new Chapters
                    {
                        BookId = bookEntity.Id,
                        Number = ch.Number,
                        Name = ch.Name,
                        Path = destFile
                    });
                }
            }

            Core.Context.SaveChanges();
            MessageBox.Show(_isEdit ? "Книга обновлена" : "Книга опубликована");
            _goBack();
        }
    }
}