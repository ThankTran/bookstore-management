// using System;
// using System.Collections.Generic;
// using System.Linq;
// using bookstore_Management.Core.Results;
// using bookstore_Management.Data.Repositories.Interfaces;
// using bookstore_Management.DTOs.Common.Reports;
// using bookstore_Management.DTOs.Book.Shared;
// using bookstore_Management.Services.Interfaces;
//
// namespace bookstore_Management.Services.Implementations
// {
//     public class ReportService : IReportService
//     {
//         private readonly IOrderRepository _orderRepository;
//         private readonly IOrderDetailRepository _orderDetailRepository;
//         private readonly IStockRepository _stockRepository;
//         private readonly IBookRepository _bookRepository;
//         private readonly ICustomerRepository _customerRepository;
//         private readonly IStaffRepository _staffRepository;
//         private readonly IImportBillRepository _importBillRepository;
//         private readonly IImportBillDetailRepository _importBillDetailRepository;
//
//         internal ReportService(
//             IOrderRepository orderRepository,
//             IOrderDetailRepository orderDetailRepository,
//             IStockRepository stockRepository,
//             IBookRepository bookRepository,
//             ICustomerRepository customerRepository,
//             IStaffRepository staffRepository,
//             IImportBillRepository importBillRepository,
//             IImportBillDetailRepository importBillDetailRepository)
//         {
//             _orderRepository = orderRepository;
//             _orderDetailRepository = orderDetailRepository;
//             _stockRepository = stockRepository;
//             _bookRepository = bookRepository;
//             _customerRepository = customerRepository;
//             _staffRepository = staffRepository;
//             _importBillRepository = importBillRepository;
//             _importBillDetailRepository = importBillDetailRepository;
//         }
//
//         public Result<decimal> GetTotalRevenue(DateTime fromDate, DateTime toDate)
//         {
//             try
//             {
//                 var orders = _orderRepository.Find(o =>
//                     o.CreatedDate >= fromDate &&
//                     o.CreatedDate <= toDate &&
//                     o.DeletedDate == null);
//                 return Result<decimal>.Success(orders.Sum(o => o.TotalPrice));
//             }
//             catch (Exception ex)
//             {
//                 return Result<decimal>.Fail($"Lỗi: {ex.Message}");
//             }
//         }
//
//         public Result<decimal> GetTotalProfit(DateTime fromDate, DateTime toDate)
//         {
//             try
//             {
//                 // Lợi nhuận = (giá bán tại thời điểm bán - giá nhập hiện tại) * số lượng
//                 // Lưu ý: OrderDetail.SalePrice lưu đúng giá bán tại thời điểm đó.
//                 var details = _orderDetailRepository.Find(od =>
//                     od.Order.CreatedDate >= fromDate &&
//                     od.Order.CreatedDate <= toDate &&
//                     od.Order.DeletedDate == null);
//
//                 decimal totalProfit = 0;
//                 foreach (var d in details)
//                 {
//                     // Get import price from ImportBillDetail, not from Book model
//                     var importPrice = _importBillDetailRepository.GetImportPriceByBookId(d.BookId);
//                     if (importPrice <= 0) continue; // Skip if no import price found
//                     totalProfit += (d.SalePrice - importPrice) * d.Quantity;
//                 }
//
//                 return Result<decimal>.Success(totalProfit);
//             }
//             catch (Exception ex)
//             {
//                 return Result<decimal>.Fail($"Lỗi: {ex.Message}");
//             }
//         }
//
//         public Result<decimal> GetAverageOrderValue(DateTime fromDate, DateTime toDate)
//         {
//             try
//             {
//                 var orders = _orderRepository.Find(o =>
//                     o.CreatedDate >= fromDate &&
//                     o.CreatedDate <= toDate &&
//                     o.DeletedDate == null);
//                 if (!orders.Any())
//                     return Result<decimal>.Success(0, "Không có đơn hàng");
//                 return Result<decimal>.Success(orders.Average(o => o.TotalPrice));
//             }
//             catch (Exception ex)
//             {
//                 return Result<decimal>.Fail($"Lỗi: {ex.Message}");
//             }
//         }
//
//         public Result<int> GetTotalOrderCount(DateTime fromDate, DateTime toDate)
//         {
//             try
//             {
//                 var count = _orderRepository.Find(o =>
//                     o.CreatedDate >= fromDate &&
//                     o.CreatedDate <= toDate &&
//                     o.DeletedDate == null).Count();
//                 return Result<int>.Success(count);
//             }
//             catch (Exception ex)
//             {
//                 return Result<int>.Fail($"Lỗi: {ex.Message}");
//             }
//         }
//
//         public Result<decimal> GetTotalDiscountGiven(DateTime fromDate, DateTime toDate)
//         {
//             try
//             {
//                 var orders = _orderRepository.Find(o =>
//                     o.CreatedDate >= fromDate &&
//                     o.CreatedDate <= toDate &&
//                     o.DeletedDate == null);
//                 return Result<decimal>.Success(orders.Sum(o => o.Discount));
//             }
//             catch (Exception ex)
//             {
//                 return Result<decimal>.Fail($"Lỗi: {ex.Message}");
//             }
//         }
//
//         public Result<IEnumerable<BookSalesReportResponseDto>> GetTopSellingBooks(DateTime fromDate, DateTime toDate, int topCount = 10)
//         {
//             try
//             {
//                 var details = _orderDetailRepository.Find(od =>
//                     od.Order.CreatedDate >= fromDate &&
//                     od.Order.CreatedDate <= toDate &&
//                     od.Order.DeletedDate == null);
//
//                 var result = details
//                     .GroupBy(od => od.BookId)
//                     .Select(g => new BookSalesReportResponseDto
//                     {
//                         BookId = g.Key,
//                         BookName = _bookRepository.GetById(g.Key)?.Name ?? "Unknown",
//                         TotalQuantitySold = g.Sum(x => x.Quantity),
//                         TotalRevenue = g.Sum(x => x.SalePrice * x.Quantity),
//                         AveragePricePerUnit = g.Average(x => x.SalePrice)
//                     })
//                     .OrderByDescending(x => x.TotalQuantitySold)
//                     .Take(topCount)
//                     .ToList();
//
//                 return Result<IEnumerable<BookSalesReportResponseDto>>.Success(result);
//             }
//             catch (Exception ex)
//             {
//                 return Result<IEnumerable<BookSalesReportResponseDto>>.Fail($"Lỗi: {ex.Message}");
//             }
//         }
//
//         public Result<IEnumerable<BookSalesReportResponseDto>> GetLowestSellingBooks(DateTime fromDate, DateTime toDate, int bottomCount = 5)
//         {
//             try
//             {
//                 var details = _orderDetailRepository.Find(od =>
//                     od.Order.CreatedDate >= fromDate &&
//                     od.Order.CreatedDate <= toDate &&
//                     od.Order.DeletedDate == null);
//
//                 var allBooks = _bookRepository.GetAll().Where(b => b.DeletedDate == null).ToList();
//
//                 var report = allBooks
//                     .GroupJoin(details,
//                         b => b.BookId,
//                         d => d.BookId,
//                         (book, det) => new
//                         {
//                             book,
//                             det = det.ToList()
//                         })
//                     .Select(x => new BookSalesReportResponseDto
//                     {
//                         BookId = x.book.BookId,
//                         BookName = x.book.Name,
//                         TotalQuantitySold = x.det.Sum(d => d.Quantity),
//                         TotalRevenue = x.det.Sum(d => d.SalePrice * d.Quantity),
//                         AveragePricePerUnit = x.det.Any() ? x.det.Average(d => d.SalePrice) : 0
//                     })
//                     .OrderBy(r => r.TotalQuantitySold)
//                     .Take(bottomCount)
//                     .ToList();
//
//                 return Result<IEnumerable<BookSalesReportResponseDto>>.Success(report);
//             }
//             catch (Exception ex)
//             {
//                 return Result<IEnumerable<BookSalesReportResponseDto>>.Fail($"Lỗi: {ex.Message}");
//             }
//         }
//
//         public Result<IEnumerable<StaffPerformanceReportResponseDto>> GetStaffPerformance(DateTime fromDate, DateTime toDate)
//         {
//             try
//             {
//                 var staff = _staffRepository.GetAll().Where(s => s.DeletedDate == null).ToList();
//                 var orders = _orderRepository.Find(o =>
//                     o.CreatedDate >= fromDate &&
//                     o.CreatedDate <= toDate &&
//                     o.DeletedDate == null);
//
//                 var report = staff
//                     .Select(s =>
//                     {
//                         var own = orders.Where(o => o.StaffId == s.Id);
//                         var totalOrders = own.Count();
//                         var revenue = own.Sum(o => o.TotalPrice);
//                         return new StaffPerformanceReportResponseDto
//                         {
//                             StaffId = s.Id,
//                             StaffName = s.Name,
//                             TotalOrders = totalOrders,
//                             TotalRevenue = revenue,
//                             AverageOrderValue = totalOrders > 0 ? revenue / totalOrders : 0,
//                             DailyAverageRevenue = revenue / Math.Max(1, (toDate.Date - fromDate.Date).Days + 1)
//                         };
//                     })
//                     .OrderByDescending(r => r.TotalRevenue)
//                     .ToList();
//
//                 return Result<IEnumerable<StaffPerformanceReportResponseDto>>.Success(report);
//             }
//             catch (Exception ex)
//             {
//                 return Result<IEnumerable<StaffPerformanceReportResponseDto>>.Fail($"Lỗi: {ex.Message}");
//             }
//         }
//
//         public Result<StaffPerformanceReportResponseDto> GetStaffPerformanceDetail(string staffId, DateTime fromDate, DateTime toDate)
//         {
//             var all = GetStaffPerformance(fromDate, toDate);
//             if (!all.IsSuccess) return Result<StaffPerformanceReportResponseDto>.Fail(all.ErrorMessage);
//             var item = all.Data.FirstOrDefault(r => r.StaffId == staffId);
//             if (item == null) return Result<StaffPerformanceReportResponseDto>.Fail("Nhân viên không tồn tại hoặc không có dữ liệu");
//             return Result<StaffPerformanceReportResponseDto>.Success(item);
//         }
//
//         public Result<InventorySummaryReportResponseDto> GetInventorySummary()
//         {
//             try
//             {
//                 var stocks = _stockRepository.GetAll().Where(s => s.DeletedDate == null).ToList();
//                 var books = _bookRepository.GetAll().Where(b => b.DeletedDate == null).ToList();
//
//                 int totalBooks = books.Count;
//                 int totalQuantity = stocks.Sum(s => s.StockQuantity);
//
//                 // Giá trị tồn kho nên tính theo giá vốn (ImportPrice), không phải giá bán (SalePrice).
//                 // Get import prices for all books in batch
//                 var bookIds = books.Select(b => b.BookId).ToList();
//                 var importPrices = _importBillDetailRepository.GetLatestImportPricesByBookIds(bookIds);
//                 
//                 decimal totalValue = books.Sum(b =>
//                     (importPrices.ContainsKey(b.BookId) ? importPrices[b.BookId] ?? 0 : 0) * 
//                     stocks.Where(s => s.BookId == b.BookId).Sum(s => s.StockQuantity));
//
//                 int lowStockCount = stocks.Count(s => s.StockQuantity > 0 && s.StockQuantity <= 5);
//                 int outOfStockCount = stocks.Count(s => s.StockQuantity == 0);
//
//                 var report = new InventorySummaryReportResponseDto
//                 {
//                     TotalBooks = totalBooks,
//                     TotalQuantity = totalQuantity,
//                     TotalValue = totalValue,
//                     LowStockCount = lowStockCount,
//                     OutOfStockCount = outOfStockCount
//                 };
//                 return Result<InventorySummaryReportResponseDto>.Success(report);
//             }
//             catch (Exception ex)
//             {
//                 return Result<InventorySummaryReportResponseDto>.Fail($"Lỗi: {ex.Message}");
//             }
//         }
//
//         public Result<decimal> GetInventoryValue()
//         {
//             var summary = GetInventorySummary();
//             if (!summary.IsSuccess) return Result<decimal>.Fail(summary.ErrorMessage);
//             return Result<decimal>.Success(summary.Data.TotalValue);
//         }
//
//         public Result<IEnumerable<CustomerSpendingReportResponseDto>> GetTopSpendingCustomers(int topCount = 10)
//         {
//             try
//             {
//                 var customers = _customerRepository.GetAll().Where(c => c.DeletedDate == null).ToList();
//                 var orders = _orderRepository.GetAll().Where(o => o.DeletedDate == null).ToList();
//
//                 var report = customers
//                     .Select(c =>
//                     {
//                         var cos = orders.Where(o => o.CustomerId == c.CustomerId);
//                         var totalSpent = cos.Sum(o => o.TotalPrice);
//                         return new CustomerSpendingReportResponseDto
//                         {
//                             CustomerId = c.CustomerId,
//                             CustomerName = c.Name,
//                             TotalSpent = totalSpent,
//                             TotalOrders = cos.Count(),
//                             AverageOrderValue = cos.Any() ? cos.Average(o => o.TotalPrice) : 0
//                         };
//                     })
//                     .OrderByDescending(r => r.TotalSpent)
//                     .Take(topCount)
//                     .ToList();
//
//                 return Result<IEnumerable<CustomerSpendingReportResponseDto>>.Success(report);
//             }
//             catch (Exception ex)
//             {
//                 return Result<IEnumerable<CustomerSpendingReportResponseDto>>.Fail($"Lỗi: {ex.Message}");
//             }
//         }
//
//         public Result<int> GetNewCustomersCount(DateTime fromDate, DateTime toDate)
//         {
//             try
//             {
//                 var count = _customerRepository.Find(c =>
//                     c.CreatedDate >= fromDate &&
//                     c.CreatedDate <= toDate &&
//                     c.DeletedDate == null).Count();
//                 return Result<int>.Success(count);
//             }
//             catch (Exception ex)
//             {
//                 return Result<int>.Fail($"Lỗi: {ex.Message}");
//             }
//         }
//
//         public Result<IEnumerable<PublisherImportReportResponseDto>> GetPublisherImportReport(DateTime fromDate, DateTime toDate)
//         {
//             try
//             {
//                 var bills = _importBillRepository.Find(ib =>
//                     ib.CreatedDate >= fromDate &&
//                     ib.CreatedDate <= toDate &&
//                     ib.DeletedDate == null);
//
//                 var report = bills
//                     .GroupBy(ib => ib.PublisherId)
//                     .Select(g =>
//                     {
//                         var supplier = g.FirstOrDefault()?.Publisher;
//                         var totalQty = g.SelectMany(x => x.ImportBillDetails).Sum(d => d.Quantity);
//                         var totalValue = g.Sum(x => x.TotalAmount);
//                         return new PublisherImportReportResponseDto()
//                         {
//                             PublisherId = g.Key,
//                             PublisherName = supplier?.Name ?? "Unknown",
//                             TotalQuantity = totalQty,
//                             TotalImportValue = totalValue
//                         };
//                     })
//                     .OrderByDescending(r => r.TotalImportValue)
//                     .ToList();
//
//                 return Result<IEnumerable<PublisherImportReportResponseDto>>.Success(report);
//             }
//             catch (Exception ex)
//             {
//                 return Result<IEnumerable<PublisherImportReportResponseDto>>.Fail($"Lỗi: {ex.Message}");
//             }
//         }
//     }
// }
