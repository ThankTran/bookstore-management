using System;
using System.Collections.Generic;
using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs.Customer.Requests;
using bookstore_Management.DTOs.Customer.Responses;
using bookstore_Management.Models;
namespace bookstore_Management.Services.Interfaces
{
    public interface ICustomerService
    {
        // CRUD
        Result<string> AddCustomer(CreateCustomerRequestDto dto);
        Result UpdateCustomer(string customerId, UpdateCustomerRequestDto dto);
        Result DeleteCustomer(string customerId);
        Result<CustomerDetailResponseDto> GetCustomerById(string customerId);
        Result<IEnumerable<CustomerDetailResponseDto>> GetAllCustomers();
        
        
        // Tìm kiếm
        Result<CustomerDetailResponseDto> GetCustomerByPhone(string phone);
        Result<IEnumerable<CustomerDetailResponseDto>> SearchByName(string name);
        Result<IEnumerable<CustomerDetailResponseDto>> GetByMemberLevel(MemberTier level);
        Result<IEnumerable<CustomerDetailResponseDto>> SearchByTotalSpent(decimal minimum, decimal maximum, DateTime startDate, DateTime endDate);
        

        // Quản lý điểm tích lũy, doanh thu trên tháng
        Result AddPoints(string customerId, decimal points);
        Result UsePoints(string customerId, decimal points);
        Result<decimal> GetPoints(string customerId);

        // Quản lý hạng thành viên
        Result UpgradeMemberLevel(string customerId);
        Result DowngradeMemberLevel(string customerId);
        Result<MemberTier> CalculateMemberLevel(decimal totalSpent);

        // Lịch sử mua hàng
        Result<IEnumerable<Order>> GetCustomerOrderHistory(string customerId, DateTime fromDate, DateTime toDate);
        Result <decimal> CustomerTotalSpentPerDay(string customerId, DateTime date);
        Result<decimal> CustomerTotalSpentPerMonth(string customerId, int month, int year);
        Result<decimal> CustomerTotalSpentPerYear(string customerId, int year);
        
        // Listviewitem
        Result<IEnumerable<CustomerListResponseDto>> GetCustomerList();
    }
}
