using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IBookRepository _bookRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IStaffRepository _staffRepository;

        internal OrderService(
            IOrderRepository orderRepository,
            IOrderDetailRepository orderDetailRepository,
            IBookRepository bookRepository,
            ICustomerRepository customerRepository,
            IStaffRepository staffRepository)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _bookRepository = bookRepository;
            _customerRepository = customerRepository;
            _staffRepository = staffRepository;
        }

        // ==================================================================
        // ---------------------- TẠO ĐƠN HÀNG ------------------------------
        // ==================================================================
        public Result<string> CreateOrder(CreateOrderRequestDto dto)
        {
            try
            {
                // Validate nhân viên
                var staff = _staffRepository.GetById(dto.StaffId);
                if (staff == null || staff.DeletedDate != null)
                    return Result<string>.Fail("Nhân viên không tồn tại");

                // Validate khách hàng (có thể null)
                if (!string.IsNullOrEmpty(dto.CustomerId))
                {
                    var customer = _customerRepository.GetById(dto.CustomerId);
                    if (customer == null || customer.DeletedDate != null)
                        return Result<string>.Fail("Khách hàng không tồn tại");
                }

                if (dto.OrderDetails == null || !dto.OrderDetails.Any())
                    return Result<string>.Fail("Đơn hàng phải có ít nhất 1 sách");

                decimal subtotal = 0;
                var orderDetails = new List<OrderDetail>();

                foreach (var item in dto.OrderDetails)
                {
                    var book = _bookRepository.GetById(item.BookId);
                    if (book == null || book.DeletedDate != null)
                        return Result<string>.Fail($"Sách {item.BookId} không tồn tại");

                    if (!book.SalePrice.HasValue)
                        return Result<string>.Fail($"Sách {book.Name} chưa có giá bán");

                    if (item.Quantity <= 0)
                        return Result<string>.Fail($"Số lượng sách {book.Name} phải > 0");

                    var available = book.Stock;
                    if (available < item.Quantity)
                        return Result<string>.Fail($"Sách {book.Name} không đủ hàng. Tồn: {available}");
                    book.Stock -= item.Quantity;
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

                var finalTotal = subtotal * (1-dto.Discount);

                // Sinh mã đơn hàng
                var orderId = GenerateOrderId();

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

                _orderRepository.Add(order);
                _orderRepository.SaveChanges();

                return Result<string>.Success(orderId, $"Tạo đơn hàng thành công. Tổng tiền: {finalTotal:N0} VND");
            }
            catch (Exception ex)
            {
                return Result<string>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- CẬP NHẬT / HỦY ---------------------------
        // ==================================================================
        public Result UpdateOrder(string orderId, UpdateOrderRequestDto dto)
        {
            try
            {
                var order = _orderRepository.GetById(orderId);
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

                    var subtotal = _orderDetailRepository.Find(od => od.OrderId == orderId).Sum(od => od.Subtotal);
                    if (dto.Discount > 1)
                        return Result.Fail("Giảm giá không được lớn hơn 100%");

                    order.Discount = dto.Discount.Value;
                    order.TotalPrice = subtotal * (1- dto.Discount.Value);
                }

                order.UpdatedDate = DateTime.Now;
                _orderRepository.Update(order);
                _orderRepository.SaveChanges();

                return Result.Success("Cập nhật đơn hàng thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result DeleteOrder(string orderId)
        {
            try
            {
                var order = _orderRepository.GetById(orderId);
                if (order == null || order.DeletedDate != null)
                    return Result.Fail("Đơn hàng không tồn tại");

                foreach (var i in order.OrderDetails)
                {
                    var book =  _bookRepository.GetById(i.BookId);
                    book.Stock += i.Quantity;
                }
                
                order.DeletedDate = DateTime.Now;
                _orderRepository.Update(order);
                _orderRepository.SaveChanges();

                return Result.Success("Hủy đơn hàng thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }
        

        // ==================================================================
        // ----------------------- TRUY VẤN ---------------------------------
        // ==================================================================
        public Result<OrderResponseDto> GetOrderById(string orderId)
        {
            try
            {
                var order = _orderRepository.GetById(orderId);
                if (order == null || order.DeletedDate != null)
                    return Result<OrderResponseDto>.Fail("Đơn hàng không tồn tại");

                var dto = MapToOrderResponseDto(order);
                return Result<OrderResponseDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return Result<OrderResponseDto>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<OrderResponseDto>> GetAllOrders()
        {
            try
            {
                var orders = _orderRepository.GetAll()
                    .Where(o => o.DeletedDate == null)
                    .OrderByDescending(o => o.CreatedDate)
                    .Select(MapToOrderResponseDto);
                return Result<IEnumerable<OrderResponseDto>>.Success(orders);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<OrderResponseDto>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<OrderResponseDto>> GetOrdersByCustomer(string customerId)
        {
            try
            {
                var orders = _orderRepository.GetByCustomer(customerId)
                    .Where(o => o.DeletedDate == null)
                    .OrderByDescending(o => o.CreatedDate)
                    .Select(MapToOrderResponseDto);
                return Result<IEnumerable<OrderResponseDto>>.Success(orders);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<OrderResponseDto>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<OrderResponseDto>> GetOrdersByStaff(string staffId)
        {
            try
            {
                var orders = _orderRepository.GetByStaff(staffId)
                    .Where(o => o.DeletedDate == null)
                    .OrderByDescending(o => o.CreatedDate)
                    .Select(MapToOrderResponseDto);
                return Result<IEnumerable<OrderResponseDto>>.Success(orders);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<OrderResponseDto>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<OrderResponseDto>> GetOrdersByDate(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var orders = _orderRepository.GetByDateRange(fromDate, toDate)
                    .Where(o => o.DeletedDate == null)
                    .OrderByDescending(o => o.CreatedDate)
                    .Select(MapToOrderResponseDto);
                return Result<IEnumerable<OrderResponseDto>>.Success(orders);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<OrderResponseDto>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<OrderDetailResponseDto>> GetOrderDetails(string orderId)
        {
            try
            {
                var details = _orderDetailRepository.GetByOrder(orderId)
                    .Select(d => new OrderDetailResponseDto
                    {
                        OrderId = d.OrderId,
                        BookId = d.BookId,
                        BookName = _bookRepository.GetById(d.BookId)?.Name ?? "Unknown",
                        SalePrice = d.SalePrice,
                        Quantity = d.Quantity,
                        Subtotal = d.Subtotal,
                        Notes = d.Notes
                    });
                return Result<IEnumerable<OrderDetailResponseDto>>.Success(details);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<OrderDetailResponseDto>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- HÀM PHỤ TRỢ ------------------------------
        // ==================================================================
        private string GenerateOrderId()
        {
            var lastOrder = _orderRepository.GetAll()
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