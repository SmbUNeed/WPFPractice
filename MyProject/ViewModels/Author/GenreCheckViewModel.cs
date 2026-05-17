using MyProject.Core_;
using MyProject.Database;

namespace MyProject.ViewModels
{
    public class GenreCheckViewModel : BaseViewModel
    {
        public Genres Genre { get; }
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }
        public GenreCheckViewModel(Genres genre, bool isSelected = false)
        {
            Genre = genre;
            IsSelected = isSelected;
        }
    }
}