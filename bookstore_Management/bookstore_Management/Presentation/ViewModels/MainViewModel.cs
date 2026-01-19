using bookstore_Management.DTOs.Common.Reports;
using bookstore_Management.Presentation.Views.Orders;
using bookstore_Management.Services.Interfaces;
using bookstore_Management.Presentation.Views.Statistics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace bookstore_Management.Presentation.ViewModels
{
    internal class MainViewModel : BaseViewModel
    {

        private readonly IReportService _reportService;
        private readonly NavigationService _navigationService;

        public decimal TodayRevenue { get; set; }
        public int TodayOrders { get; set; }
        public int NewCustomers { get; set; }
        public int LowStockBooks { get; set; }

        public ICommand OrderCommand  { get; set; }
        public ICommand StaffCommand { get; set; }
        public ICommand StatiticsCommand { get; set; }

        public ObservableCollection<BookSalesReportResponseDto> TopSellingBooks { get; set; }

        public MainViewModel(IReportService reportService)
        {
            _reportService = reportService;
            LoadDashboardData();

            #region commands
            OrderCommand = new RelayCommand<object>((p) =>
            {
                _navigationService.Navigate?.Invoke(new InvoiceView());
            });
            StaffCommand = new RelayCommand<object>((p) =>
            {
                _navigationService.Navigate?.Invoke(new InvoiceView());
            });
            StatiticsCommand = new RelayCommand<object>((p) =>
            {
                _navigationService.Navigate?.Invoke(new DashboardView());
            });
            #endregion
        }


        private void LoadDashboardData()
        {
            var from = DateTime.Today.AddMonths(-1);
            var to = DateTime.Today.AddDays(1).AddTicks(-1);


            TodayRevenue = _reportService.GetTotalRevenue(from, to).Data;
            TodayOrders = _reportService.GetTotalOrderCount(from, to).Data;
            NewCustomers = _reportService.GetTotalCustomerCount(from, to).Data;// 1. Gọi hàm 1 lần duy nhất và lưu kết quả vào biến
                                                                               // Tách ra thành biến riêng để kiểm tra lỗi
            var inventoryResult = _reportService.GetInventorySummary();

            // Chỉ lấy dữ liệu khi Thành công (Succeeded) và Data khác null
            if (inventoryResult.IsSuccess && inventoryResult.Data != null)
            {
                LowStockBooks = inventoryResult.Data.OutOfStockCount;
            }
            else
            {
                // Nếu lỗi thì gán = 0 và xem thử lỗi gì (để sửa ở Bước 2)
                LowStockBooks = 0;
                // Đặt chuột vào biến errorMessage bên dưới khi debug để xem nó báo lỗi gì
                var errorMessage = inventoryResult.ErrorMessage;
            }

            var topBooksResult = _reportService.GetTopSellingBooks(from, to, 3);
            if (topBooksResult.IsSuccess)
            {
                TopSellingBooks = new ObservableCollection<BookSalesReportResponseDto>(
                    topBooksResult.Data
                );
            }


            OnPropertyChanged(nameof(TodayRevenue));
            OnPropertyChanged(nameof(TodayOrders));
            OnPropertyChanged(nameof(NewCustomers));
            OnPropertyChanged(nameof(LowStockBooks));
            OnPropertyChanged(nameof(TopSellingBooks));
        }
    }
    public class NavigationService
    {
        public Action<UserControl> Navigate { get; set; }
    }

}
