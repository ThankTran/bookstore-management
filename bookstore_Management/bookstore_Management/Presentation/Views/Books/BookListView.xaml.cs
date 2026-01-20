using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using bookstore_Management.Presentation.ViewModels;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;

namespace bookstore_Management.Views.Books
{

    public partial class BookListView : UserControl
    {
        private BookViewModel _viewModel;

        public BookListView()
        {
            InitializeComponent();
            IBookService bookService = new BookService();
            _viewModel = new BookViewModel(bookService);
            this.DataContext = _viewModel;
        }
        #region Event Handlers
        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
        

        #endregion
    }
}