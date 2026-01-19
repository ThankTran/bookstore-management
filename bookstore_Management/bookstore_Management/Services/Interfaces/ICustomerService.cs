using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs.Customer.Requests;
using bookstore_Management.DTOs.Customer.Responses;
using bookstore_Management.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bookstore_Management.Services.Interfaces
{
    public interface ICustomerService
    {

        Task<Result<string>> AddCustomerAsync(CreateCustomerRequestDto dto);
        

        Task<Result> UpdateCustomerAsync(string customerId, UpdateCustomerRequestDto dto);


        Task<Result> DeleteCustomerAsync(string customerId);


        Task<Result<CustomerDetailResponseDto>> GetCustomerByIdAsync(string customerId);
        
        Task<Result<IEnumerable<CustomerDetailResponseDto>>> GetAllCustomersAsync();
        
        Task<Result<CustomerDetailResponseDto>> GetCustomerByPhoneAsync(string phone);
        
        Task<Result<IEnumerable<CustomerDetailResponseDto>>> SearchByNameAsync(string name);
        
        Task<Result<IEnumerable<CustomerDetailResponseDto>>> GetByMemberLevelAsync(MemberTier level);
        
        Task<Result<IEnumerable<CustomerDetailResponseDto>>> SearchByTotalSpentAsync(
            decimal minimum, decimal maximum, DateTime startDate, DateTime endDate);
        

        Task<Result> UpgradeMemberLevelAsync(string customerId);
        
        Task<Result> DowngradeMemberLevelAsync(string customerId);
        
        Task<Result<MemberTier>> CalculateMemberTierAsync(decimal totalSpent);


        Task<Result<IEnumerable<Order>>> GetCustomerOrderHistoryAsync(
            string customerId, DateTime fromDate, DateTime toDate);
        
        Task<Result<decimal>> CustomerTotalSpentPerDayAsync(string customerId, DateTime date);
        
        Task<Result<decimal>> CustomerTotalSpentPerMonthAsync(string customerId, int month, int year);
        
        Task<Result<decimal>> CustomerTotalSpentPerYearAsync(string customerId, int year);
        
    }
}