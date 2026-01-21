using bookstore_Management.Core.Enums;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.DTOs.ImportBill.Responses;
using bookstore_Management.DTOs.Order.Responses;
using bookstore_Management.Models;
using bookstore_Management.Presentation.Views.Dialogs.Invoices;
using bookstore_Management.Presentation.Views.Dialogs.Share;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

namespace bookstore_Management.Presentation.ViewModels
{
    public class InvoiceViewModel : BaseViewModel
    {
        #region Services

        private readonly IImportBillService _importBillService;
        private readonly IOrderService _orderService;
        IPublisherService _publisherService;
        IStaffService _staffService;
        ICustomerService _customerService;

        #endregion

        #region Data

        private ObservableCollection<InvoiceDisplayItem> _invoices;
        public ObservableCollection<InvoiceDisplayItem> Invoices
        {
            get => _invoices;
            set
            {
                _invoices = value;
                OnPropertyChanged();
            }
        }

        private string _searchKeywork;
        public string SearchKeywork
        {
            get => _searchKeywork;
            set
            {
                _searchKeywork = value;
                OnPropertyChanged();
                SearchCommand.Execute(null);
            }
        }
        private int _totalInvoices;
        public int TotalInvoices
        {
            get => _totalInvoices;
            set
            {
                _totalInvoices = value;
                OnPropertyChanged();
            }
        }

        private int _totalImportInvoices;
        public int TotalImportInvoices
        {
            get => _totalImportInvoices;
            set
            {
                _totalImportInvoices = value;
                OnPropertyChanged();
            }
        }

        private int _totalExportInvoices;
        public int TotalExportInvoices
        {
            get => _totalExportInvoices;
            set
            {
                _totalExportInvoices = value;
                OnPropertyChanged();
            }
        }

        private List<InvoiceDisplayItem> _allInvoices = new List<InvoiceDisplayItem>();


        private InvoiceDisplayItem _selectedInvoice;
        public InvoiceDisplayItem SelectedInvoice
        {
            get => _selectedInvoice;
            set
            {
                _selectedInvoice = value;
                OnPropertyChanged();
            }
        }


        private InvoiceFilterType _currentFilter = InvoiceFilterType.All;

        #endregion

        #region Commands

        public ICommand LoadCommand { get; }
        public ICommand FilterAllCommand { get; }
        public ICommand FilterImportCommand { get; }
        public ICommand FilterExportCommand { get; }

        public ICommand AddImportCommand { get; }
        public ICommand AddExportCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand PrintCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand ExportCommand { get; }

        public ICommand SearchCommand { get; set; }

        #endregion

        #region Constructor

        public InvoiceViewModel(IImportBillService importBillService, IOrderService orderService, IPublisherService publisherService
        , IStaffService staffService, ICustomerService customerService)
        {
            _importBillService = importBillService;
            _orderService = orderService;
            _publisherService = publisherService;
            _staffService = staffService;
            _customerService = customerService;

            Invoices = new ObservableCollection<InvoiceDisplayItem>();

            Invoices = new ObservableCollection<InvoiceDisplayItem>();

            LoadCommand = new RelayCommand(
                 () => LoadAllInvoices()
             );

            FilterAllCommand = new RelayCommand(
                () => ApplyFilter(InvoiceFilterType.All)
            );

            FilterImportCommand = new RelayCommand(
                () => ApplyFilter(InvoiceFilterType.Import)
            );

            FilterExportCommand = new RelayCommand(
                () => ApplyFilter(InvoiceFilterType.Export)
            );

            AddImportCommand = new RelayCommand(

                () => AddImportInvoice()
            );


            AddExportCommand = new RelayCommand(
                () => AddExportInvoice()
            );

            DeleteCommand = new RelayCommand(
                () => DeleteSelectedInvoice(),
                () => SelectedInvoice != null
            );

            PrintCommand = new RelayCommand<object>((p) => PrintSelectedInvoice());

            ExportCommand = new RelayCommand(() => ExportInvoice());

            SearchCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrWhiteSpace(SearchKeywork))
                {
                    ApplyFilter(_currentFilter);
                    return;
                }

                var keyword = SearchKeywork.Trim().ToLower();

                var filtered = _allInvoices.Where(x =>
                    (x.InvoiceType == InvoiceType.Export &&
                     (x.InvoiceId ?? "").ToLower().Contains(keyword))
                    ||
                    (x.InvoiceType == InvoiceType.Import &&
                     (x.Partner ?? "").ToLower().Contains(keyword))
                );

                Invoices = new ObservableCollection<InvoiceDisplayItem>(
                    filtered.OrderByDescending(x => x.CreatedDate)
                );
            });

            EditCommand = new RelayCommand<InvoiceDisplayItem>(
                (invoice) => EditInvoice(invoice),
                (invoice) => invoice != null
            );




            LoadAllInvoices();
        }

        #endregion

        #region Load Data

        public void LoadAllInvoices()
        {
            try
            {
                _allInvoices.Clear();

                var importResult = _importBillService.GetAllImportBills();
                if (importResult.IsSuccess && importResult.Data != null)
                {
                    _allInvoices.AddRange(
                        importResult.Data.Select(MapImportBillToDisplay)
                    );
                }

                var orderResult = _orderService.GetAllOrders();
                if (orderResult.IsSuccess && orderResult.Data != null)
                {
                    _allInvoices.AddRange(
                        orderResult.Data.Select(MapOrderToDisplay)
                    );
                }

                ApplyFilter(_currentFilter);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            CalTotal();
        }

        #endregion

        #region Mapping

        private InvoiceDisplayItem MapImportBillToDisplay(ImportBillResponseDto import)
        {
            return new InvoiceDisplayItem
            {
                InvoiceId = import.Id,
                InvoiceType = InvoiceType.Import,
                Partner = import.PublisherName ?? "N/A",
                CreatedDate = import.CreatedDate,
                TotalAmount = import.TotalAmount,
                CreatedBy = import.CreatedBy ?? "System",
                Notes = import.Notes
            };
        }

        private InvoiceDisplayItem MapOrderToDisplay(OrderResponseDto order)
        {
            return new InvoiceDisplayItem
            {
                InvoiceId = order.OrderId,
                InvoiceType = InvoiceType.Export,
                Partner = order.CustomerName ?? "Khách vãng lai",
                CreatedDate = order.CreatedDate,
                TotalAmount = order.TotalPrice,
                CreatedBy = order.StaffName ?? "N/A",
                Notes = order.Notes
            };
        }

        #endregion

        #region Filter & Search

        private void ApplyFilter(InvoiceFilterType filterType)
        {
            _currentFilter = filterType;

            IEnumerable<InvoiceDisplayItem> source = _allInvoices;

            if (filterType == InvoiceFilterType.Import)
                source = source.Where(x => x.InvoiceType == InvoiceType.Import);
            else if (filterType == InvoiceFilterType.Export)
                source = source.Where(x => x.InvoiceType == InvoiceType.Export);

            Invoices = new ObservableCollection<InvoiceDisplayItem>(
                source.OrderByDescending(x => x.CreatedDate)
            );
        }

        private void ApplySearch()
        {
            if (string.IsNullOrWhiteSpace(SearchKeywork))
            {
                ApplyFilter(_currentFilter);
                return;
            }

            var text = SearchKeywork.ToLower();

            Invoices = new ObservableCollection<InvoiceDisplayItem>(
                _allInvoices.Where(x =>
                    (x.InvoiceId ?? "").ToLower().Contains(text) ||
                    (x.Partner ?? "").ToLower().Contains(text) ||
                    (x.CreatedBy ?? "").ToLower().Contains(text)
                )
            );
        }

        #endregion

        #region Actions

        private void AddImportInvoice()
        {
            if (SessionModel.Role == UserRole.InventoryManager && SessionModel.Role != UserRole.CustomerManager)
            {
                var noPermission = new NAdd();
                noPermission.ShowDialog();
                return;
            }

            try
            {
                var dialog = new CreateImportBill(
                    App.Services.GetRequiredService<IBookService>()
                );

                var mainWindow = Application.Current.MainWindow;
                if (mainWindow != null && dialog != mainWindow)
                {
                    dialog.Owner = mainWindow;
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                }
                else
                {
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }

                var publishersResult = _publisherService.GetAllPublishers();

                if (!publishersResult.IsSuccess)
                {
                    MessageBox.Show("Không thể tải danh sách nhà xuất bản",
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
        
                dialog.LoadPublishers(publishersResult.Data);

                if (dialog.ShowDialog() == true)
                {
                    var dto = dialog.GetImportBillData();
                    var result = _importBillService.CreateImportBill(dto);

                    if (!result.IsSuccess)
                    {
                        MessageBox.Show(result.ErrorMessage, "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    LoadAllInvoices();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}\n\nChi tiết: {ex.StackTrace}", 
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void AddExportInvoice()
        {
            if (SessionModel.Role == UserRole.InventoryManager && SessionModel.Role != UserRole.CustomerManager)
            {
                var noPermission = new NAdd();
                noPermission.ShowDialog();
                return; 
            }

            try
            {
                var dialog = new CreateOrderBill(
                    App.Services.GetRequiredService<IBookService>()
                );

                var mainWindow = Application.Current.MainWindow;
                if (mainWindow != null && dialog != mainWindow)
                {
                    dialog.Owner = mainWindow;
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                }
                else
                {
                    dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }

                dialog.LoadStaffs(_staffService.GetAllStaff().Data);
                dialog.LoadCustomers(_customerService.GetAllCustomers().Data);

                if (dialog.ShowDialog() == true)
                {
                    var dto = dialog.GetOrderData();
                    var result = _orderService.CreateOrder(dto);

                    if (!result.IsSuccess)
                    {
                        MessageBox.Show(result.ErrorMessage, "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    LoadAllInvoices();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}\n\nChi tiết: {ex.StackTrace}", 
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void DeleteSelectedInvoice()
        {
            if (SessionModel.Role == UserRole.InventoryManager || SessionModel.Role == UserRole.CustomerManager)
            {
                var noPermission = new NDelete();
                noPermission.ShowDialog();
            }
            if (SelectedInvoice == null) return;

            bool confirmed = Delete.ShowForInvoice(
                SelectedInvoice.InvoiceId,
                Application.Current.MainWindow
            );

            if (!confirmed) return;

            if (SelectedInvoice.InvoiceType == InvoiceType.Import)
                _importBillService.DeleteImportBill(SelectedInvoice.InvoiceId);
            else
                _orderService.DeleteOrder(SelectedInvoice.InvoiceId);

            LoadAllInvoices();
        }

        private void PrintSelectedInvoice()
        {
            if (SelectedInvoice == null) return;

            MessageBox.Show(
                $"In hóa đơn {SelectedInvoice.InvoiceId}\n" +
                $"Đối tác: {SelectedInvoice.Partner}\n" +
                $"Tổng tiền: {SelectedInvoice.TotalAmount:N0} ₫",
                "In hóa đơn"
            );
        }

        public void CalTotal()
        {
            TotalInvoices = _allInvoices.Count;

            TotalExportInvoices = _allInvoices
                .Count(x => x.InvoiceType == InvoiceType.Export);

            TotalImportInvoices = _allInvoices
                .Count(x => x.InvoiceType == InvoiceType.Import);
        }

        private void EditInvoice(InvoiceDisplayItem invoice)
        {
            if (SessionModel.Role == UserRole.InventoryManager || SessionModel.Role == UserRole.CustomerManager)
            {
                var noPermission = new NUpdate();
                noPermission.ShowDialog();
                return;
            }
            if (invoice == null) return;

            if (invoice.InvoiceType == InvoiceType.Import)
            {
                var dialog = new EditImportBillDialog( invoice.InvoiceId,
                    App.Services.GetRequiredService<IImportBillService>() );
                dialog.ShowDialog();
            }
            else
            {
                var dialog = new EditOrderDialog(invoice.InvoiceId,
                    App.Services.GetRequiredService<IOrderService>());
                dialog.ShowDialog();
            }
        }
        
        private void ExportInvoice()
        {
            var dialog = new ExportExcelInvoice(_importBillService, _orderService);
            dialog.ShowDialog();
        }


        #endregion
    }
    public class InvoiceDisplayItem     
    {
        public int STT { get; set; }
        public string InvoiceId { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public string TypeDisplay => InvoiceType == InvoiceType.Import ? "Nhập" : "Xuất";
        public string Partner { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedDateDisplay => CreatedDate.ToString("dd/MM/yyyy");
        public decimal TotalAmount { get; set; }
        public string TotalAmountDisplay => $"{TotalAmount:N0} ₫";
        public string CreatedBy { get; set; }
        public string Notes { get; set; }
    }
    public enum InvoiceType
    {
        Import,  // Phiếu nhập
        Export   // Hóa đơn bán
    }

    public enum InvoiceFilterType
    {
        All,
        Import,
        Export
    }
}
