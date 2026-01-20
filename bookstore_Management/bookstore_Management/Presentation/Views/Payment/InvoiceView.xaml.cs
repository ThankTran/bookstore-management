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
using bookstore_Management.Presentation.Views.Payment;

namespace bookstore_Management.Presentation.Views.Orders
{
    public partial class InvoiceView : UserControl
    {
       
        public InvoiceView()
        {
            InitializeComponent();
            
            var context = new BookstoreDbContext();
            IImportBillService importBillService;
            IOrderService orderService;

            importBillService = new ImportBillService(
                new ImportBillRepository(context),
                new ImportBillDetailRepository(context),
                new BookRepository(context),
                new PublisherRepository(context));

            orderService = new OrderService(
                new OrderRepository(context),
                new OrderDetailRepository(context),
                new BookRepository(context),
                new CustomerRepository(context),
                new StaffRepository(context)
                );

            DataContext = new InvoiceViewModel(importBillService, orderService);
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
            if (item.InvoiceType == InvoiceType.Import)
            {
                var detailView = new ImportDetailView();
                detailView.LoadImportBillAsync(item.InvoiceId);
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.MainFrame.Content = detailView;
                }
            }
            else
            {
                var detailView = new OrderDetailView();
                detailView.LoadOrderAsync(item.InvoiceId);
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.MainFrame.Content = detailView;
                }
            }
        }

        private void dgInvoices_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header=(e.Row.GetIndex()+1).ToString();
        }
    }
}