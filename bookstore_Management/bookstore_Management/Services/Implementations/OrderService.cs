using System;
using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Repositories;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.DTOs;
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

        public Result<string> CreateOrder(OrderDto dto)
        {
            try
            {
                // Validate staff
                var staff = _staffRepository.GetById(dto.StaffId);
                if (staff == null)
                    return Result<string>.Fail("Nhân viên không tồn tại");
                
                // Validate customer 
                if (!string.IsNullOrEmpty(dto.CustomerId))
                {
                    var customer = _customerRepository.GetById(dto.CustomerId);
                    if (customer == null)
                        return Result<string>.Fail("Khách hàng không tồn tại");
                }
                
                // Validate items
                if (dto.Items == null || !dto.Items.Any())
                    return Result<string>.Fail("Đơn hàng phải có ít nhất 1 sách");
                
                decimal totalPrice = 0;
                var orderDetails = new List<OrderDetail>();
                
                // Calculate total
                foreach (var item in dto.Items)
                {
                    var book = _bookRepository.GetById(item.BookId);
                    if (book == null)
                        return Result<string>.Fail($"Sách {item.BookId} không tồn tại");
                    
                    if (item.Quantity <= 0)
                        return Result<string>.Fail($"Số lượng sách {book.Name} phải > 0");
                    
                    if (!book.SalePrice.HasValue)
                        return Result<string>.Fail($"Sách {book.Name} chưa có giá bán");
                    
                    var itemTotal = book.SalePrice.Value * item.Quantity;
                    totalPrice += itemTotal;
                    
                    orderDetails.Add(new OrderDetail
                    {
                        OrderId = "", // Will be set after order created
                        BookId = item.BookId,
                        SalePrice = book.SalePrice.Value,
                        Quantity = item.Quantity
                    });
                }
                
                // Apply discount
                decimal discountAmount = totalPrice * (dto.DiscountPercent / 100);
                decimal finalTotal = totalPrice - discountAmount;
                
                // Generate Order ID
                string orderId = GenerateOrderId();
                
                // Create order
                var order = new Order
                {
                    OrderId = orderId,
                    StaffId = dto.StaffId,
                    CustomerId = string.IsNullOrEmpty(dto.CustomerId) ? null : dto.CustomerId,
                    PaymentMethod = dto.PaymentMethod,
                    Discount = discountAmount,
                    TotalPrice = finalTotal,
                    Notes = dto.Notes
                };
                
                // Set OrderId for details
                foreach (var detail in orderDetails)
                {
                    detail.OrderId = orderId;
                }
                
                order.OrderDetails = orderDetails;
                
                // Save to database
                _orderRepository.Add(order);
                _orderRepository.SaveChanges();
                
                return Result<string>.Success(orderId, "Tạo đơn hàng thành công");
            }
            catch (Exception ex)
            {
                return Result<string>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result UpdateOrder(string orderId, UpdateOrderDto dto)
        {
            try
            {
                var order = _orderRepository.GetById(orderId);
                if (order == null || order.DeletedDate != null)
                    return Result.Fail("Đơn hàng không tồn tại");

                if (!string.IsNullOrEmpty(dto.Notes))
                    order.Notes = dto.Notes;

                if (dto.PaymentMethod.HasValue)
                    order.PaymentMethod = dto.PaymentMethod.Value;

                if (dto.Discount.HasValue)
                {
                    var orderDetails = _orderDetailRepository.Find(od => od.OrderId == orderId);
                    var subtotal = orderDetails.Sum(od => od.SalePrice * od.Quantity);

                    // discount theo %
                    var discountAmount = subtotal * (dto.Discount.Value / 100m);
                    order.Discount = discountAmount;
                    order.TotalPrice = subtotal - discountAmount;
                }

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

                // Soft delete
                order.DeletedDate = DateTime.Now;
                _orderRepository.Update(order);
                _orderRepository.SaveChanges();

                return Result.Success("Hủy đơn hàng thành công (soft delete)");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<Order> GetOrderById(string orderId)
        {
            try
            {
                var order = _orderRepository.GetById(orderId);
                if (order == null)
                    return Result<Order>.Fail("Đơn hàng không tồn tại");
                    
                return Result<Order>.Success(order);
            }
            catch (Exception ex)
            {
                return Result<Order>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<Order>> GetAllOrders()
        {
            try
            {
                var orders = _orderRepository.GetAll();
                return Result<IEnumerable<Order>>.Success(orders);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Order>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<Order>> GetOrdersByCustomer(string customerId)
        {
            try
            {
                var orders = _orderRepository.Find(o => o.CustomerId == customerId);
                return Result<IEnumerable<Order>>.Success(orders);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Order>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<Order>> GetOrdersByStaff(string staffId)
        {
            try
            {
                var orders = _orderRepository.Find(o => o.StaffId == staffId);
                return Result<IEnumerable<Order>>.Success(orders);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Order>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<Order>> GetOrdersByDate(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var orders = _orderRepository.Find(o => 
                    o.CreatedDate >= fromDate && o.CreatedDate <= toDate && o.DeletedDate == null);

                return Result<IEnumerable<Order>>.Success(orders);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Order>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<Order>> SearchOrders(OrderSearchDto criteria)
        {
            try
            {
                var query = _orderRepository.GetAll()
                    .Where(o => o.DeletedDate == null)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(criteria.StaffId))
                    query = query.Where(o => o.StaffId == criteria.StaffId);

                if (!string.IsNullOrEmpty(criteria.CustomerId))
                    query = query.Where(o => o.CustomerId == criteria.CustomerId);

                if (criteria.PaymentMethod.HasValue)
                    query = query.Where(o => o.PaymentMethod == criteria.PaymentMethod.Value);

                if (criteria.MinTotal.HasValue)
                    query = query.Where(o => o.TotalPrice >= criteria.MinTotal.Value);

                if (criteria.MaxTotal.HasValue)
                    query = query.Where(o => o.TotalPrice <= criteria.MaxTotal.Value);

                if (criteria.FromDate.HasValue)
                    query = query.Where(o => o.CreatedDate >= criteria.FromDate.Value);

                if (criteria.ToDate.HasValue)
                    query = query.Where(o => o.CreatedDate <= criteria.ToDate.Value);

                return Result<IEnumerable<Order>>.Success(query.ToList());
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Order>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result ProcessPayment(string orderId, PaymentType paymentMethod)
        {
            try
            {
                var order = _orderRepository.GetById(orderId);
                if (order == null)
                    return Result.Fail("Đơn hàng không tồn tại");
                
                order.PaymentMethod = paymentMethod;
                _orderRepository.Update(order);
                _orderRepository.SaveChanges();
                
                return Result.Success($"Xử lý thanh toán {paymentMethod} thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result ApplyDiscount(string orderId, decimal discountAmount)
        {
            try
            {
                var order = _orderRepository.GetById(orderId);
                if (order == null)
                    return Result.Fail("Đơn hàng không tồn tại");
                
                var orderDetails = _orderDetailRepository.Find(od => od.OrderId == orderId);
                decimal subtotal = orderDetails.Sum(od => od.SalePrice * od.Quantity);
                
                if (discountAmount > subtotal)
                    return Result.Fail("Giảm giá không được lớn hơn tổng tiền");
                
                order.Discount = discountAmount;
                order.TotalPrice = subtotal - discountAmount;
                
                _orderRepository.Update(order);
                _orderRepository.SaveChanges();
                
                return Result.Success($"Áp dụng giảm giá {discountAmount:N0} thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<decimal> CalculateOrderTotal(OrderDto dto)
        {
            try
            {
                decimal total = 0;
                
                foreach (var item in dto.Items)
                {
                    var book = _bookRepository.GetById(item.BookId);
                    if (book == null || !book.SalePrice.HasValue)
                        return Result<decimal>.Fail($"Sách {item.BookId} không tồn tại hoặc chưa có giá");
                    
                    total += book.SalePrice.Value * item.Quantity;
                }
                
                // Apply discount
                decimal discount = total * (dto.DiscountPercent / 100);
                decimal finalTotal = total - discount;
                
                return Result<decimal>.Success(finalTotal);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result AddOrderItem(string orderId, OrderItemDto item)
        {
            try
            {
                var order = _orderRepository.GetById(orderId);
                if (order == null)
                    return Result.Fail("Đơn hàng không tồn tại");
                
                var book = _bookRepository.GetById(item.BookId);
                if (book == null || !book.SalePrice.HasValue)
                    return Result.Fail("Sách không tồn tại hoặc chưa có giá");
                
                // Check if item already exists
                var existingDetail = _orderDetailRepository
                    .Find(od => od.OrderId == orderId && od.BookId == item.BookId)
                    .FirstOrDefault();
                
                if (existingDetail != null)
                {
                    // Update quantity
                    existingDetail.Quantity += item.Quantity;
                    _orderDetailRepository.Update(existingDetail);
                }
                else
                {
                    // Add new detail
                    var detail = new OrderDetail
                    {
                        OrderId = orderId,
                        BookId = item.BookId,
                        SalePrice = book.SalePrice.Value,
                        Quantity = item.Quantity
                    };
                    _orderDetailRepository.Add(detail);
                }
                
                // Recalculate order total
                RecalculateOrderTotal(orderId);
                
                return Result.Success("Thêm sách vào đơn hàng thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result RemoveOrderItem(string orderId, string bookId)
        {
            try
            {
                var detail = _orderDetailRepository
                    .Find(od => od.OrderId == orderId && od.BookId == bookId)
                    .FirstOrDefault();
                
                if (detail == null)
                    return Result.Fail("Không tìm thấy sách trong đơn hàng");
                
                //_orderDetailRepository.Delete(detail);
                _orderDetailRepository.SaveChanges();
                
                // Recalculate order total
                RecalculateOrderTotal(orderId);
                
                return Result.Success("Xóa sách khỏi đơn hàng thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result UpdateOrderItem(string orderId, string bookId, int newQuantity)
        {
            try
            {
                var detail = _orderDetailRepository
                    .Find(od => od.OrderId == orderId && od.BookId == bookId)
                    .FirstOrDefault();
                
                if (detail == null)
                    return Result.Fail("Không tìm thấy sách trong đơn hàng");
                
                if (newQuantity <= 0)
                    return Result.Fail("Số lượng phải > 0");
                
                detail.Quantity = newQuantity;
                _orderDetailRepository.Update(detail);
                _orderDetailRepository.SaveChanges();
                
                // Recalculate order total
                RecalculateOrderTotal(orderId);
                
                return Result.Success("Cập nhật số lượng thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<OrderDetail>> GetOrderDetails(string orderId)
        {
            try
            {
                var details = _orderDetailRepository.Find(od => od.OrderId == orderId);
                return Result<IEnumerable<OrderDetail>>.Success(details);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<OrderDetail>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        private string GenerateOrderId()
        {
            var lastOrder = _orderRepository.GetAll()
                .OrderByDescending(o => o.OrderId)
                .FirstOrDefault();
                
            if (lastOrder == null || !lastOrder.OrderId.StartsWith("HD"))
                return "HD0001";
                
            int lastNumber = int.Parse(lastOrder.OrderId.Substring(2));
            return $"HD{(lastNumber + 1):D4}";
        }

        private void RecalculateOrderTotal(string orderId)
        {
            var order = _orderRepository.GetById(orderId);
            if (order == null) return;
            
            var orderDetails = _orderDetailRepository.Find(od => od.OrderId == orderId);
            var subtotal = orderDetails.Sum(od => od.SalePrice * od.Quantity);
            
            //order.TotalPrice = subtotal - (order.Discount ?? 0);
            _orderRepository.Update(order);
            _orderRepository.SaveChanges();
        }
    }
}
