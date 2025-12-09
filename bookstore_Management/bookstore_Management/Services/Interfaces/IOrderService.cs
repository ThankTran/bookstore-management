using System;
using System.Collections.Generic;
using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs;
using bookstore_Management.Models;
namespace bookstore_Management.Services.Interfaces
{
    public interface IOrderService
    {
        // CRUD
        Result<string> CreateOrder(OrderDto dto);
        Result UpdateOrder(string orderId, UpdateOrderDto dto);
        Result DeleteOrder(string orderId);
        Result CancelOrder(string orderId, string reason);
        Result<Order> GetOrderById(string orderId);
        Result<IEnumerable<Order>> GetAllOrders();
        // Tìm kiếm đơn hàng
        Result<IEnumerable<Order>> GetOrdersByCustomer(string customerId);
        Result<IEnumerable<Order>> GetOrdersByStaff(string staffId);
        Result<IEnumerable<Order>> GetOrdersByDate(DateTime fromDate, DateTime toDate);
        Result<IEnumerable<Order>> SearchOrders(OrderSearchDto criteria);

        // Xử lý thanh toán
        Result ProcessPayment(string orderId, PaymentType paymentMethod);
        Result ApplyDiscount(string orderId, decimal discountAmount);
        Result<decimal> CalculateOrderTotal(OrderDto dto);

        // Quản lý chi tiết đơn hàng
        Result AddOrderItem(string orderId, OrderItemDto item);
        Result RemoveOrderItem(string orderId, string bookId);
        Result UpdateOrderItem(string orderId, string bookId, int newQuantity);
        Result<IEnumerable<OrderDetail>> GetOrderDetails(string orderId);
    }
}