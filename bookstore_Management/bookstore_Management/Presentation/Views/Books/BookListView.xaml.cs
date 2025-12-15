using bookstore_Management.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using bookstore_Management.Presentation.Views.Dialogs.Books;

namespace bookstore_Management.Views.Books
{
    /// <summary>
    /// Interaction logic for BookListView.xaml
    /// </summary>
    public partial class BookListView : UserControl
    {
        public ObservableCollection<Book> Books { get; set; }

        public BookListView()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var owner = Window.GetWindow(this);
            var dialog = new InputBooksDialog();

            dialog.WindowState = WindowState.Normal;
            dialog.Width = 456;
            dialog.Height = 300;

            if (owner != null)
            {
                dialog.Owner = owner;
                UpdateLayout();
                var pos = PointToScreen(new Point(0, 0));
                double left = pos.X + (ActualWidth - dialog.Width) / 2;
                double top = pos.Y + (ActualHeight - dialog.Height) / 2;
                dialog.Left = Math.Round(left);
                dialog.Top = Math.Round(top);
            }

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                var newBook = new Book
                {
                    BookId = $"B{100 + Books.Count}",
                    Name = dialog.BookName,
                    SupplierId = dialog.SupplierId,
                    Category = dialog.Category,
                    ImportPrice = dialog.ImportPrice,
                    SalePrice = dialog.SalePrice,
                    CreatedDate = DateTime.Now
                };
                Books.Add(newBook);
            }
        }
    }
}
