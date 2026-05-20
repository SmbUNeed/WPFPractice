using MyProject.Core_;
using MyProject.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MyProject.ViewModels
{
    public class ReadViewModel : BaseViewModel
    {
        private Books _book;
        private int _currentChapter;

        private string _chapterText;
        public string ChapterText { get => _chapterText; set { _chapterText = value; OnPropertyChanged(); } }

        private string _chapterTitle;
        public string ChapterTitle { get => _chapterTitle; set { _chapterTitle = value; OnPropertyChanged(); } }
        public string BookTitle { get; }

        private bool _nextAvailable;
        public bool NextAvailable
        {
            get => _nextAvailable;
            set { _nextAvailable = value; OnPropertyChanged(); }
        }

        private bool _prevAvailable;
        public bool PrevAvailable
        {
            get => _prevAvailable;
            set { _prevAvailable = value; OnPropertyChanged(); }
        }

        public ICommand NextChapter { get; }
        public ICommand PreviousChapter { get; }
        public ReadViewModel(Books book, int chapter=1)
        {
            _book = book;
            SetChapter(chapter);

            NextChapter = new RelayCommand(_ => ToNextChapter());
            PreviousChapter = new RelayCommand(_ => ToPreviousChapter());
        }

        private void SetChapter(int number)
        {
            _currentChapter = number;
            Chapters chapter = Core.Context.Chapters.First(c => c.BookId == _book.Id && c.Number == number);
            ChapterTitle = chapter.Name;

            StreamReader sr = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets\\chapters", _book.Id.ToString(), $"ch{_currentChapter}.txt"));
            ChapterText = sr.ReadToEnd();
            sr.Close();

            PrevAvailable = _currentChapter > 1;
            NextAvailable = Core.Context.Chapters.Any(ch => ch.Number == _currentChapter + 1 && ch.BookId == _book.Id);
        }

        private void ToNextChapter()
        {
            if (!NextAvailable)
            {
                NavigationService.GoBack();
                return;
            }
            SetChapter(_currentChapter + 1);
        }

        private void ToPreviousChapter()
        {
            if (!PrevAvailable) return;
            SetChapter(_currentChapter - 1);
        }
    }
}
