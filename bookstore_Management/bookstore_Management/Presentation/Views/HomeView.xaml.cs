using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Presentation.ViewModels;
using bookstore_Management.Presentation.Views.Payment;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Presentation.Views;
using bookstore_Management.Services.Interfaces;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using bookstore_Management.Presentation.Views.Orders;
using bookstore_Management.Presentation.Views.Statistics;
using Microsoft.Extensions.DependencyInjection;

namespace bookstore_Management.Presentation.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        
        public HomeView(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var main = Window.GetWindow(this) as MainWindow;
            if (main == null) return;

            var paymentView = App.Services.GetRequiredService<PaymentView>();
            main.MainFrame.Content = paymentView;
            main.SetClickedButtonColor(main.btnPayment);
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var main = Window.GetWindow(this) as MainWindow;
            if (main == null) return;

            var invoiceView = App.Services.GetRequiredService<InvoiceView>();
            main.MainFrame.Content = invoiceView;
            main.SetClickedButtonColor(main.btnBills);  
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var main = Window.GetWindow(this) as MainWindow;
            if (main == null) return;

            var dashboardView = App.Services.GetRequiredService<DashboardView>();
            main.MainFrame.Content = dashboardView;
            main.SetClickedButtonColor(main.btnStatistics);
            
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var main = Window.GetWindow(this) as MainWindow;
            if (main == null) return;

            var invoiceView = App.Services.GetRequiredService<InvoiceView>();
            main.MainFrame.Content = invoiceView;
            main.SetClickedButtonColor(main.btnBills);
        }
    }
}