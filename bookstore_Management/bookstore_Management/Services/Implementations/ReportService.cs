using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.DTOs.Common.Reports;
using bookstore_Management.Models;
using bookstore_Management.Services.Interfaces;

namespace bookstore_Management.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        internal ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Báo cáo doanh thu
        public async Task<Result<decimal>> GetTotalRevenueAsync(DateTime fromDate, DateTime toDate)
        {
            var orders = await _unitOfWork.Orders.FindAsync(o =>
                o.CreatedDate >= fromDate &&
                o.CreatedDate <= toDate &&
                o.DeletedDate == null);
            
            return Result<decimal>.Success(orders.Sum(o => o.TotalPrice));
        }

        public async Task<Result<decimal>> GetTotalProfitAsync(DateTime fromDate, DateTime toDate)
        {
            var ordersTask = _unitOfWork.Orders.FindAsync(o =>
                o.CreatedDate >= fromDate &&
                o.CreatedDate <= toDate &&
                o.DeletedDate == null);

            var importsTask = _unitOfWork.ImportBills.FindAsync(i =>
                i.CreatedDate >= fromDate &&
                i.CreatedDate <= toDate &&
                i.DeletedDate == null);

            await Task.WhenAll(ordersTask, importsTask);

            var totalSales = ordersTask.Result.Select(o => o.TotalPrice).DefaultIfEmpty(0).Sum();
            var totalImport = importsTask.Result.Select(i => i.TotalAmount).DefaultIfEmpty(0).Sum();

            return Result<decimal>.Success(totalSales - totalImport);
        }


        public Result<IEnumerable<decimal>> GetRevenue(DateTime fromDate, DateTime toDate, int jump = 1)
        {
            var revenue = new List<decimal>();

            while (fromDate <= toDate)
            {
                var endDate = fromDate.AddDays(jump);

                var total = _unitOfWork.Orders.Query(o =>
                        o.DeletedDate == null &&
                        o.CreatedDate >= fromDate &&
                        o.CreatedDate <= endDate
                    )
                    .Sum(o => o.TotalPrice);

                revenue.Add(total);

                fromDate = endDate;
            }

            return Result<IEnumerable<decimal>>.Success(revenue);
        }



        public Result<IEnumerable<decimal>> GetImport(DateTime fromDate, DateTime toDate, int jump = 1)
        {
            var import = new List<decimal>();

            while (fromDate <= toDate)
            {
                var total = _unitOfWork.ImportBills.Query( o =>
                        
                        o.DeletedDate == null &&
                        o.CreatedDate >= fromDate &&
                        o.CreatedDate <= toDate )
                    .Sum(o => o.TotalAmount);
                import.Add(total);
                fromDate = fromDate.AddDays(jump);
            }
            return Result<IEnumerable<decimal>>.Success(import);  
        }
        

        // Báo cáo đơn hàng
        public async Task<Result<int>> GetTotalOrderCountAsync(DateTime fromDate, DateTime toDate)
        {
            var count = await _unitOfWork.Orders.CountAsync(o =>
                o.CreatedDate >= fromDate &&
                o.CreatedDate <= toDate &&
                o.DeletedDate == null);
            
            return Result<int>.Success(count);
        }

        // Báo cáo khách hàng mới
        public async Task<Result<int>> GetTotalCustomerCountAsync(DateTime fromDate, DateTime toDate)
        {
            var count = await _unitOfWork.Customers.CountAsync(c =>
                c.CreatedDate >= fromDate &&
                c.CreatedDate <= toDate &&
                c.DeletedDate == null);
            
            return Result<int>.Success(count);
        }
        

        // Báo cáo sách bán chạy
        public async Task<Result<IEnumerable<BookSalesReportResponseDto>>> GetTopSellingBooksAsync(
            DateTime fromDate, DateTime toDate, int topCount = 10)
        {
            var details = await _unitOfWork.OrderDetails.FindAsync(od =>
                od.Order.CreatedDate >= fromDate &&
                od.Order.CreatedDate <= toDate &&
                od.Order.DeletedDate == null);

            var allBooks = await _unitOfWork.Books.GetAllAsync();
            var bookDict = allBooks
                .Where(b => b.DeletedDate == null)
                .ToDictionary(b => b.BookId, b => b.Name);

            var result = details
                .GroupBy(od => od.BookId)
                .Select(g => new BookSalesReportResponseDto
                {
                    BookId = g.Key,
                    BookName = bookDict.TryGetValue(g.Key, out var name) ? name : "Unknown",
                    TotalQuantitySold = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.SalePrice * x.Quantity),
                    AveragePricePerUnit = g.Average(x => x.SalePrice)
                })
                .OrderByDescending(x => x.TotalQuantitySold)
                .Take(topCount)
                .ToList();

            return Result<IEnumerable<BookSalesReportResponseDto>>.Success(result);
        }

        public async Task<Result<IEnumerable<BookSalesReportResponseDto>>> GetLowestSellingBooksAsync(
            DateTime fromDate, DateTime toDate, int bottomCount = 5)
        {
            var details = await _unitOfWork.OrderDetails.FindAsync(od =>
                od.Order.CreatedDate >= fromDate &&
                od.Order.CreatedDate <= toDate &&
                od.Order.DeletedDate == null);

            var allBooks = await _unitOfWork.Books.GetAllAsync();
            var booksList = allBooks.Where(b => b.DeletedDate == null).ToList();
            var detailsList = details.ToList();

            var report = booksList
                .GroupJoin(detailsList,
                    b => b.BookId,
                    d => d.BookId,
                    (book, det) => new
                    {
                        book,
                        det = det.ToList()
                    })
                .Select(x => new BookSalesReportResponseDto
                {
                    BookId = x.book.BookId,
                    BookName = x.book.Name,
                    TotalQuantitySold = x.det.Sum(d => d.Quantity),
                    TotalRevenue = x.det.Sum(d => d.SalePrice * d.Quantity),
                    AveragePricePerUnit = x.det.Any() ? x.det.Average(d => d.SalePrice) : 0
                })
                .OrderBy(r => r.TotalQuantitySold)
                .Take(bottomCount)
                .ToList();

            return Result<IEnumerable<BookSalesReportResponseDto>>.Success(report);
        }

        // Báo cáo kho
        public async Task<Result<InventorySummaryReportResponseDto>> GetInventorySummaryAsync()
        {
            var allBooks = await _unitOfWork.Books.GetAllAsync();
            var books = allBooks.Where(b => b.DeletedDate == null).ToList();

            var totalBooks = books.Count;
            var totalQuantity = books.Sum(b => b.Stock);

            var bookIds = books.Select(b => b.BookId).ToList();
            var importPrices = await _unitOfWork.ImportBillDetails.GetLatestImportPricesByBookIdsAsync(bookIds);

            var totalValue = books.Sum(b =>
                (importPrices.TryGetValue(b.BookId, out var price) ? price ?? 0 : 0) * b.Stock
            );

            var lowStockCount = books.Count(b => b.Stock > 0 && b.Stock < 13);
            var outOfStockCount = books.Count(b => b.Stock == 0);

            var report = new InventorySummaryReportResponseDto
            {
                TotalBooks = totalBooks,
                TotalQuantity = totalQuantity,
                TotalValue = totalValue,
                LowStockCount = lowStockCount,
                OutOfStockCount = outOfStockCount
            };
            
            return Result<InventorySummaryReportResponseDto>.Success(report);
        }

        public async Task<Result<decimal>> GetInventoryValueAsync()
        {
            var summary = await GetInventorySummaryAsync();
            return (!summary.IsSuccess) ?
                Result<decimal>.Fail(summary.ErrorMessage) :
                Result<decimal>.Success(summary.Data.TotalValue);
        }

        // Báo cáo khách hàng
        public async Task<Result<IEnumerable<CustomerSpendingReportResponseDto>>> GetTopSpendingCustomersAsync(int topCount = 10)
        {
            var customersTask = _unitOfWork.Customers.GetAllAsync();
            var ordersTask = _unitOfWork.Orders.GetAllAsync();

            await Task.WhenAll(customersTask, ordersTask);

            var customers = customersTask.Result.Where(c => c.DeletedDate == null).ToList();
            var orders = ordersTask.Result.Where(o => o.DeletedDate == null).ToList();

            var report = customers
                .Select(c =>
                {
                    var customerOrders = orders.Where(o => o.CustomerId == c.CustomerId).ToList();
                    var totalSpent = customerOrders.Sum(o => o.TotalPrice);
                    return new CustomerSpendingReportResponseDto
                    {
                        CustomerId = c.CustomerId,
                        CustomerName = c.Name,
                        TotalSpent = totalSpent,
                        TotalOrders = customerOrders.Count,
                        AverageOrderValue = customerOrders.Any() ? customerOrders.Average(o => o.TotalPrice) : 0
                    };
                })
                .OrderByDescending(r => r.TotalSpent)
                .Take(topCount)
                .ToList();

            return Result<IEnumerable<CustomerSpendingReportResponseDto>>.Success(report);
        }

        // Báo cáo tỉ lệ khách hàng vãng lai và thân thiết
        public async Task<Result<CustomerPurchaseRatioDto>> GetWalkInToMemberPurchaseRatioAsync(
            DateTime fromDate, DateTime toDate)
        {
            var orders = await _unitOfWork.Orders.FindAsync(o =>
                o.CreatedDate >= fromDate &&
                o.CreatedDate <= toDate &&
                o.DeletedDate == null);

            var ordersList = orders.ToList();
            var walkIn = ordersList.Count(o => o.CustomerId == null);
            var member = ordersList.Count(o => o.CustomerId != null);
            var total = member + walkIn;

            var dto = new CustomerPurchaseRatioDto
            {
                WalkIn = walkIn,
                Member = member,
                WalkInRatio = total == 0 ? 0 : (double)walkIn / total,
                MemberRatio = total == 0 ? 0 : (double)member / total
            };

            return Result<CustomerPurchaseRatioDto>.Success(dto);
        }
        
        

        // Báo cáo nhà cung cấp
        public async Task<Result<IEnumerable<PublisherImportReportResponseDto>>> GetPublisherImportReportAsync(
            DateTime fromDate, DateTime toDate)
        {
            var bills = await _unitOfWork.ImportBills.FindAsync(ib =>
                ib.CreatedDate >= fromDate &&
                ib.CreatedDate <= toDate &&
                ib.DeletedDate == null);

            var report = bills
                .GroupBy(ib => ib.PublisherId)
                .Select(g =>
                {
                    var supplier = g.FirstOrDefault()?.Publisher;
                    var totalQty = g.SelectMany(x => x.ImportBillDetails ?? Enumerable.Empty<ImportBillDetail>())
                        .Where(d => d.DeletedDate == null)
                        .Sum(d => d.Quantity);
                    var totalValue = g.Sum(x => x.TotalAmount);
                    return new PublisherImportReportResponseDto
                    {
                        PublisherId = g.Key,
                        PublisherName = supplier?.Name ?? "Unknown",
                        TotalQuantity = totalQty,
                        TotalImportValue = totalValue
                    };
                })
                .OrderByDescending(r => r.TotalImportValue)
                .ToList();

            return Result<IEnumerable<PublisherImportReportResponseDto>>.Success(report);
        }
        
    }
} 