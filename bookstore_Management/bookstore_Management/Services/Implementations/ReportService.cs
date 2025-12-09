using System;
using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.DTOs.Reports;
using bookstore_Management.Services.Interfaces;

namespace bookstore_Management.Services.Implementations
{
    /// <summary>
    /// Service báo cáo và thống kê
    /// </summary>
    public class ReportService : IReportService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IStockRepository _stockRepository;
        private readonly IBookRepository _bookRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IStaffDailyRevenueRepository _revenueRepository;
        private readonly IImportBillRepository _importBillRepository;

        public ReportService(
            IOrderRepository orderRepository,
            IOrderDetailRepository orderDetailRepository,
            IStockRepository stockRepository,
            IBookRepository bookRepository,
            ICustomerRepository customerRepository,
            IStaffRepository staffRepository,
            IStaffDailyRevenueRepository revenueRepository,
            IImportBillRepository importBillRepository)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _stockRepository = stockRepository;
            _bookRepository = bookRepository;
            _customerRepository = customerRepository;
            _staffRepository = staffRepository;
            _revenueRepository = revenueRepository;
            _importBillRepository = importBillRepository;
        }

        // ==================================================================
        // ----------------------- BÁO CÁO DOANH THU -----------------------
        // ==================================================================

        /// <summary>
        /// Tính tổng doanh thu trong khoảng ngày
        /// </summary>
        public Result<decimal> GetTotalRevenue(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var orders = _orderRepository.Find(o =>
                    o.CreatedDate >= fromDate &&
                    o.CreatedDate <= toDate &&
                    o.DeletedDate == null);

                decimal totalRevenue = orders.Sum(o => o.TotalPrice);
                return Result<decimal>.Success(totalRevenue);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Tính tổng lợi nhuận trong khoảng ngày
        /// </summary>
        public Result<decimal> GetTotalProfit(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var orderDetails = _orderDetailRepository.Find(od =>
                    od.Order.CreatedDate >= fromDate &&
                    od.Order.CreatedDate <= toDate &&
                    od.Order.DeletedDate == null);

                decimal totalProfit = 0;
                foreach (var detail in orderDetails)
                {
                    var book = _bookRepository.GetById(detail.BookId);
                    if (book != null)
                    {
                        decimal profit = (detail.SalePrice - book.ImportPrice) * detail.Quantity;
                        totalProfit += profit;
                    }
                }

                return Result<decimal>.Success(totalProfit);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Tính giá trị trung bình mỗi đơn hàng
        /// </summary>
        public Result<decimal> GetAverageOrderValue(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var orders = _orderRepository.Find(o =>
                    o.CreatedDate >= fromDate &&
                    o.CreatedDate <= toDate &&
                    o.DeletedDate == null);

                if (!orders.Any())
                    return Result<decimal>.Success(0, "Không có đơn hàng");

                decimal averageValue = orders.Average(o => o.TotalPrice);
                return Result<decimal>.Success(averageValue);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- BÁO CÁO ĐƠN HÀNG -------------------------
        // ==================================================================

        /// <summary>
        /// Tính tổng số đơn hàng trong khoảng ngày
        /// </summary>
        public Result<int> GetTotalOrderCount(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var count = _orderRepository.Find(o =>
                    o.CreatedDate >= fromDate &&
                    o.CreatedDate <= toDate &&
                    o.DeletedDate == null).Count();

                return Result<int>.Success(count);
            }
            catch (Exception ex)
            {
                return Result<int>.Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Tính tổng tiền giảm giá đã cho trong khoảng ngày
        /// </summary>
        public Result<decimal> GetTotalDiscountGiven(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var orders = _orderRepository.Find(o =>
                    o.CreatedDate >= fromDate &&
                    o.CreatedDate <= toDate &&
                    o.DeletedDate == null);

                decimal totalDiscount = orders.Sum(o => o.Discount ?? 0);
                return Result<decimal>.Success(totalDiscount);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- BÁO CÁO SÁCH BÁN CHẠY --------------------
        // ==================================================================

        /// <summary>
        /// Lấy top sách bán chạy nhất
        /// </summary>
        public Result<IEnumerable<BookSalesReport>> GetTopSellingBooks(DateTime fromDate, DateTime toDate, int topCount = 10)
        {
            try
            {
                var orderDetails = _orderDetailRepository.Find(od =>
                    od.Order.CreatedDate >= fromDate &&
                    od.Order.CreatedDate <= toDate &&
                    od.Order.DeletedDate == null);

                var bookSalesReport = orderDetails
                    .GroupBy(od => od.BookId)
                    .Select(g => new BookSalesReport
                    {
                        BookId = g.Key,
                        BookName = _bookRepository.GetById(g.Key)?.Name ?? "Unknown",
                        TotalQuantitySold = g.Sum(od => od.Quantity),
                        TotalRevenue = g.Sum(od => od.SalePrice * od.Quantity),
                        AveragePricePerUnit = g.Average(od => od.SalePrice)
                    })
                    .OrderByDescending(r => r.TotalQuantitySold)
                    .Take(topCount)
                    .ToList();

                return Result<IEnumerable<BookSalesReport>>.Success(bookSalesReport);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<BookSalesReport>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy top sách bán ít nhất
        /// </summary>
        public Result<IEnumerable<BookSalesReport>> GetLowestSellingBooks(DateTime fromDate, DateTime toDate, int bottomCount = 5)
        {
            try
            {
                var orderDetails = _orderDetailRepository.Find(od =>
                    od.Order.CreatedDate >= fromDate &&
                    od.Order.CreatedDate <= toDate &&
                    od.Order.DeletedDate == null);

                var allBooks = _bookRepository.GetAll()
                    .Where(b => b.DeletedDate == null)
                    .ToList();

                var bookSalesReport = allBooks
                    .GroupJoin(
                        orderDetails,
                        book => book.BookId,
                        detail => detail.BookId,
                        (book, details) => new
                        {
                            book,
                            details = details.ToList()
                        })
                    .Select(x => new BookSalesReport
                    {
                        BookId = x.book.BookId,
                        BookName = x.book.Name,
                        TotalQuantitySold = x.details.Sum(od => od.Quantity),
                        TotalRevenue = x.details.Sum(od => od.SalePrice * od.Quantity),
                        AveragePricePerUnit = x.details.Any() ? x.details.Average(od => od.SalePrice) : 0
                    })
                    .OrderBy(r => r.TotalQuantitySold)
                    .Take(bottomCount)
                    .ToList();

                return Result<IEnumerable<BookSalesReport>>.Success(bookSalesReport);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<BookSalesReport>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- BÁO CÁO NHÂN VIÊN -------------------------
        // ==================================================================

        /// <summary>
        /// Lấy báo cáo hiệu suất tất cả nhân viên trong khoảng ngày
        /// </summary>
        public Result<IEnumerable<StaffPerformanceReport>> GetStaffPerformance(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var allStaff = _staffRepository.GetAll()
                    .Where(s => s.DeletedDate == null)
                    .ToList();

                var staffPerformanceReport = allStaff
                    .Select(staff => GetStaffPerformanceDetail(staff.Id, fromDate, toDate).Data)
                    .Where(r => r != null)
                    .OrderByDescending(r => r.TotalRevenue)
                    .ToList();

                return Result<IEnumerable<StaffPerformanceReport>>.Success(staffPerformanceReport);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<StaffPerformanceReport>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy báo cáo hiệu suất chi tiết của 1 nhân viên
        /// </summary>
        public Result<StaffPerformanceReport> GetStaffPerformanceDetail(string staffId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var staff = _staffRepository.GetById(staffId);
                if (staff == null)
                    return Result<StaffPerformanceReport>.Fail("Nhân viên không tồn tại");

                var orders = _orderRepository.Find(o =>
                    o.StaffId == staffId &&
                    o.CreatedDate >= fromDate &&
                    o.CreatedDate <= toDate &&
                    o.DeletedDate == null);

                int totalOrders = orders.Count();
                decimal totalRevenue = orders.Sum(o => o.TotalPrice);
                decimal averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

                // Tính doanh thu trung bình hàng ngày
                var workingDays = (toDate.Date - fromDate.Date).Days + 1;
                decimal dailyAverageRevenue = workingDays > 0 ? totalRevenue / workingDays : 0;

                var report = new StaffPerformanceReport
                {
                    StaffId = staffId,
                    StaffName = staff.Name,
                    TotalOrders = totalOrders,
                    TotalRevenue = totalRevenue,
                    AverageOrderValue = averageOrderValue,
                    DailyAverageRevenue = dailyAverageRevenue
                };

                return Result<StaffPerformanceReport>.Success(report);
            }
            catch (Exception ex)
            {
                return Result<StaffPerformanceReport>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- BÁO CÁO KHO --------------------------------
        // ==================================================================

        /// <summary>
        /// Lấy báo cáo tổng hợp tồn kho
        /// </summary>
        public Result<InventorySummaryReport> GetInventorySummary()
        {
            try
            {
                var stocks = _stockRepository.GetAll();
                var books = _bookRepository.GetAll().Where(b => b.DeletedDate == null).ToList();

                int totalBooks = books.Count();
                int totalQuantity = stocks.Sum(s => s.StockQuantity);
                decimal totalValue = 0;
                int lowStockCount = 0;
                int outOfStockCount = 0;

                foreach (var stock in stocks)
                {
                    var book = books.FirstOrDefault(b => b.BookId == stock.BookId);
                    if (book != null)
                    {
                        totalValue += book.ImportPrice * stock.StockQuantity;
                    }

                    if (stock.StockQuantity == 0)
                        outOfStockCount++;
                    else if (stock.StockQuantity <= 5)
                        lowStockCount++;
                }

                var report = new InventorySummaryReport
                {
                    TotalBooks = totalBooks,
                    TotalQuantity = totalQuantity,
                    TotalValue = totalValue,
                    LowStockCount = lowStockCount,
                    OutOfStockCount = outOfStockCount
                };

                return Result<InventorySummaryReport>.Success(report);
            }
            catch (Exception ex)
            {
                return Result<InventorySummaryReport>.Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Tính tổng giá trị tồn kho
        /// </summary>
        public Result<decimal> GetInventoryValue()
        {
            try
            {
                var stocks = _stockRepository.GetAll();
                decimal totalValue = 0;

                foreach (var stock in stocks)
                {
                    var book = _bookRepository.GetById(stock.BookId);
                    if (book != null)
                    {
                        totalValue += book.ImportPrice * stock.StockQuantity;
                    }
                }

                return Result<decimal>.Success(totalValue);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- BÁO CÁO KHÁCH HÀNG ------------------------
        // ==================================================================

        /// <summary>
        /// Lấy top khách hàng chi tiêu nhiều nhất
        /// </summary>
        public Result<IEnumerable<CustomerSpendingReport>> GetTopSpendingCustomers(int topCount = 10)
        {
            try
            {
                var customers = _customerRepository.GetAll()
                    .Where(c => c.DeletedDate == null)
                    .ToList();

                var customerSpendingReport = customers
                    .Select(customer =>
                    {
                        var orders = _orderRepository.Find(o =>
                            o.CustomerId == customer.CustomerId &&
                            o.DeletedDate == null);

                        return new CustomerSpendingReport
                        {
                            CustomerId = customer.CustomerId,
                            CustomerName = customer.Name,
                            TotalSpent = orders.Sum(o => o.TotalPrice),
                            TotalOrders = orders.Count(),
                            AverageOrderValue = orders.Any() ? orders.Average(o => o.TotalPrice) : 0
                        };
                    })
                    .Where(r => r.TotalSpent > 0)
                    .OrderByDescending(r => r.TotalSpent)
                    .Take(topCount)
                    .ToList();

                return Result<IEnumerable<CustomerSpendingReport>>.Success(customerSpendingReport);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<CustomerSpendingReport>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Tính số lượng khách hàng mới trong khoảng ngày
        /// </summary>
        public Result<int> GetNewCustomersCount(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var newCustomers = _customerRepository.Find(c =>
                    c.CreatedDate >= fromDate &&
                    c.CreatedDate <= toDate &&
                    c.DeletedDate == null).Count();

                return Result<int>.Success(newCustomers);
            }
            catch (Exception ex)
            {
                return Result<int>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- BÁO CÁO NHÀ CUNG CẤP ----------------------
        // ==================================================================

        /// <summary>
        /// Lấy báo cáo nhập hàng theo nhà cung cấp
        /// </summary>
        public Result<IEnumerable<SupplierImportReport>> GetSupplierImportReport(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var importBills = _importBillRepository.Find(ib =>
                    ib.ImportDate >= fromDate &&
                    ib.ImportDate <= toDate &&
                    ib.DeletedDate == null);

                var supplierImportReport = importBills
                    .GroupBy(ib => ib.SupplierId)
                    .Select(g =>
                    {
                        var supplier = g.FirstOrDefault()?.Supplier;
                        var details = g.SelectMany(ib => ib.ImportBillDetails).ToList();

                        return new SupplierImportReport
                        {
                            SupplierId = g.Key,
                            SupplierName = supplier?.Name ?? "Unknown",
                            TotalImportBills = g.Count(),
                            TotalImportValue = g.Sum(ib => ib.TotalAmount),
                            TotalBooksImported = details.Sum(d => d.Quantity)
                        };
                    })
                    .OrderByDescending(r => r.TotalImportValue)
                    .ToList();

                return Result<IEnumerable<SupplierImportReport>>.Success(supplierImportReport);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<SupplierImportReport>>.Fail($"Lỗi: {ex.Message}");
            }
        }
    }
}