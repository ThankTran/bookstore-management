using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.DTOs.ImportBill.Responses;
using bookstore_Management.DTOs.Order.Responses;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using bookstore_Management.Presentation.Views.Dialogs.Invoices;

namespace bookstore_Management.Presentation.Views.Orders
{
    public partial class InvoiceView : UserControl, INotifyPropertyChanged
    {
        #region Fields & Services

        private  IImportBillService _importBillService;
        private  IOrderService _orderService;
        private List<InvoiceDisplayItem> _allInvoices = new List<InvoiceDisplayItem>();

        private InvoiceFilterType _currentFilter = InvoiceFilterType.All;
        private ObservableCollection<InvoiceDisplayItem> _invoices = new ObservableCollection<InvoiceDisplayItem>();

        public ObservableCollection<InvoiceDisplayItem> Invoices
        {
            get => _invoices;
            set 
            { 
                _invoices = value; 
                OnPropertyChanged(nameof(Invoices)); 
            }
        }


        #endregion

        #region Constructor & Initialization

        public InvoiceView()
        {
            InitializeComponent();
            DataContext = this;

            InitializeServices();
            LoadAllInvoices();
        }

        private void InitializeServices()
        {
            var context = new BookstoreDbContext();

            // Import Bill Service
     
            var bookRepo = new BookRepository(context);
            var unitOfWork = new UnitOfWork(context);

            _importBillService = new ImportBillService(
                unitOfWork
            );

            // Order Service

            _orderService = new OrderService(
                unitOfWork
            );
        }

        #endregion

        #region Data Loading

        private async void LoadAllInvoices()
        {
            try
            {
                var displayItems = new List<InvoiceDisplayItem>();

                // Load Import Bills
                var importResult = await _importBillService.GetAllImportBillsAsync();
                if (importResult.IsSuccess && importResult.Data != null)
                {
                    displayItems.AddRange(importResult.Data.Select(MapImportBillToDisplay));
                }

                // Load Order Bills
                var orderResult = await _orderService.GetAllOrdersAsync();
                if (orderResult.IsSuccess && orderResult.Data != null)
                {
                    displayItems.AddRange(orderResult.Data.Select(MapOrderToDisplay));
                }

                // Sort by date descending
                Invoices = new ObservableCollection<InvoiceDisplayItem>(
                    displayItems.OrderByDescending(x => x.CreatedDate)
                );

                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilter(InvoiceFilterType filterType)
        {
            _currentFilter = filterType;

            IEnumerable<InvoiceDisplayItem> source = _allInvoices;

            switch (filterType)
            {
                case InvoiceFilterType.Import:
                    source = source.Where(x => x.InvoiceType == InvoiceType.Import);
                    break;
                case InvoiceFilterType.Export:
                    source = source.Where(x => x.InvoiceType == InvoiceType.Export);
                    break;
            }

            Invoices = new ObservableCollection<InvoiceDisplayItem>(source);
            UpdateTabStyles();
            UpdateStatistics();
        }


        #endregion

        #region Mapping Methods

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

        #region UI Event Handlers

        private void TabAll_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter(InvoiceFilterType.All);
        }

        private void TabImport_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter(InvoiceFilterType.Import);
        }

        private void TabExport_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter(InvoiceFilterType.Export);
        }

        private async void BtnAddImport_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CreateImportBill { Owner = Application.Current.MainWindow };

            // TODO: Load Publishers
            // dialog.LoadPublishers(publishers);
            // dialog.SetCreatedBy(currentUserId, currentUserName);

            if (dialog.ShowDialog() == true)
            {
                var dto = dialog.GetImportBillData();
                var result = await _importBillService.CreateImportBillAsync(dto);

                if (result.IsSuccess)
                {
                    MessageBox.Show("Tạo phiếu nhập thành công!", "Thành công",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadAllInvoices();
                }
                else
                {
                    MessageBox.Show(result.ErrorMessage, "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void BtnAddExport_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CreateOrderBill { Owner = Application.Current.MainWindow };

            // TODO: Load Staffs & Customers
            // dialog.LoadStaffs(staffs);
            // dialog.LoadCustomers(customers);

            if (dialog.ShowDialog() == true)
            {
                var dto = dialog.GetOrderData();
                var result = await _orderService.CreateOrderAsync(dto);

                if (result.IsSuccess)
                {
                    MessageBox.Show("Tạo hóa đơn bán thành công!", "Thành công",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadAllInvoices();
                }
                else
                {
                    MessageBox.Show(result.ErrorMessage, "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnView_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is InvoiceDisplayItem item)
            {
                OpenDetailView(item);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is InvoiceDisplayItem item)
            {
                OpenEditDialog(item);
            }
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is InvoiceDisplayItem item)
            {
                PrintInvoice(item);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is InvoiceDisplayItem item)
            {
                DeleteInvoice(item);
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = txtSearch.Text?.Trim().ToLower() ?? "";

            if (string.IsNullOrWhiteSpace(searchText))
            {
                ApplyFilter(_currentFilter);
                return;
            }

            var filtered = _allInvoices.Where(x =>
                            (x.InvoiceId ?? "").ToLower().Contains(searchText) ||
                            (x.Partner ?? "").ToLower().Contains(searchText) ||
                            (x.CreatedBy ?? "").ToLower().Contains(searchText)
                        ).ToList();

            Invoices = new ObservableCollection<InvoiceDisplayItem>(filtered);

        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgInvoices.SelectedItem is InvoiceDisplayItem item)
            {
                OpenDetailView(item);
            }
        }

        #endregion

        #region Business Logic

        private void OpenDetailView(InvoiceDisplayItem item)
        {
            if (item.InvoiceType == InvoiceType.Import)
            {
                // TODO: Navigate to ImportDetailView with item.InvoiceId
                MessageBox.Show($"Mở chi tiết phiếu nhập: {item.InvoiceId}", "Thông báo");
            }
            else
            {
                // TODO: Navigate to OrderDetailView with item.InvoiceId
                MessageBox.Show($"Mở chi tiết hóa đơn bán: {item.InvoiceId}", "Thông báo");
            }
        }

        private void OpenEditDialog(InvoiceDisplayItem item)
        {
            var confirm = MessageBox.Show(
                $"Bạn có chắc muốn sửa {(item.InvoiceType == InvoiceType.Import ? "phiếu nhập" : "hóa đơn bán")} {item.InvoiceId}?",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (confirm != MessageBoxResult.Yes) return;

            if (item.InvoiceType == InvoiceType.Import)
            {
                // TODO: Open EditImportBillDialog
                MessageBox.Show("Chức năng sửa phiếu nhập đang phát triển", "Thông báo");
            }
            else
            {
                // TODO: Open EditOrderDialog
                MessageBox.Show("Chức năng sửa hóa đơn bán đang phát triển", "Thông báo");
            }
        }

        private void PrintInvoice(InvoiceDisplayItem item)
        {
            try
            {
                // TODO: Implement print logic
                MessageBox.Show(
                    $"In {(item.InvoiceType == InvoiceType.Import ? "phiếu nhập" : "hóa đơn bán")}: {item.InvoiceId}\n" +
                    $"Đối tác: {item.Partner}\n" +
                    $"Tổng tiền: {item.TotalAmount:N0} ₫",
                    "In hóa đơn"
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi in: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteInvoice(InvoiceDisplayItem item)
        {
            var confirm = MessageBox.Show(
                $"Bạn có chắc muốn xóa {(item.InvoiceType == InvoiceType.Import ? "phiếu nhập" : "hóa đơn bán")} {item.InvoiceId}?\n" +
                $"Hành động này không thể hoàn tác!",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                if (item.InvoiceType == InvoiceType.Import)
                {
                    var result = await _importBillService.DeleteImportBillAsync(item.InvoiceId);

                    if (result.IsSuccess)
                    {
                        MessageBox.Show("Xóa phiếu nhập thành công!", "Thành công",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadAllInvoices();
                    }
                    else
                    {
                        MessageBox.Show(result.ErrorMessage, "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    var result =await _orderService.DeleteOrderAsync(item.InvoiceId);

                    if (result.IsSuccess)
                    {
                        MessageBox.Show("Xóa hóa đơn bán thành công!", "Thành công",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadAllInvoices();
                    }
                    else
                    {
                        MessageBox.Show(result.ErrorMessage, "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Helper Methods

        private void UpdateStatistics()
        {
            var importCount = Invoices.Count(x => x.InvoiceType == InvoiceType.Import);
            var exportCount = Invoices.Count(x => x.InvoiceType == InvoiceType.Export);

            // TODO: Update UI statistics labels
            // tbImportCount.Text = importCount.ToString();
            // tbExportCount.Text = exportCount.ToString();
        }

        private void UpdateTabStyles()
        {
            // TODO: Update tab visual states based on _currentFilter
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }

    #region Supporting Classes

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

    #endregion
}