using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.DTOs.Order.Requests;
using bookstore_Management.DTOs.Order.Responses;
using bookstore_Management.Models;
using bookstore_Management.Services.Interfaces;

namespace bookstore_Management.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        internal OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ==================================================================
        // ---------------------- TẠO ĐƠN HÀNG ------------------------------
        // ==================================================================
        public async Task<Result<string>> CreateOrderAsync(CreateOrderRequestDto dto)
        {
            // Validate nhân viên
            var staff = await _unitOfWork.Staffs.GetByIdAsync(dto.StaffId);
            if (staff == null || staff.DeletedDate != null)
                return Result<string>.Fail("Nhân viên không tồn tại");

            // Validate khách hàng (có thể null)
            if (!string.IsNullOrEmpty(dto.CustomerId))
            {
                var customer = await _unitOfWork.Customers.GetByIdAsync(dto.CustomerId);
                if (customer == null || customer.DeletedDate != null)
                    return Result<string>.Fail("Khách hàng không tồn tại");
            }

            if (dto.OrderDetails == null || !dto.OrderDetails.Any())
                return Result<string>.Fail("Đơn hàng phải có ít nhất 1 sách");

            // Batch load all books at once để tránh N+1 queries
            var bookIds = dto.OrderDetails.Select(x => x.BookId).Distinct().ToList();
            var books = await _unitOfWork.Books.FindAsync(b => 
                bookIds.Contains(b.BookId) && b.DeletedDate == null);
            var bookDict = books.ToDictionary(b => b.BookId);

            decimal subtotal = 0;
            var orderDetails = new List<OrderDetail>();

            foreach (var item in dto.OrderDetails)
            {
                if (!bookDict.TryGetValue(item.BookId, out var book))
                    return Result<string>.Fail($"Sách {item.BookId} không tồn tại");

                if (!book.SalePrice.HasValue)
                    return Result<string>.Fail($"Sách {book.Name} chưa có giá bán");

                if (item.Quantity <= 0)
                    return Result<string>.Fail($"Số lượng sách {book.Name} phải > 0");

                var available = book.Stock;
                if (available < item.Quantity)
                    return Result<string>.Fail($"Sách {book.Name} không đủ hàng. Tồn: {available}");
                
                book.Stock -= item.Quantity;
                book.UpdatedDate = DateTime.Now;
                _unitOfWork.Books.Update(book);
                
                var lineTotal = book.SalePrice.Value * item.Quantity;
                subtotal += lineTotal;

                orderDetails.Add(new OrderDetail
                {
                    OrderId = "", // set sau khi tạo mã
                    BookId = item.BookId,
                    SalePrice = book.SalePrice.Value,
                    Quantity = item.Quantity,
                    Subtotal = lineTotal,
                    Notes = string.IsNullOrWhiteSpace(item.Notes) ? null : item.Notes.Trim()
                });
            }

            if (dto.Discount < 0)
                return Result<string>.Fail("Giảm giá không được âm");
            if (dto.Discount > 1)
                return Result<string>.Fail("Giảm giá không được lớn hơn 1");

            var finalTotal = subtotal * (1 - dto.Discount);

            // Sinh mã đơn hàng
            var orderId = await GenerateOrderIdAsync();

            // Gán OrderId cho chi tiết
            foreach (var detail in orderDetails)
            {
                detail.OrderId = orderId;
            }

            var order = new Order
            {
                OrderId = orderId,
                StaffId = dto.StaffId,
                CustomerId = string.IsNullOrWhiteSpace(dto.CustomerId) ? null : dto.CustomerId,
                PaymentMethod = dto.PaymentMethod,
                Discount = dto.Discount,
                TotalPrice = finalTotal,
                Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim(),
                CreatedDate = DateTime.Now,
                OrderDetails = orderDetails
            };

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Success(orderId, $"Tạo đơn hàng thành công. Tổng tiền: {finalTotal:N0} VND");
        }

        // ==================================================================
        // ----------------------- CẬP NHẬT / HỦY ---------------------------
        // ==================================================================
        public async Task<Result> UpdateOrderAsync(string orderId, UpdateOrderRequestDto dto)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null || order.DeletedDate != null)
                return Result.Fail("Đơn hàng không tồn tại");

            if (!string.IsNullOrWhiteSpace(dto.Notes))
                order.Notes = dto.Notes.Trim();

            if (dto.PaymentMethod.HasValue)
                order.PaymentMethod = dto.PaymentMethod.Value;

            if (dto.Discount.HasValue)
            {
                if (dto.Discount < 0)
                    return Result.Fail("Giảm giá không được âm");

                var orderDetails = await _unitOfWork.OrderDetails.FindAsync(od => od.OrderId == orderId);
                var subtotal = orderDetails.Sum(od => od.Subtotal);
                
                if (dto.Discount > 1)
                    return Result.Fail("Giảm giá không được lớn hơn 100%");

                order.Discount = dto.Discount.Value;
                order.TotalPrice = subtotal * (1 - dto.Discount.Value);
            }

            order.UpdatedDate = DateTime.Now;
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success("Cập nhật đơn hàng thành công");
        }

        public async Task<Result> DeleteOrderAsync(string orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null || order.DeletedDate != null)
                return Result.Fail("Đơn hàng không tồn tại");

            // Batch load all books để tránh N+1 queries
            var bookIds = order.OrderDetails.Select(od => od.BookId).Distinct().ToList();
            var books = await _unitOfWork.Books.FindAsync(b => bookIds.Contains(b.BookId));
            var bookDict = books.ToDictionary(b => b.BookId);

            foreach (var detail in order.OrderDetails)
            {
                if (bookDict.TryGetValue(detail.BookId, out var book))
                {
                    book.Stock += detail.Quantity;
                    book.UpdatedDate = DateTime.Now;
                    _unitOfWork.Books.Update(book);
                }
            }
            
            order.DeletedDate = DateTime.Now;
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success("Hủy đơn hàng thành công");
        }

        // ==================================================================
        // ----------------------- TRUY VẤN ---------------------------------
        // ==================================================================
        public async Task<Result<OrderResponseDto>> GetOrderByIdAsync(string orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null || order.DeletedDate != null)
                return Result<OrderResponseDto>.Fail("Đơn hàng không tồn tại");

            var dto = MapToOrderResponseDto(order);
            return Result<OrderResponseDto>.Success(dto);
        }

        public async Task<Result<IEnumerable<OrderResponseDto>>> GetAllOrdersAsync()
        {
            var allOrders = await _unitOfWork.Orders.GetAllAsync();
            var orders = allOrders
                .Where(o => o.DeletedDate == null)
                .OrderByDescending(o => o.CreatedDate)
                .Select(MapToOrderResponseDto)
                .ToList();

            return Result<IEnumerable<OrderResponseDto>>.Success(orders);
        }

        public async Task<Result<IEnumerable<OrderResponseDto>>> GetOrdersByCustomerAsync(string customerId)
        {
            var allOrders = await _unitOfWork.Orders.GetByCustomerAsync(customerId);
            var orders = allOrders
                .Where(o => o.DeletedDate == null)
                .OrderByDescending(o => o.CreatedDate)
                .Select(MapToOrderResponseDto)
                .ToList();

            return Result<IEnumerable<OrderResponseDto>>.Success(orders);
        }
        
        public async Task<Result<IEnumerable<OrderResponseDto>>> SearchOrderBillsAsync(string keyword)
        {
            keyword = keyword?.Trim().ToLower() ?? "";

            var list = await _unitOfWork.Orders
                .Query(o => o.DeletedDate == null &&
                            o.OrderId.ToString().ToLower().Contains(keyword))
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .Include(o => o.OrderDetails)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();

            return list.Select(MapToOrderResponseDto).ToList();
        }


        public async Task<Result<IEnumerable<OrderResponseDto>>> GetOrdersByStaffAsync(string staffId)
        {
            var allOrders = await _unitOfWork.Orders.GetByStaffAsync(staffId);
            var orders = allOrders
                .Where(o => o.DeletedDate == null)
                .OrderByDescending(o => o.CreatedDate)
                .Select(MapToOrderResponseDto)
                .ToList();

            return Result<IEnumerable<OrderResponseDto>>.Success(orders);
        }

        public async Task<Result<IEnumerable<OrderResponseDto>>> GetOrdersByDateAsync(DateTime fromDate, DateTime toDate)
        {
            var allOrders = await _unitOfWork.Orders.GetByDateRangeAsync(fromDate, toDate);
            var orders = allOrders
                .Where(o => o.DeletedDate == null)
                .OrderByDescending(o => o.CreatedDate)
                .Select(MapToOrderResponseDto)
                .ToList();

            return Result<IEnumerable<OrderResponseDto>>.Success(orders);
        }

        public async Task<Result<IEnumerable<OrderDetailResponseDto>>> GetOrderDetailsAsync(string orderId)
        {
            var allDetails = await _unitOfWork.OrderDetails.GetByOrderAsync(orderId);
            
            // Batch load all books để tránh N+1 queries
            var bookIds = allDetails.Select(d => d.BookId).Distinct().ToList();
            var books = await _unitOfWork.Books.FindAsync(b => bookIds.Contains(b.BookId));
            var bookDict = books.ToDictionary(b => b.BookId);

            var detailsList = allDetails.Select(d => new OrderDetailResponseDto
            {
                OrderId = d.OrderId,
                BookId = d.BookId,
                BookName = bookDict.TryGetValue(d.BookId, out var book) ? book.Name : "Unknown",
                SalePrice = d.SalePrice,
                Quantity = d.Quantity,
                Subtotal = d.Subtotal,
                Notes = d.Notes
            }).ToList();

            return Result<IEnumerable<OrderDetailResponseDto>>.Success(detailsList);
        }

        // ==================================================================
        // ----------------------- HÀM PHỤ TRỢ ------------------------------
        // ==================================================================
        private async Task<string> GenerateOrderIdAsync()
        {
            var allOrders = await _unitOfWork.Orders.GetAllAsync();
            var lastOrder = allOrders
                .OrderByDescending(o => o.OrderId)
                .FirstOrDefault();

            if (lastOrder == null || !lastOrder.OrderId.StartsWith("HD"))
                return "HD0001";

            var lastNumber = int.Parse(lastOrder.OrderId.Substring(2));
            return $"HD{(lastNumber + 1):D4}";
        }

        /// <summary>
        /// Maps Order entity to OrderResponseDto
        /// </summary>
        private OrderResponseDto MapToOrderResponseDto(Order order)
        {
            return new OrderResponseDto
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer?.Name,
                StaffId = order.StaffId,
                StaffName = order.Staff?.Name,
                CreatedDate = order.CreatedDate,
                TotalPrice = order.TotalPrice,
                Discount = order.Discount,
                PaymentMethod = order.PaymentMethod,
                Notes = order.Notes
            };
        }
    }
}