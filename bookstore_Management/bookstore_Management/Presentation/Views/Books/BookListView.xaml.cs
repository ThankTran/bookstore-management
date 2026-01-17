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
            // Lấy chỉ số dòng (bắt đầu từ 0) + 1
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    if (sender is Button button && button.DataContext is Book selectedBook)
            //    {
            //        // Execute the EditBookCommand from ViewModel
            //        if (_viewModel.EditBookCommand.CanExecute(selectedBook))
            //        {
            //            _viewModel.EditBookCommand.Execute(selectedBook);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Lỗi khi chỉnh sửa sách: {ex.Message}",
            //        "Lỗi",
            //        MessageBoxButton.OK,
            //        MessageBoxImage.Error);
            //}
        }


        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    if (sender is Button button && button.DataContext is Book selectedBook)
            //    {
            //        // Execute the RemoveBookCommand from ViewModel
            //        if (_viewModel.RemoveBookCommand.CanExecute(selectedBook))
            //        {
            //            _viewModel.RemoveBookCommand.Execute(selectedBook);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Lỗi khi xóa sách: {ex.Message}",
            //        "Lỗi",
            //        MessageBoxButton.OK,
            //        MessageBoxImage.Error);
            //}
        }

        //private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    //try
        //    //{
        //    //    string searchText = txtSearch.Text.ToLower().Trim();

        //    //    if (string.IsNullOrEmpty(searchText))
        //    //    {
        //    //        dgBooks.ItemsSource = _viewModel.Books;
        //    //    }
        //    //    else
        //    //    {
        //    //        var filteredBooks = _viewModel.Books.Where(b =>
        //    //            (b.bookID?.ToLower().Contains(searchText) ?? false) ||
        //    //            (b.name?.ToLower().Contains(searchText) ?? false) ||
        //    //            (b.author?.ToLower().Contains(searchText) ?? false) ||
        //    //            (b.publisher?.ToLower().Contains(searchText) ?? false)
        //    //        ).ToList();

        //    //        dgBooks.ItemsSource = filteredBooks;
        //    //    }
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}",
        //    //        "Lỗi",
        //    //        MessageBoxButton.OK,
        //    //        MessageBoxImage.Error);
        //    //}
        //}


        private void dgBooks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //    try
            //    {
            //        if (dgBooks.SelectedItem is Book selectedBook)
            //        {
            //            var detailMessage = $"Chi tiết sách:\n\n" +
            //                $"Mã sách: {selectedBook.bookID}\n" +
            //                $"Tên sách: {selectedBook.name}\n" +
            //                $"Tác giả: {selectedBook.author}\n" +
            //                $"Nhà xuất bản: {selectedBook.publisher}\n" +
            //                $"Thể loại: {selectedBook.category}\n" +
            //                $"Giá nhập: {selectedBook.importPrice:N0} VNĐ\n" +
            //                $"Giá bán: {selectedBook.salePrice:N0} VNĐ";

            //            MessageBox.Show(detailMessage,
            //                "Thông tin sách",
            //                MessageBoxButton.OK,
            //                MessageBoxImage.Information);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show($"Lỗi khi xem chi tiết: {ex.Message}",
            //            "Lỗi",
            //            MessageBoxButton.OK,
            //            MessageBoxImage.Error);
            //    }
        }
        #endregion

        //    #region Public Methods

        //    public void RefreshBookList()
        //    {
        //        try
        //        {
        //            dgBooks.Items.Refresh();
        //            txtSearch.Clear();
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show($"Lỗi khi làm mới danh sách: {ex.Message}",
        //                "Lỗi",
        //                MessageBoxButton.OK,
        //                MessageBoxImage.Error);
        //        }
        //    }
        //    #endregion
    }
}