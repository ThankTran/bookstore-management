using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.DTOs.ImportBill.Responses;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using bookstore_Management.Presentation.Views;
using bookstore_Management.Presentation.Views.Dialogs.Share;
using bookstore_Management.Presentation.ViewModels;
using bookstore_Management.Presentation.Views.Dialogs.Invoices;
using bookstore_Management.Presentation.Views.Orders;
using Microsoft.Extensions.DependencyInjection;

namespace bookstore_Management.Presentation.Views.Payment
{
    /// <summary>
    /// Interaction logic for ImportDetailView.xaml
    /// </summary>
    public partial class ImportDetailView : UserControl, IDisposable
    {
        private readonly IImportBillService _importBillService;
        private string _currentImportBillId;

        public ImportDetailView(IImportBillService importBillService)
        {
            InitializeComponent();
            _importBillService = importBillService;
        }

        public void LoadImportBillAsync(string importBillId)
        {
            if (string.IsNullOrEmpty(importBillId)) return;

            _currentImportBillId = importBillId;
            var result = _importBillService.GetImportBillById(importBillId);

            if (!result.IsSuccess || result.Data == null)
            {
                MessageBox.Show("Không thể tải thông tin phiếu nhập: " + result.ErrorMessage, 
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var importBill = result.Data;
            
            // Load details
            var detailsResult = _importBillService.GetImportDetails(importBillId);
            if (detailsResult.IsSuccess && detailsResult.Data != null)
            {
                var details = detailsResult.Data.Select((d, index) => new
                {
                    Index = index + 1,
                    d.BookId,
                    d.BookName,
                    d.Quantity,
                    d.ImportPrice,
                    d.Subtotal
                }).ToList();

                dgImportDetails.ItemsSource = details;

                // Calculate summary
                var totalItems = details.Count;
                var totalQuantity = details.Sum(d => d.Quantity);
                txtSummary.Text = $"Tổng số mặt hàng: {totalItems} | Tổng số lượng: {totalQuantity} cuốn";
            }

            // Set header info
            txtImportBillId.Text = $"Mã phiếu: {importBill.Id}";
            txtInfoImportId.Text = importBill.Id;
            txtSupplier.Text = importBill.PublisherName ?? "N/A";
            txtInfoSupplier.Text = importBill.PublisherName ?? "N/A";
            txtStatus.Text = "Đã nhập kho";
            txtTotalAmount.Text = importBill.TotalAmount.ToString("N0") + " ₫";
            txtGrandTotal.Text = importBill.TotalAmount.ToString("N0") + " ₫";
            txtCreatedBy.Text = importBill.CreatedBy ?? "System";
            txtCreatedDate.Text = importBill.CreatedDate.ToString("dd/MM/yyyy - HH:mm");
            txtNotes.Text = importBill.Notes ?? "Không có ghi chú";
        }
        
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow == null) return;

            var scope = App.Services.CreateScope();
            var invoiceView = scope.ServiceProvider.GetRequiredService<InvoiceView>();
            invoiceView.Unloaded += (_, __) => scope.Dispose();
            
            mainWindow.MainFrame.Content = invoiceView;
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentImportBillId))
            {
                MessageBox.Show("Không có thông tin phiếu nhập để in.", "Thông báo", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var print = new PrintInvoice(_currentImportBillId, InvoiceType.Import, context);
            print.ShowDialog();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentImportBillId))
            {
                MessageBox.Show("Không có thông tin phiếu nhập để xóa.", "Thông báo", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var confirmed = Delete.ShowForInvoice(_currentImportBillId);
            if (!confirmed) return;

            var result = _importBillService.DeleteImportBill(_currentImportBillId);
            if (!result.IsSuccess)
            {
                MessageBox.Show($"Không thể xóa phiếu nhập.\nChi tiết lỗi: {result.ErrorMessage}",
                    "Lỗi xóa dữ liệu",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Đã xóa phiếu nhập thành công.", "Thông báo", 
                MessageBoxButton.OK, MessageBoxImage.Information);

            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow == null) return;

            var scope = App.Services.CreateScope();
            var invoiceView = scope.ServiceProvider.GetRequiredService<InvoiceView>();
            invoiceView.Unloaded += (_, __) => scope.Dispose();
            
            mainWindow.MainFrame.Content = invoiceView;
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
                    // // Dispose managed resources
                    // _unitOfWork?.Dispose();
                    // context?.Dispose();
                }
                _disposed = true;
            }
        }

        #endregion
    }
}