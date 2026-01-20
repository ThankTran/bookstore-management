using bookstore_Management.Core.Enums;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Presentation.ViewModels;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;


namespace bookstore_Management.Presentation.Views.Statistics
{

    public partial class DashboardView : UserControl
    {
        private DashboardViewModel _viewModel;

        public DashboardView()
        {
            InitializeComponent();
            var context = new BookstoreDbContext();
            IReportService reportService;
            reportService = new ReportService(
                new OrderRepository(context),
                new OrderDetailRepository(context),
                new BookRepository(context),
                new CustomerRepository(context),
                new ImportBillRepository(context),
                new ImportBillDetailRepository(context)
            );
            _viewModel = new DashboardViewModel(reportService);
            this.DataContext = _viewModel;

           _viewModel.SelectedTimeRange = TimeRange.Today;
        }


    }
}