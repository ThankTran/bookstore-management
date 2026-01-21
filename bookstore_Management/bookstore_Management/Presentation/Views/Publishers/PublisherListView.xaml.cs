using bookstore_Management.Core.Enums;
using bookstore_Management.Models;
using bookstore_Management.Presentation.ViewModels;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using DocumentFormat.OpenXml.VariantTypes;
using System.Windows;
using System.Windows.Controls;


namespace bookstore_Management.Presentation.Views.Publishers
{
    /// <summary>
    /// Interaction logic for PublisherListView.xaml
    /// </summary>
    public partial class PublisherListView : UserControl
    {
        public PublisherListView(PublisherViewModel publisherViewModel)
        {
            InitializeComponent();
            this.DataContext = publisherViewModel;
        }

        private void dgPublishers_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
