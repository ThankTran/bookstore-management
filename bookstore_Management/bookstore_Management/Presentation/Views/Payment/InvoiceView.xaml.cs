using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.DTOs.ImportBill.Responses;
using bookstore_Management.DTOs.Order.Responses;
using bookstore_Management.Presentation.ViewModels;
using bookstore_Management.Presentation.Views.Dialogs.Invoices;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using bookstore_Management.Presentation.Views.Payment;
using Microsoft.Extensions.DependencyInjection;

namespace bookstore_Management.Presentation.Views.Orders
{
    public partial class InvoiceView : UserControl
    {
       
        public InvoiceView(InvoiceViewModel invoiceViewModel)
        {
            InitializeComponent();
            DataContext = invoiceViewModel;
        }
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgInvoices.SelectedItem is InvoiceDisplayItem item)
            {
                OpenDetailView(item);
            }
        }

        private void OpenDetailView(InvoiceDisplayItem item)
        {
            try
            {
                
                var mainWindow = Window.GetWindow(this) as MainWindow;
        
                if (mainWindow == null)
                {
                    MessageBox.Show("Không tìm thấy MainWindow!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var scope = App.Services.CreateScope();

                if (item.InvoiceType == InvoiceType.Import)
                {
                    var detailView = scope.ServiceProvider.GetRequiredService<ImportDetailView>();
                    detailView.Unloaded += (_, __) => scope.Dispose();
                    detailView.LoadImportBillAsync(item.InvoiceId);
                    mainWindow.MainFrame.Content = detailView;
                }
                else
                {
                    var detailView = scope.ServiceProvider.GetRequiredService<OrderDetailView>();
                    detailView.Unloaded += (_, __) => scope.Dispose();
                    detailView.LoadOrderAsync(item.InvoiceId);
                    mainWindow.MainFrame.Content = detailView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgInvoices_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header=(e.Row.GetIndex()+1).ToString();
        }
    }
}