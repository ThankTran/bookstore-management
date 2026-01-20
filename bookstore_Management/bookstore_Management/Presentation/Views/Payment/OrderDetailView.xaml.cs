using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.DTOs.Order.Responses;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using bookstore_Management.Presentation.Views;
using bookstore_Management.Presentation.Views.Dialogs.Share;
using bookstore_Management.Core.Enums;
using bookstore_Management.Presentation.ViewModels;

namespace bookstore_Management.Presentation.Views.Payment
{
    /// <summary>
    /// Interaction logic for OrderDetailView.xaml
    /// </summary>
    public partial class OrderDetailView : UserControl, IDisposable
    {
        private readonly IOrderService _orderService;
        private readonly BookstoreDbContext _context;
        private readonly UnitOfWork _unitOfWork;
        private string _currentOrderId;

        public OrderDetailView()
        {
            InitializeComponent();
            // Lưu reference để dispose sau
            _context = new BookstoreDbContext();
            _unitOfWork = new UnitOfWork(_context);
            _orderService = new OrderService(_unitOfWork);
        }

        public async Task LoadOrderAsync(string orderId)
        {
            if (string.IsNullOrEmpty(orderId)) return;

            _currentOrderId = orderId;
            var result = await _orderService.GetOrderByIdAsync(orderId);

            if (!result.IsSuccess || result.Data == null)
            {
                MessageBox.Show("Không thể tải thông tin đơn hàng: " + result.ErrorMessage,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var order = result.Data;

            // Load details
            var detailsResult = await _orderService.GetOrderDetailsAsync(orderId);
            if (detailsResult.IsSuccess && detailsResult.Data != null)
            {
                var details = detailsResult.Data.Select((d, index) => new
                {
                    Index = index + 1,
                    d.BookId,
                    d.BookName,
                    d.Quantity,
                    d.SalePrice,
                    d.Subtotal
                }).ToList();

                dgOrderDetails.ItemsSource = details;

                // Calculate summary
                var totalItems = details.Count;
                var totalQuantity = details.Sum(d => d.Quantity);
                txtTotalItems.Text = $"{totalItems} mặt hàng";
                txtTotalQuantity.Text = $"{totalQuantity} cuốn";
            }

            // Set header info
            txtOrderId.Text = $"Mã đơn: {order.OrderId}";
            txtInfoOrderId.Text = order.OrderId;
            txtCustomer.Text = order.CustomerName ?? "Khách vãng lai";
            txtInfoCustomer.Text = order.CustomerName ?? "Khách vãng lai";
            txtStatus.Text = "Hoàn thành";
            txtTotalAmount.Text = order.TotalPrice.ToString("N0") + " ₫";
            txtSubtotal.Text = (order.TotalPrice + order.Discount).ToString("N0") + " ₫";
            txtDiscount.Text = order.Discount > 0 ? "- " + order.Discount.ToString("N0") + " ₫" : "0 ₫";
            txtGrandTotal.Text = order.TotalPrice.ToString("N0") + " ₫";
            txtStaffName.Text = order.StaffName ?? "N/A";
            txtCreatedDate.Text = order.CreatedDate.ToString("dd/MM/yyyy - HH:mm");
            txtNotes.Text = order.Notes ?? "Không có ghi chú";

            // Payment method
            string paymentMethodText;
            switch (order.PaymentMethod)
            {
                case PaymentType.Cash:
                    paymentMethodText = " Tiền mặt";
                    break;
                case PaymentType.Card:
                    paymentMethodText = " Thẻ";
                    break;
                case PaymentType.BankTransfer:
                    paymentMethodText = " Chuyển khoản";
                    break;
                case PaymentType.DebitCard:
                    paymentMethodText = "Tín dụng";
                    break;
                default:
                    paymentMethodText = " Tiền mặt";
                    break;
            }

            txtPaymentMethod.Text = paymentMethodText;
            txtInfoPaymentMethod.Text = paymentMethodText;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                // Reload data thay vì tạo view mới để tránh memory leak
                if (mainWindow.MainFrame.Content is InvoiceView existingView)
                {
                    var viewModel = existingView.DataContext as InvoiceViewModel;
                    _ = viewModel?.LoadAllDataAsync();
                }
                else
                {
                    mainWindow.MainFrame.Content = new InvoiceView();
                }
            }
        }

        private async void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentOrderId))
            {
                MessageBox.Show("Không có thông tin đơn hàng để in.", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            // TODO: Implement print functionality
            MessageBox.Show("Chức năng in đang được phát triển.", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void BtnCancelOrder_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentOrderId))
            {
                MessageBox.Show("Không có thông tin đơn hàng để hủy.", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show("Bạn có chắc chắn muốn hủy đơn hàng này không?",
                "Xác nhận hủy đơn",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            var deleteResult = await _orderService.DeleteOrderAsync(_currentOrderId);
            if (!deleteResult.IsSuccess)
            {
                MessageBox.Show($"Không thể hủy đơn hàng.\nChi tiết lỗi: {deleteResult.ErrorMessage}",
                    "Lỗi hủy đơn",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Đã hủy đơn hàng thành công.", "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Information);

            // Navigate back to invoice list - reload data thay vì tạo mới
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                if (mainWindow.MainFrame.Content is InvoiceView existingView)
                {
                    var viewModel = existingView.DataContext as InvoiceViewModel;
                    _ = viewModel?.LoadAllDataAsync();
                }
                else
                {
                    mainWindow.MainFrame.Content = new InvoiceView();
                }
            }
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
    }
}
