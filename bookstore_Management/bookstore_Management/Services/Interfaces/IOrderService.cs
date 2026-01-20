using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs.Order.Requests;
using bookstore_Management.DTOs.Order.Responses;

namespace bookstore_Management.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Result<string>> CreateOrderAsync(CreateOrderRequestDto dto);
        Task<Result> UpdateOrderAsync(string orderId, UpdateOrderRequestDto dto);
        Task<Result> DeleteOrderAsync(string orderId);
        Task<Result<OrderResponseDto>> GetOrderByIdAsync(string orderId);
        Task<Result<IEnumerable<OrderResponseDto>>> GetAllOrdersAsync();
        Task<Result<IEnumerable<OrderResponseDto>>> SearchOrderBillsAsync(string keyword);
        Task<Result<IEnumerable<OrderResponseDto>>> GetOrdersByCustomerAsync(string customerId);
        Task<Result<IEnumerable<OrderResponseDto>>> GetOrdersByStaffAsync(string staffId);
        Task<Result<IEnumerable<OrderResponseDto>>> GetOrdersByDateAsync(DateTime fromDate, DateTime toDate);
        Task<Result<IEnumerable<OrderDetailResponseDto>>> GetOrderDetailsAsync(string orderId);
    }
}