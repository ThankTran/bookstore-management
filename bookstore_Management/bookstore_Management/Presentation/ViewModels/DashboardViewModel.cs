using bookstore_Management.Core.Enums;
using bookstore_Management.DTOs.Common.Reports;
using bookstore_Management.Services.Interfaces;
using DocumentFormat.OpenXml.Bibliography;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace bookstore_Management.Presentation.ViewModels
{
    internal class DashboardViewModel : BaseViewModel
    {
        #region === CHART PROPERTIES ===

        // Doanh thu
        public SeriesCollection RevenueSeries { get; set; }
        public string[] RevenueLabels { get; set; }
        public Func<double, string> MoneyFormatter { get; set; }


        // Thể loại / sách bán chạy
        public SeriesCollection CategorySeries { get; set; }
        public string[] CategoryLabels { get; set; }

        // Khách hàng
        public SeriesCollection CustomerSeries { get; set; }

        //Doanh thu
        public SeriesCollection ProfitSeries { get; set; }
        public string[] ProfitLabels { get; set; }

        #endregion

        #region === SERVICES ===
        private readonly IReportService _reportService;
        #endregion

        #region === TIME RANGE ===

        private TimeRange _selectedTimeRange;
        public TimeRange SelectedTimeRange
        {
            get => _selectedTimeRange;
            set
            {
                _selectedTimeRange = value;
                OnPropertyChanged();
                CalculateTimeRange();
                LoadDataByTime();
            }
        }

        private DateTime _from;
        public DateTime From
        {
            get => _from;
            set { _from = value; OnPropertyChanged(); }
        }

        private DateTime _to;
        public DateTime To
        {
            get => _to;
            set { _to = value; OnPropertyChanged(); }
        }

        #endregion

        #region === DASHBOARD NUMBERS ===

        private int _walkIn;
        public int WalkIn
        {
            get => _walkIn;
            set { _walkIn = value; OnPropertyChanged(); }
        }

        private int _member;
        public int Member
        {
            get => _member;
            set { _member = value; OnPropertyChanged(); }
        }

        private int _totalCustomers;
        public int TotalCustomers
        {
            get => _totalCustomers;
            set { _totalCustomers = value; OnPropertyChanged(); }
        }

        public decimal TodayRevenue { get; set; }
        public int TodayOrders { get; set; }
        public int NewCustomers { get; set; }
        public int LowStockBooks { get; set; }

        #endregion

        #region === COMMANDS ===

        public ICommand SelectTimeCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand ShowOutOfStockDetailCommand { get; }


        #endregion

        #region === CONSTRUCTOR ===

        public DashboardViewModel(IReportService reportService)
        {
            _reportService = reportService;

            RevenueSeries = new SeriesCollection();
            CategorySeries = new SeriesCollection();
            CustomerSeries = new SeriesCollection();
            ProfitSeries = new SeriesCollection();

            MoneyFormatter = value => value.ToString("N0");

            SelectTimeCommand = new RelayCommand<TimeRange>(p =>
            {
                SelectedTimeRange = p;
                
            });


            RefreshCommand = new RelayCommand<TimeRange>(p =>
            {

            });

            ShowOutOfStockDetailCommand = new RelayCommand<object>(p =>
            {
                OpenOutOfStockDetail();
            });

        }

        #endregion

        private void OpenOutOfStockDetail()
        {
            MessageBox.Show($"Có {LowStockBooks} sách sắp hết hàng",
                            "Tồn kho thấp",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
        }


        #region === LOAD DATA ===

        private void LoadDataByTime()
        {
            LoadCategoryChart();
            LoadCustomerChart();
            LoadSummaryNumbers();
            LoadRevenueChart();
            LoadDoanhThuChart();
        }

        #endregion

        #region === CATEGORY COLUMN CHART ===

        private async Task LoadCategoryChart()
        {
            var result = await _reportService.GetTopSellingBooksAsync(From, To);

            CategorySeries.Clear();

            if (!result.IsSuccess || result.Data == null) return;

            Brush[] colors =
            {
                (Brush)Application.Current.FindResource("Chart1Brush"),
                (Brush)Application.Current.FindResource("Chart2Brush"),
                (Brush)Application.Current.FindResource("Chart3Brush"),
                (Brush)Application.Current.FindResource("Chart4Brush"),
                (Brush)Application.Current.FindResource("Chart5Brush"),
            };

            var top5 = result.Data.Take(5).ToList();
            CategoryLabels = new string[top5.Count];

            for (int i = 0; i < top5.Count; i++)
            {
                var item = top5[i];
                CategoryLabels[i] = item.BookName;

                CategorySeries.Add(new ColumnSeries
                {
                    Title = item.BookName, // legend
                    Values = new ChartValues<int> { item.TotalQuantitySold },
                    Fill = colors[i % colors.Length],
                    MaxColumnWidth = 45
                });
            }

            OnPropertyChanged(nameof(CategorySeries));
            OnPropertyChanged(nameof(CategoryLabels));
        }

        #endregion

        #region === CUSTOMER PIE CHART ===

        private async Task LoadCustomerChart()
        {
            var asy = await _reportService.GetWalkInToMemberPurchaseRatioAsync(From, To);
            var data = asy.Data;
            CustomerSeries.Clear();

            Member = data.Member;
            WalkIn = data.WalkIn;
            TotalCustomers = Member + WalkIn;

            CustomerSeries.Add(new PieSeries
            {
                Title = "Thành viên",
                Values = new ChartValues<int> { Member },
                DataLabels = true,
                Fill = (Brush)Application.Current.FindResource("PieChartMemberBrush")
            });

            CustomerSeries.Add(new PieSeries
            {
                Title = "Vãng lai",
                Values = new ChartValues<int> { WalkIn },
                DataLabels = true,
                Fill = (Brush)Application.Current.FindResource("PieChartRegularBrush")
            });

            OnPropertyChanged(nameof(CustomerSeries));
        }

        #endregion

        #region === SUMMARY NUMBERS ===

        private async Task LoadSummaryNumbers()
        {
            var revenue = await _reportService.GetTotalRevenueAsync(From, To);
            var orders = await _reportService.GetTotalOrderCountAsync(From, To);
            var cus = await _reportService.GetTotalCustomerCountAsync(From, To);
            var lowStock = await _reportService.GetInventorySummaryAsync();
            TodayRevenue = revenue.Data;
            TodayOrders = orders.Data;
            NewCustomers = cus.Data;
            LowStockBooks = lowStock.Data.LowStockCount;

            OnPropertyChanged(nameof(TodayRevenue));
            OnPropertyChanged(nameof(TodayOrders));
            OnPropertyChanged(nameof(NewCustomers));
            OnPropertyChanged(nameof(LowStockBooks));
        }

        #endregion

        #region === TIME RANGE CALC ===

        private void CalculateTimeRange()
        {
            switch (SelectedTimeRange)
            {
                case TimeRange.Today:
                    From = DateTime.Today;
                    To = DateTime.Now;
                    break;

                case TimeRange.ThisWeek:
                    From = DateTime.Now.AddDays(-7);
                    To = DateTime.Now;
                    break;

                case TimeRange.ThisMonth:
                    From = DateTime.Now.AddMonths(-1);
                    To = DateTime.Now;
                    break;

                case TimeRange.ThisQuarter:
                    From = DateTime.Now.AddMonths(-3);
                    To = DateTime.Now;
                    break;

                case TimeRange.ThisYear:
                    From = DateTime.Now.AddYears(-1);
                    To = DateTime.Now;
                    break;

                case TimeRange.ThePast30Days:
                    From = DateTime.Now.AddDays(-30);
                    To = DateTime.Now;
                    break;
            }
        }

        #endregion

        #region === REVENUE TREND ===
        public void LoadRevenueChart()
        {
            var result = _reportService.GetRevenue(From, To);

            if (!result.IsSuccess || result.Data == null) return;

            var data = result.Data.ToList();

            RevenueSeries.Clear();

            RevenueSeries.Add(new LineSeries
            {
                Title = "Doanh Thu (triệu)",
                Values = new ChartValues<double>(data.Select(x => (double)x / 1_000_000)),

                Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF9800")),
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF3E0")),

                StrokeThickness = 2,
                LineSmoothness = 0.7,
                PointGeometrySize = 5
            });

            RevenueLabels = Enumerable.Range(0, data.Count).Select(i => $"Ngày {i}").ToArray();

            OnPropertyChanged(nameof(RevenueSeries));
            OnPropertyChanged(nameof(RevenueLabels));

        }
        #endregion

        #region === dOANH THU TREND ===
        public void LoadDoanhThuChart()
        {
            var result = _reportService.GetImport(From, To);
            if (!result.IsSuccess || result.Data == null) return;

            var data = result.Data.ToList();

            ProfitSeries.Clear();

            ProfitSeries.Add(new LineSeries
            {
                Title = "Tổng chi /ngày (triệu)",
                Values = new ChartValues<double>(data.Select(x => (double)x / 1_000_000)),

                Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50")),
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#334CAF50")),

                StrokeThickness = 2,
                LineSmoothness = 0.7,
                PointGeometrySize = 5
            });

            ProfitLabels = Enumerable.Range(0, data.Count).Select(i => $"Ngày {i}").ToArray();

            OnPropertyChanged(nameof(ProfitSeries));
            OnPropertyChanged(nameof(ProfitLabels));

        }
        #endregion
    }
}
