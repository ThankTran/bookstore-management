using System;
using System.Collections.Generic;
using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs.Order.Requests;
using bookstore_Management.DTOs.Order.Responses;
using bookstore_Management.Models;
namespace bookstore_Management.Services.Interfaces
{
    public interface IOrderService
    {
        // CRUD
        Result<string> CreateOrder(CreateOrderRequestDto dto);
        Result UpdateOrder(string orderId, UpdateOrderRequestDto dto);
        Result DeleteOrder(string orderId);
        Result CancelOrder(string orderId, string reason);
        Result<OrderResponseDto> GetOrderById(string orderId);
        Result<IEnumerable<OrderResponseDto>> GetAllOrders();
        
        // Tìm kiếm đơn hàng
        Result<IEnumerable<OrderResponseDto>> GetOrdersByCustomer(string customerId);
        Result<IEnumerable<OrderResponseDto>> GetOrdersByStaff(string staffId);
        Result<IEnumerable<OrderResponseDto>> GetOrdersByDate(DateTime fromDate, DateTime toDate);
        Result<IEnumerable<OrderDetailResponseDto>> GetOrderDetails(string orderId);
    }
}