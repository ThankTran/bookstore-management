using System.Windows.Controls;
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
            IPublisherService publisherService;

            _viewModel = new PublisherViewModel();
            this.DataContext = _viewModel;
        }

        private void dgPublishers_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
