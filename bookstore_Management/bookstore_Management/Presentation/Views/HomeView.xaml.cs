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

namespace bookstore_Management.Presentation.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        
        public HomeView()
        {
            InitializeComponent();
            
            var context = new BookstoreDbContext();
            var unitOfWork = new UnitOfWork(context);
            IReportService reportService = new ReportService(unitOfWork);
            var viewModel = new MainViewModel(reportService);
            this.DataContext = viewModel;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var main = Window.GetWindow(this) as MainWindow;

            if (main != null)
            {
                main.MainFrame.Content = new PaymentView();
                main.SetClickedButtonColor(main.btnPayment);
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var main = Window.GetWindow(this) as MainWindow;

            if (main != null)
            {
                main.MainFrame.Content = new InvoiceView();
                main.SetClickedButtonColor(main.btnBills);
            }            
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var main = Window.GetWindow(this) as MainWindow;

            if (main != null)
            {
                main.MainFrame.Content = new Views.Statistics.DashboardView();
                main.SetClickedButtonColor(main.btnStatistics);
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var main = Window.GetWindow(this) as MainWindow;

            if (main != null)
            {
                main.MainFrame.Content = new InvoiceView();
                main.SetClickedButtonColor(main.btnBills);
            }
        }
    }
}