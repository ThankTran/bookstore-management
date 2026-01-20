using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.DTOs.ImportBill.Responses;
using bookstore_Management.DTOs.Order.Responses;
using bookstore_Management.Presentation.ViewModels;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using bookstore_Management.Presentation.Views.Dialogs.Invoices;
using bookstore_Management.Presentation.Views;

namespace bookstore_Management.Presentation.Views.Payment
{
    public partial class InvoiceView : UserControl, INotifyPropertyChanged, IDisposable
    {
        #region Fields & Services

        private readonly BookstoreDbContext _context;
        private readonly UnitOfWork _unitOfWork;
        private InvoiceFilterType _currentFilter = InvoiceFilterType.All;

        #endregion

        #region Constructor

        public InvoiceView()
        {
            InitializeComponent();
            // Lưu reference để dispose sau
            _context = new BookstoreDbContext();
            _unitOfWork = new UnitOfWork(_context);
            var importService = new ImportBillService(_unitOfWork);
            var orderService = new OrderService(_unitOfWork);
            DataContext = new InvoiceViewModel(importService, orderService);

            Loaded += InvoiceListView_Loaded;
        }

        #endregion

        #region Data Loading

        private async void InvoiceListView_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = (InvoiceViewModel)DataContext;
            await viewModel.LoadAllDataAsync();
            viewModel.DataLoaded += ViewModel_DataLoaded;
            UpdateFilterTabs();
        }

        private void ViewModel_DataLoaded()
        {
            UpdateStats();
            UpdateFilterTabs();
        }

        private void UpdateStats()
        {
            var viewModel = (InvoiceViewModel)DataContext;
            if (viewModel == null) return;

            var importCount = viewModel.FilteredInvoices?.Count(i => i.InvoiceType == InvoiceType.Import) ?? 0;
            var exportCount = viewModel.FilteredInvoices?.Count(i => i.InvoiceType == InvoiceType.Export) ?? 0;
            var totalCount = viewModel.FilteredInvoices?.Count ?? 0;

            tbImportCount.Text = importCount.ToString();
            tbExportCount.Text = exportCount.ToString();
            tbTotalCount.Text = totalCount.ToString();
            tbDisplayCount.Text = totalCount.ToString();
        }

        private void UpdateFilterTabs()
        {
            var viewModel = (InvoiceViewModel)DataContext;
            if (viewModel == null) return;

            // Reset all tabs
            tabAll.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
            tabImport.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
            tabExport.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));

            // Highlight active tab based on current filter
            switch (_currentFilter)
            {
                case InvoiceFilterType.All:
                    tabAll.Background = (System.Windows.Media.Brush)Application.Current.FindResource("PrimaryBrush");
                    break;
                case InvoiceFilterType.Import:
                    tabImport.Background = (System.Windows.Media.Brush)Application.Current.FindResource("PrimaryBrush");
                    break;
                case InvoiceFilterType.Export:
                    tabExport.Background = (System.Windows.Media.Brush)Application.Current.FindResource("PrimaryBrush");
                    break;
            }

            UpdateStats();
        }

        #endregion

        private async void BtnView_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is InvoiceDisplayItem item)
            {
                await OpenDetailViewAsync(item);
            }
        }
        
        private async Task OpenDetailViewAsync(InvoiceDisplayItem item)
        {
            if (item.InvoiceType == InvoiceType.Import)
            {
                var detailView = new ImportDetailView();
                await detailView.LoadImportBillAsync(item.InvoiceId);
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.MainFrame.Content = detailView;
                }
            }
            else
            {
                var detailView = new OrderDetailView();
                await detailView.LoadOrderAsync(item.InvoiceId);
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.MainFrame.Content = detailView;
                }
            }
        }
        
        private async void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgInvoices.SelectedItem is InvoiceDisplayItem item)
            {
                await OpenDetailViewAsync(item);
            }
        }

        private void TabAll_Click(object sender, RoutedEventArgs e)
        {
            _currentFilter = InvoiceFilterType.All;
            var viewModel = (InvoiceViewModel)DataContext;
            viewModel?.ApplyFilter(InvoiceFilterType.All);
            UpdateFilterTabs();
        }

        private void TabImport_Click(object sender, RoutedEventArgs e)
        {
            _currentFilter = InvoiceFilterType.Import;
            var viewModel = (InvoiceViewModel)DataContext;
            viewModel?.ApplyFilter(InvoiceFilterType.Import);
            UpdateFilterTabs();
        }

        private void TabExport_Click(object sender, RoutedEventArgs e)
        {
            _currentFilter = InvoiceFilterType.Export;
            var viewModel = (InvoiceViewModel)DataContext;
            viewModel?.ApplyFilter(InvoiceFilterType.Export);
            UpdateFilterTabs();
        }

        private void BtnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (InvoiceViewModel)DataContext;
            if (viewModel != null)
            {
                viewModel.SearchKeyword = string.Empty;
                txtSearch.Clear();
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #region IDisposable

        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    _unitOfWork?.Dispose();
                    _context?.Dispose();
                }
                _disposed = true;
            }
        }

        #endregion

        #region Supporting Classes

        public enum InvoiceType
        {
            Import,
            Export
        }

        public enum InvoiceFilterType
        {
            All,
            Import,
            Export
        }

        public class InvoiceDisplayItem : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            private int _stt;
            public int STT
            {
                get => _stt;
                set
                {
                    _stt = value;
                    OnPropertyChanged();
                }
            }

            public string InvoiceId { get; set; }
            public InvoiceType InvoiceType { get; set; }
            public string Partner { get; set; }
            public DateTime CreatedDate { get; set; }
            public decimal TotalAmount { get; set; }
            public string CreatedBy { get; set; }
            public string Notes { get; set; }

            // Display properties for binding
            public string TypeDisplay => InvoiceType == InvoiceType.Import ? "Nhập" : "Xuất";

            public string CreatedDateDisplay => CreatedDate.ToString("dd/MM/yyyy");

            public string TotalAmountDisplay => TotalAmount.ToString("N0") + " đ";
        }

        #endregion
    }
}