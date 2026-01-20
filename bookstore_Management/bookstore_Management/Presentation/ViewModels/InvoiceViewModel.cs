using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Models;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.DTOs.ImportBill.Requests;
using bookstore_Management.DTOs.ImportBill.Responses;
using bookstore_Management.DTOs.Order.Responses;
using bookstore_Management.Presentation.Views.Dialogs.Invoices;
using bookstore_Management.Presentation.Views.Payment;
using CommunityToolkit.Mvvm.Input;
using NUnit.Framework.Internal.Execution;
using RelayCommand = bookstore_Management.Presentation.Views.Payment.RelayCommand;
using bookstore_Management.Presentation.Views;


namespace bookstore_Management.Presentation.ViewModels
{
    internal class InvoiceViewModel : BaseViewModel
    {
        #region khai báo
        private readonly IOrderService _orderService;
        private readonly IImportBillService _importBillService;
        public IOrderService OrderService => _orderService;
        public IImportBillService ImportBillService => _importBillService;
        
        public event Action DataLoaded;
        public ObservableCollection<InvoiceView.InvoiceDisplayItem> AllInvoices { get; set; }
        public ObservableCollection<InvoiceView.InvoiceDisplayItem> FilteredInvoices { get; set; }
        private ObservableCollection<ImportBillResponseDto> _imports;
        public ObservableCollection<ImportBillResponseDto> Imports
        {
            get { return _imports; }
            set
            {
                _imports = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<OrderResponseDto> _orders;
        public ObservableCollection<OrderResponseDto> Orders
        {
            get { return _orders; }
            set
            {
                _orders = value;
                OnPropertyChanged();
            }
        }

        private InvoiceView.InvoiceDisplayItem  _selectedInvoice;
        public InvoiceView.InvoiceDisplayItem SelectedInvoice
        {
            get => _selectedInvoice;
            set
            {
                _selectedInvoice = value;
                OnPropertyChanged();
            }
        }

        private string _searchKeyword;
        private CancellationTokenSource _searchCancellationTokenSource;
        private const int SEARCH_DEBOUNCE_MS = 500;

        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                _searchKeyword = value;
                OnPropertyChanged();
                
                // Debounce search để tránh query quá nhiều
                _searchCancellationTokenSource?.Cancel();
                _searchCancellationTokenSource = new CancellationTokenSource();
                var token = _searchCancellationTokenSource.Token;
                
                _ = Task.Delay(SEARCH_DEBOUNCE_MS, token).ContinueWith(async t =>
                {
                    if (!t.IsCanceled && !token.IsCancellationRequested)
                    {
                        await Application.Current.Dispatcher.InvokeAsync(async () =>
                        {
                            await SearchInvoiceCommandAsync();
                        });
                    }
                }, TaskScheduler.Default);
            }
        }
        #endregion

        #region khai báo command
        //khai báo command cho thao tác thêm, xóa, sửa hóa đơn
        public ICommand AddOrderCommand { get; set; }
        public ICommand AddImportCommand { get; set; }
        public ICommand RemoveInvoiceCommand { get; set; }
        public ICommand EditInvoiceCommand { get; set; }

        //command cho thao tác tìm kiếm - load lại
        public ICommand SearchInvoiceCommand { get; set; }
        public ICommand AllInvoicesCommand { get; set; }
        public ICommand ExportInvoiceCommand { get; set; }
        public ICommand OrderInvoiceCommand { get; set; }

        //command cho in / xuất excel
        public ICommand ExportCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        
        // command cho các details
        public ICommand DetailCommand { get; set; }
        #endregion

        #region Load data from db
        private async Task LoadOrdersFromDatabase()
        {
            var result = await _orderService.GetAllOrdersAsync();
            if(!result.IsSuccess)
            {
                MessageBox.Show("Lỗi tải đơn hàng: " + result.ErrorMessage);
                return;
            }
            if(result.Data == null) return; 

            Orders = new ObservableCollection<OrderResponseDto>(result.Data);
        }

        private async Task LoadImportsFromDatabase()
        {
            var result = await _importBillService.GetAllImportBillsAsync();
            if (!result.IsSuccess)
            {
                MessageBox.Show("Lỗi tải hóa đơn nhập: " + result.ErrorMessage);
                return;
            }
            if (result.Data == null) return;
            
            Imports = new ObservableCollection<ImportBillResponseDto>(result.Data);
        }
        
        private void BuildInvoiceList()
        {
            var list = new List<InvoiceView.InvoiceDisplayItem>();

            if (Imports != null)
                list.AddRange(Imports.Select(MapImportBillToDisplay));

            if (Orders != null)
                list.AddRange(Orders.Select(MapOrderToDisplay));

            var i = 1;
            foreach (var item in list.OrderByDescending(x => x.CreatedDate))
            {
                item.STT = i++;
            }
            AllInvoices = new ObservableCollection<InvoiceView.InvoiceDisplayItem>(list);

        }
        public async Task LoadAllDataAsync()
        {
            await LoadOrdersFromDatabase();
            await LoadImportsFromDatabase();
            BuildInvoiceList();
            ApplyFilter(InvoiceView.InvoiceFilterType.All);
            DataLoaded?.Invoke();
        }

        private async Task ExportInvoiceCommandAsync()
        {
            await LoadAllDataAsync();
            ApplyFilter(InvoiceView.InvoiceFilterType.Import);
        }

        private async Task OrderInvoiceCommandAsync()
        {
            await LoadAllDataAsync();
            ApplyFilter(InvoiceView.InvoiceFilterType.Export);
        }
        #endregion

        #region constructor
        public InvoiceViewModel(IImportBillService importBillService, IOrderService orderService)
        {
            
            
            _importBillService = importBillService;
            _orderService = orderService;

            Imports = new ObservableCollection<ImportBillResponseDto>();
            Orders = new ObservableCollection<OrderResponseDto>();
            FilteredInvoices = new ObservableCollection<InvoiceView.InvoiceDisplayItem>();
            
            AllInvoicesCommand = new AsyncRelayCommand(LoadAllDataAsync);
            ExportInvoiceCommand = new AsyncRelayCommand(ExportInvoiceCommandAsync);
            OrderInvoiceCommand = new AsyncRelayCommand(OrderInvoiceCommandAsync);
            AddImportCommand = new AsyncRelayCommand(AddImportCommandAsync);
            AddOrderCommand = new AsyncRelayCommand(AddOrderCommandAsync);
            EditInvoiceCommand = new RelayCommand(EditInvoiceCommandAsync);
            RemoveInvoiceCommand = new AsyncRelayCommand(RemoveCommandAsync);
            SearchInvoiceCommand = new AsyncRelayCommand(SearchInvoiceCommandAsync);
            PrintCommand = new AsyncRelayCommand(PrintCommandAsync);
            ExportCommand = new AsyncRelayCommand(ExportCommandAsync);
            DetailCommand = new RelayCommand<InvoiceView.InvoiceDisplayItem>((item) => 
            {
                _ = DetailCommandAsync(item);
            });

        }
        
        
        
        #endregion
        
        #region AddImportCommand + AddOrderComand

        private async Task AddImportCommandAsync()
        {
            var dialog = new Views.Dialogs.Invoices.CreateImportBill();
            if(dialog.ShowDialog() == true)
            {
                var dto = dialog.GetImportBillData();

                var result = await _importBillService.CreateImportBillAsync(dto);
                if (!result.IsSuccess)
                {
                    MessageBox.Show("Lỗi khi thêm hóa đơn nhập");
                    return;
                }

                await LoadAllDataAsync();
            }
        }

        private async Task AddOrderCommandAsync()
        {
            var dialog = new Views.Dialogs.Invoices.CreateOrderBill();
            if (dialog.ShowDialog() == true)
            {
                var dto = dialog.GetOrderData();
                var result = await _orderService.CreateOrderAsync(dto);
                if (!result.IsSuccess)
                {
                    MessageBox.Show("Lỗi khi thêm hóa đơn bán");
                }
                await LoadAllDataAsync();
            }
        }
        #endregion
        
        #region EditCommand
        
        private void EditImportCommand(string importId)
        {
            if (SelectedInvoice == null) return;

            var dialog = new EditImportBillDialog(SelectedInvoice.InvoiceId);
            dialog.ShowDialog();
        }

        private void EditOrderCommand(string orderId)
        {
            if (SelectedInvoice == null) return;

            var dialog = new EditImportBillDialog(SelectedInvoice.InvoiceId);
            dialog.ShowDialog();
        }

        private void EditInvoiceCommandAsync()
        {
            switch (SelectedInvoice.InvoiceType)
            {
                case InvoiceView.InvoiceType.Export:
                    EditOrderCommand(SelectedInvoice.InvoiceId);
                    break;
                case InvoiceView.InvoiceType.Import:
                    EditImportCommand(SelectedInvoice.InvoiceId);
                    break;
                default:
                    return;
            }
        }
        #endregion
        
        #region RemoveCommand
        
        private async Task RemoveCommandAsync()
        {
            var inv = SelectedInvoice;
            if (inv == null)
            {
                MessageBox.Show("Vui lòng chọn khách hàng để xóa.");
                return;
            }
            var confirmed = Views.Dialogs.Share.Delete.ShowForInvoice(inv.InvoiceId);
            if (!confirmed) return;

            Result result;
            if (inv.InvoiceType.Equals(InvoiceView.InvoiceType.Export))
            {
                result = await _importBillService.DeleteImportBillAsync(inv.InvoiceId);
            }
            else
            {
                result = await _orderService.DeleteOrderAsync(inv.InvoiceId);
            }
            if (!result.IsSuccess)
            {
                MessageBox.Show($"Không thể xóa khách hàng.\nChi tiết lỗi: {result.ErrorMessage}",
                    "Lỗi xóa dữ liệu",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            await LoadAllDataAsync();
        }
        #endregion

        #region SearchCommand

        private async Task SearchInvoiceCommandAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword))
            {
                BuildInvoiceList();
                ApplyFilter(InvoiceView.InvoiceFilterType.All);
                return;
            }
        
            // Tìm kiếm import
            var importResult = await _importBillService.SearchImportBillsAsync(SearchKeyword);
        
            // Tìm kiếm order
            var orderResult = await _orderService.SearchOrderBillsAsync(SearchKeyword);
        
            if (!importResult.IsSuccess || !orderResult.IsSuccess)
            {
                MessageBox.Show("Lỗi khi tìm kiếm hóa đơn!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        
            var list = new List<InvoiceView.InvoiceDisplayItem>();
        
            // Map import
            if (importResult.Data != null)
                list.AddRange(importResult.Data.Select(MapImportBillToDisplay));
        
            // Map order
            if (orderResult.Data != null)
                list.AddRange(orderResult.Data.Select(MapOrderToDisplay));
        
            // Sắp xếp + gán số thứ tự
            int i = 1;
            foreach (var item in list.OrderByDescending(x => x.CreatedDate))
            {
                item.STT = i++;
            }
        
            FilteredInvoices.Clear();
            foreach (var item in list)
                FilteredInvoices.Add(item);
        }

        #endregion
        
        #region Export + Print Command

        private async Task ExportCommandAsync()
        {
            
        }

        private async Task PrintCommandAsync()
        {
            
        }
        #endregion
        
        #region Mapping
        private InvoiceView.InvoiceDisplayItem MapImportBillToDisplay(ImportBillResponseDto import)
        {
            return new InvoiceView.InvoiceDisplayItem
            {
                InvoiceId = import.Id,
                InvoiceType = InvoiceView.InvoiceType.Import,
                Partner = import.PublisherName ?? "N/A",
                CreatedDate = import.CreatedDate,
                TotalAmount = import.TotalAmount,
                CreatedBy = import.CreatedBy ?? "System",
                Notes = import.Notes
            };
        }
        
        private InvoiceView.InvoiceDisplayItem MapOrderToDisplay(OrderResponseDto order)
        {
            return new InvoiceView.InvoiceDisplayItem
            {
                InvoiceId = order.OrderId,
                InvoiceType = InvoiceView.InvoiceType.Export,
                Partner = order.CustomerName ?? "Khách vãng lai",
                CreatedDate = order.CreatedDate,
                TotalAmount = order.TotalPrice,
                CreatedBy = order.StaffName ?? "N/A",
                Notes = order.Notes
            };
        }

        #endregion

        #region Filter
        public void ApplyFilter(InvoiceView.InvoiceFilterType filterType)
        {
            IEnumerable<InvoiceView.InvoiceDisplayItem> source = AllInvoices;

            switch (filterType)
            {
                case InvoiceView.InvoiceFilterType.Import:
                    source = source.Where(x => x.InvoiceType == InvoiceView.InvoiceType.Import);
                    break;

                case InvoiceView.InvoiceFilterType.Export:
                    source = source.Where(x => x.InvoiceType == InvoiceView.InvoiceType.Export);
                    break;
            }

            FilteredInvoices.Clear();
            foreach (var item in source)
                FilteredInvoices.Add(item);
        }
        #endregion

        #region DetailView

        private async Task DetailCommandAsync(InvoiceView.InvoiceDisplayItem invoiceSelected)
        {
            if (invoiceSelected == null) return;
            if (invoiceSelected.InvoiceType.Equals(InvoiceView.InvoiceType.Import))
            {
                var window = new ImportDetailView();
                await window.LoadImportBillAsync(invoiceSelected.InvoiceId);
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.MainFrame.Content = window;
                }
            }
            else
            {
                var window = new OrderDetailView();
                await window.LoadOrderAsync(invoiceSelected.InvoiceId);
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.MainFrame.Content = window;
                }
            }
        }

        #endregion
    }
}
