using System;
using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.DTOs.Common.Reports;
using bookstore_Management.Models;
using bookstore_Management.Services.Interfaces;

namespace bookstore_Management.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IBookRepository _bookRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IImportBillRepository _importBillRepository;
        private readonly IImportBillDetailRepository _importBillDetailRepository;

        internal ReportService(
            IOrderRepository orderRepository,
            IOrderDetailRepository orderDetailRepository,
            IBookRepository bookRepository,
            ICustomerRepository customerRepository,
            IImportBillRepository importBillRepository,
            IImportBillDetailRepository importBillDetailRepository)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _bookRepository = bookRepository;
            _customerRepository = customerRepository;
            _importBillRepository = importBillRepository;
            _importBillDetailRepository = importBillDetailRepository;
        }

        public Result<decimal> GetTotalRevenue(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var orders = _orderRepository.Find(o =>
                    o.CreatedDate >= fromDate &&
                    o.CreatedDate <= toDate &&
                    o.DeletedDate == null);
                return Result<decimal>.Success(orders.Sum(o => o.TotalPrice));
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

        
        public Result<decimal> GetTotalProfit(DateTime fromDate, DateTime toDate)
        {
            try
            {
                // Lợi nhuận = (tổng tiền bán - tổng tiền nhập)
                var totalSales = _orderRepository.Find(o =>
                    o.CreatedDate >= fromDate &&
                    o.CreatedDate <= toDate &&
                    o.DeletedDate == null
                ).Select(o => o.TotalPrice).DefaultIfEmpty(0).Sum();

                var totalImport = _importBillRepository.Find(i =>
                    i.CreatedDate >= fromDate &&
                    i.CreatedDate <= toDate &&
                    i.DeletedDate == null
                ).Select(i => i.TotalAmount).DefaultIfEmpty(0).Sum();

                return Result<decimal>.Success(totalSales - totalImport);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

        //public Result<IEnumerable<decimal>> GetRevenue(DateTime fromDate, DateTime toDate, int jump = 1)
        //{
        //    var revenue = new List<decimal>();

        //    while (fromDate <= toDate)
        //    {
        //        var total = _orderRepository.Find(o => o.DeletedDate == null)
        //            .Sum(o => o.TotalPrice);
        //        revenue.Add(total);
        //        fromDate = fromDate.AddDays(jump);
        //    }
        //    return Result<IEnumerable<decimal>>.Success(revenue);  
        //}
        public Result<IEnumerable<decimal>> GetRevenue(DateTime fromDate, DateTime toDate, int jump = 1)
        {
            var revenue = new List<decimal>();

            while (fromDate <= toDate)
            {
                var endDate = fromDate.AddDays(jump);

                var total = _orderRepository.Find(o =>
                        o.DeletedDate == null &&
                        o.CreatedDate >= fromDate &&
                        o.CreatedDate < endDate
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
                var endDate = fromDate.AddDays(jump);

                var total = _importBillRepository.Find(o =>
                        o.DeletedDate == null &&
                        o.CreatedDate >= fromDate &&
                        o.CreatedDate < endDate
                    )
                    .Sum(o => o.TotalAmount);

                import.Add(total);
                fromDate = endDate;
            }

            return Result<IEnumerable<decimal>>.Success(import);
        }

        public Result<int> GetTotalCustomerCount(DateTime fromDate, DateTime toDate)
        {
            try
            {   
                var customer = _customerRepository.Find( c =>
                    c.CreatedDate >= fromDate &&
                    c.CreatedDate <= toDate &&
                    c.DeletedDate == null).Count();
                return Result<int>.Success(customer);
            }
            catch (Exception e)
            {
                return Result<int>.Fail(e.Message);
            }
        }
        
        public Result<decimal> GetAverageOrderValue(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var orders = _orderRepository.Find(o =>
                    o.CreatedDate >= fromDate &&
                    o.CreatedDate <= toDate &&
                    o.DeletedDate == null).ToList();
                return (!orders.Any()) ?
                    Result<decimal>.Success(0, "Không có đơn hàng") :
                    Result<decimal>.Success(orders.Average(o => o.TotalPrice));
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

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
        

        public Result<IEnumerable<BookSalesReportResponseDto>> GetTopSellingBooks(DateTime fromDate, DateTime toDate, int topCount = 10)
        {
            try
            {
                var details = _orderDetailRepository.Find(od =>
                    od.Order.CreatedDate >= fromDate &&
                    od.Order.CreatedDate <= toDate &&
                    od.Order.DeletedDate == null);
                var bookDict = _bookRepository.GetAll()
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
            catch (Exception ex)
            {
                return Result<IEnumerable<BookSalesReportResponseDto>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<BookSalesReportResponseDto>> GetLowestSellingBooks(DateTime fromDate, DateTime toDate, int bottomCount = 5)
        {
            try
            {
                var details = _orderDetailRepository.Find(od =>
                    od.Order.CreatedDate >= fromDate &&
                    od.Order.CreatedDate <= toDate &&
                    od.Order.DeletedDate == null);

                var allBooks = _bookRepository.GetAll().Where(b => b.DeletedDate == null).ToList();

                var report = allBooks
                    .GroupJoin(details,
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
            catch (Exception ex)
            {
                return Result<IEnumerable<BookSalesReportResponseDto>>.Fail($"Lỗi: {ex.Message}");
            }
        }


        public Result<InventorySummaryReportResponseDto> GetInventorySummary()
        {
            try
            {
                var books = _bookRepository.GetAll().Where(b => b.DeletedDate == null).ToList();

                var totalBooks = books.Count;
                var totalQuantity = books.Sum(b => b.Stock);

                // Giá trị tồn kho nên tính theo giá vốn (ImportPrice), không phải giá bán (SalePrice).
                var bookIds = books.Select(b => b.BookId).ToList();
                var importPrices = _importBillDetailRepository.GetLatestImportPricesByBookIds(bookIds);

                var totalValue = books.Sum(b =>
                    (importPrices.TryGetValue(b.BookId, out var price) ? price ?? 0 : 0) * b.Stock
                );


                var outOfStockCount = books.Count(b => b.Stock == 0); 
                var lowStockCount = books.Count(b => b.Stock > 0 && b.Stock < 5);


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
            catch (Exception ex)
            {
                return Result<InventorySummaryReportResponseDto>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<decimal> GetInventoryValue()
        {
            var summary = GetInventorySummary();
            return (!summary.IsSuccess) ?
                Result<decimal>.Fail(summary.ErrorMessage):
                Result<decimal>.Success(summary.Data.TotalValue);
        }

        public Result<IEnumerable<CustomerSpendingReportResponseDto>> GetTopSpendingCustomers(int topCount = 10)
        {
            try
            {
                var customers = _customerRepository.GetAll().Where(c => c.DeletedDate == null).ToList();
                var orders = _orderRepository.GetAll().Where(o => o.DeletedDate == null).ToList();

                var report = customers
                    .Select(c =>
                    {
                        var cos = orders.Where(o => o.CustomerId == c.CustomerId).ToList();
                        var totalSpent = cos.Sum(o => o.TotalPrice);
                        return new CustomerSpendingReportResponseDto
                        {
                            CustomerId = c.CustomerId,
                            CustomerName = c.Name,
                            TotalSpent = totalSpent,
                            TotalOrders = cos.Count(),
                            AverageOrderValue = cos.Any() ? cos.Average(o => o.TotalPrice) : 0
                        };
                    })
                    .OrderByDescending(r => r.TotalSpent)
                    .Take(topCount)
                    .ToList();

                return Result<IEnumerable<CustomerSpendingReportResponseDto>>.Success(report);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<CustomerSpendingReportResponseDto>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<CustomerPurchaseRatioDto> GetWalkInToMemberPurchaseRatio(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var walkIn = 0;
                var member = 0;
                _orderRepository.Find(o =>
                    o.CreatedDate >= fromDate &&
                    o.CreatedDate <= toDate &&
                    o.DeletedDate == null).ToList().ForEach(o =>
                    {
                        if (o.CustomerId == null) walkIn++;
                        else member++;
                    });
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
            catch (Exception ex)
            {
                return Result<CustomerPurchaseRatioDto>.Fail($"Lỗi: {ex.Message}");
            }
        }
        
        


        public Result<IEnumerable<PublisherImportReportResponseDto>> GetPublisherImportReport(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var bills = _importBillRepository.Find(ib =>
                    ib.CreatedDate >= fromDate &&
                    ib.CreatedDate <= toDate &&
                    ib.DeletedDate == null);

                var report = bills
                    .GroupBy(ib => ib.PublisherId)
                    .Select(g =>
                    {
                        var supplier = g.FirstOrDefault()?.Publisher;
                        var totalQty = g.SelectMany(x => x.ImportBillDetails ?? Enumerable.Empty<ImportBillDetail>())
                            .Sum(d => d.Quantity);
                        var totalValue = g.Sum(x => x.TotalAmount);
                        return new PublisherImportReportResponseDto()
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
            catch (Exception ex)
            {
                return Result<IEnumerable<PublisherImportReportResponseDto>>.Fail($"Lỗi: {ex.Message}");
            }
        }
        
    }
}
