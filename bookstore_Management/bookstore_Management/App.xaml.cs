using System;
using System.Windows;
using bookstore_Management;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Presentation.ViewModels;
using bookstore_Management.Presentation.Views;
using bookstore_Management.Presentation.Views.Orders;
using bookstore_Management.Presentation.Views.Payment;
using bookstore_Management.Presentation.Views.Publishers;
using bookstore_Management.Presentation.Views.Statistics;
using bookstore_Management.Presentation.Views.Users;
using bookstore_Management.Services;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using bookstore_Management.Views.Books;
using bookstore_Management.Views.Customers;
using Microsoft.Extensions.DependencyInjection;

namespace bookstore_Management
{
    public partial class App : Application
{
    
    public static IServiceProvider Services { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        // ===== DB / UNIT OF WORK =====
        services.AddScoped<BookstoreDbContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // ===== SERVICES =====
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IStaffService, StaffService>();
        services.AddScoped<IPublisherService, PublisherService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IImportBillService, ImportBillService>();
        services.AddScoped<IOrderService, OrderService>();

        // ===== VIEWMODELS =====
        services.AddTransient<LoginViewModel>();
        services.AddTransient<BookViewModel>();
        services.AddTransient<CustomerViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<InvoiceViewModel>();
        services.AddTransient<PaymentViewModel>();
        services.AddTransient<PublisherViewModel>();
        services.AddTransient<StaffViewModel>();
        services.AddTransient<UserViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<PrintViewModel>();

        // ===== VIEWS =====
        services.AddTransient<LoginView>();
        services.AddScoped<MainWindow>();
        services.AddTransient<HomeView>();
        services.AddTransient<BookListView>();
        services.AddTransient<StaffListView>();
        services.AddTransient<RoleManagementView>();
        services.AddTransient<AccountListView>();
        services.AddTransient<PublisherListView>();
        services.AddTransient<DashboardView>();
        services.AddTransient<ImportDetailView>();
        services.AddTransient<OrderDetailView>();
        services.AddTransient<InvoiceView>();
        services.AddTransient<PaymentView>();
        services.AddTransient<CustomerListView>();
        services.AddTransient<CustomerDetailView>();
        services.AddTransient<BookListView>();

        Services = services.BuildServiceProvider();
        
        var scope = Services.CreateScope();
        var loginView = scope.ServiceProvider.GetRequiredService<LoginView>();
        loginView.Closed += (_, __) => scope.Dispose();
        loginView.Show();
    }
}
}
