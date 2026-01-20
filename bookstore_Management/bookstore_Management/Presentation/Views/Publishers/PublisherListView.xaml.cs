using System.Windows;
using System.Windows.Controls;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Presentation.ViewModels;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using DocumentFormat.OpenXml.VariantTypes;


namespace bookstore_Management.Presentation.Views.Publishers
{
    /// <summary>
    /// Interaction logic for PublisherListView.xaml
    /// </summary>
    public partial class PublisherListView : UserControl
    {
        private PublisherViewModel _viewModel;
        public PublisherListView()
        {
            InitializeComponent();
            var context = new BookstoreDbContext();
            var unitOfWork = new UnitOfWork(context);
            var publisherService = new PublisherService(unitOfWork);
            _viewModel = new PublisherViewModel(publisherService);
            this.DataContext = _viewModel;

            Loaded += PublisherListView_Loaded;
        }

        private void dgPublishers_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private async void PublisherListView_Loaded(object sender, RoutedEventArgs e)
        {
            await ((PublisherViewModel)DataContext).LoadPublishersFromDatabaseAsync();
        }
    }
}
