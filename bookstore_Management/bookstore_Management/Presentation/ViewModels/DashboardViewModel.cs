using bookstore_Management.Core.Enums;
using bookstore_Management.DTOs.Common.Reports;
using bookstore_Management.Services.Interfaces;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.ObjectModel;
using System.Linq;
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

        #endregion

        #region === CONSTRUCTOR ===

        public DashboardViewModel(IReportService reportService)
        {
            _reportService = reportService;

            RevenueSeries = new SeriesCollection();
            CategorySeries = new SeriesCollection();
            CustomerSeries = new SeriesCollection();

            MoneyFormatter = value => value.ToString("N0");

            SelectTimeCommand = new RelayCommand<TimeRange>(p =>
            {
                SelectedTimeRange = p;
            });

            SelectedTimeRange = TimeRange.Today;
        }

        #endregion

        #region === LOAD DATA ===

        private void LoadDataByTime()
        {
            LoadCategoryChart();
            LoadCustomerChart();
            LoadSummaryNumbers();
        }

        #endregion

        #region === CATEGORY COLUMN CHART ===

        private void LoadCategoryChart()
        {
            var result = _reportService.GetTopSellingBooks(From, To);

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

        private void LoadCustomerChart()
        {
            var data = _reportService.GetWalkInToMemberPurchaseRatio(From, To).Data;

            CustomerSeries.Clear();

            Member = data.Member;
            WalkIn = data.WalkIn;
            TotalCustomers = Member + WalkIn;

            CustomerSeries.Add(new PieSeries
            {
                Title = "Thành viên",
                Values = new ChartValues<int> { Member },
                DataLabels = true
            });

            CustomerSeries.Add(new PieSeries
            {
                Title = "Vãng lai",
                Values = new ChartValues<int> { WalkIn },
                DataLabels = true
            });

            OnPropertyChanged(nameof(CustomerSeries));
        }

        #endregion

        #region === SUMMARY NUMBERS ===

        private void LoadSummaryNumbers()
        {
            TodayRevenue = _reportService.GetTotalRevenue(From, To).Data;
            TodayOrders = _reportService.GetTotalOrderCount(From, To).Data;
            NewCustomers = _reportService.GetTotalCustomerCount(From, To).Data;
            LowStockBooks = _reportService.GetInventorySummary().Data.OutOfStockCount;

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

                case TimeRange.All:
                    From = DateTime.Now.AddYears(-2);
                    To = DateTime.Now;
                    break;
            }
        }

        #endregion
    }
}
