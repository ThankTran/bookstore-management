using System.Collections.Generic;
using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs;
using bookstore_Management.Models;

namespace bookstore_Management.Services
{
    internal interface ICustomerService
    {
        // CRUD cho Customer
        Result<string> AddCustomer(CustomerDto dto);
        Result UpdateCustomer(string customerId, CustomerDto dto);
        Result DeleteCustomer(string customerId);
        Result<Customer> GetCustomerById(string customerId);
        Result<IEnumerable<Customer>> GetAllCustomers();
        
        // Tìm kiếm
        Result<Customer> GetCustomerByPhone(string phone);
        Result<IEnumerable<Customer>> SearchByName(string name);
        Result<IEnumerable<Customer>> GetByMemberLevel(MemberTier level);
        
        // Quản lý điểm tích lũy
        Result AddPoints(string customerId, decimal points);
        Result UsePoints(string customerId, decimal points);
        Result<decimal> GetPoints(string customerId);
        
        // Quản lý hạng thành viên
        Result UpgradeMemberLevel(string customerId);
        Result DowngradeMemberLevel(string customerId);
        Result<MemberTier> CalculateMemberLevel(decimal totalSpent);
    }
}