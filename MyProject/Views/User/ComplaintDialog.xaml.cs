using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyProject.Views
{
    /// <summary>
    /// Логика взаимодействия для ComplainDialog.xaml
    /// </summary>
    public partial class ComplaintDialog : Window
    {
        public string ComplaintText { get; set; }
        public ComplaintDialog()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ComplaintText = ComplaintTB.Text.Trim();
            DialogResult = true;
        }
    }
}
