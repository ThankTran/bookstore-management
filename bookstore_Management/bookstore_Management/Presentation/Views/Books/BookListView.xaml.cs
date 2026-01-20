using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
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

            var context = new BookstoreDbContext();
            var unitOfWork = new UnitOfWork(context);
            var service = new BookService(unitOfWork);
            var publisherRepo = new PublisherRepository(context);

            DataContext = new BookViewModel(service, publisherRepo);
            
            this.Loaded += BookListView_Loaded;
        }
        #region Event Handlers
        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
        
        private async void BookListView_Loaded(object sender, RoutedEventArgs e)
        {
            await ((BookViewModel)DataContext).LoadBooksFromDatabase();
        }
        #endregion
    }
}