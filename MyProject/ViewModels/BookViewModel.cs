using MyProject.Core_;
using MyProject.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyProject.ViewModels
{
    public class BookViewModel : BaseViewModel
    {
        private object _parent;
        public Books Book { get; }
        public ICommand ToCatalog { get; }
        public BookViewModel(object parent, Books book)
        {
            _parent = parent;
            Book = book;
            ToCatalog = new RelayCommand(_ => NavigationService.Navigate(_parent));
        }
    }
}
