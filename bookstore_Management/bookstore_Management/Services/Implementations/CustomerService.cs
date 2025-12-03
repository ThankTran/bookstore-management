using System;
using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Repositories;
using bookstore_Management.DTOs;
using bookstore_Management.Models;

namespace bookstore_Management.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;

        internal CustomerService(ICustomerRepository customerRepository, IOrderRepository orderRepository)
        {
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
        }

        public Result<string> AddCustomer(CustomerDto dto)
        {
            try
            {
                // Validate
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result<string>.Fail("Tên không được trống");
                    
                if (string.IsNullOrWhiteSpace(dto.Phone))
                    return Result<string>.Fail("Số điện thoại không được trống");
                    
                if (dto.Phone.Length != 10 || !dto.Phone.All(char.IsDigit))
                    return Result<string>.Fail("Số điện thoại phải 10 chữ số");
                
                // Check duplicate phone
                var existing = _customerRepository.Get(dto.Phone);
                if (existing != null)
                    return Result<string>.Fail("Số điện thoại đã được đăng ký");
                
                // Generate Customer ID
                string customerId = GenerateCustomerId();
                
                // Create customer
                var customer = new Customer
                {
                    CustomerId = customerId,
                    Name = dto.Name.Trim(),
                    Phone = dto.Phone,
                    Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim(),
                    Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim(),
                    LoyaltyPoints = 0,
                    MemberLevel = MemberTier.WalkIn
                };
                
                _customerRepository.Add(customer);
                _customerRepository.SaveChanges();
                
                return Result<string>.Success(customerId, "Thêm khách hàng thành công");
            }
            catch (Exception ex)
            {
                return Result<string>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result UpdateCustomer(string customerId, CustomerDto dto)
        {
            try
            {
                var customer = _customerRepository.Get(customerId);
                if (customer == null)
                    return Result.Fail("Khách hàng không tồn tại");
                
                // Check phone duplicate (if changed)
                if (dto.Phone != customer.Phone)
                {
                    var existing = _customerRepositor.Get(dto.Phone);
                    if (existing != null && existing.CustomerId != customerId)
                        return Result.Fail("Số điện thoại đã được sử dụng");
                }
                
                customer.Name = dto.Name.Trim();
                customer.Phone = dto.Phone;
                customer.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
                customer.Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim();
                
                _customerRepository.Update(customer);
                _customerRepository.SaveChanges();
                
                return Result.Success("Cập nhật thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result DeleteCustomer(string customerId)
        {
            try
            {
                var customer = _customerRepository.Get(customerId);
                if (customer == null)
                    return Result.Fail("Khách hàng không tồn tại");
                
                _customerRepository.Delete(customerId);
                _customerRepository.SaveChanges();
                
                return Result.Success("Xóa khách hàng thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<Customer> GetCustomerById(string customerId)
        {
            try
            {
                var customer = _customerRepository.Get(customerId);
                if (customer == null)
                    return Result<Customer>.Fail("Khách hàng không tồn tại");
                    
                return Result<Customer>.Success(customer);
            }
            catch (Exception ex)
            {
                return Result<Customer>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<Customer>> GetAllCustomers()
        {
            try
            {
                var customers = _customerRepository.GetAll();
                return Result<IEnumerable<Customer>>.Success(customers);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Customer>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<Customer> GetCustomerByPhone(string phone)
        {
            try
            {
                var customer = _customerRepository.GetByPhone(phone);
                if (customer == null)
                    return Result<Customer>.Fail("Không tìm thấy khách hàng");
                    
                return Result<Customer>.Success(customer);
            }
            catch (Exception ex)
            {
                return Result<Customer>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<Customer>> SearchByName(string name)
        {
            try
            {
                var customers = _customerRepository.SearchByName(name);
                return Result<IEnumerable<Customer>>.Success(customers);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Customer>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<Customer>> GetByMemberLevel(MemberTier level)
        {
            try
            {
                var customers = _customerRepository.GetByMemberLevel(level);
                return Result<IEnumerable<Customer>>.Success(customers);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Customer>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result AddPoints(string customerId, decimal points)
        {
            try
            {
                var customer = _customerRepository.Get(customerId);
                if (customer == null)
                    return Result.Fail("Khách hàng không tồn tại");
                
                customer.LoyaltyPoints += points;
                
                // Auto update member level
                customer.MemberLevel = CalculateMemberLevel(customer.LoyaltyPoints).Data;
                
                _customerRepository.Update(customer);
                _customerRepository.SaveChanges();
                
                return Result.Success($"Thêm {points} điểm thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result UsePoints(string customerId, decimal points)
        {
            try
            {
                var customer = _customerRepository.Get(customerId);
                if (customer == null)
                    return Result.Fail("Khách hàng không tồn tại");
                
                if (customer.LoyaltyPoints < points)
                    return Result.Fail("Không đủ điểm");
                
                customer.LoyaltyPoints -= points;
                
                _customerRepository.Update(customer);
                _customerRepository.SaveChanges();
                
                return Result.Success($"Sử dụng {points} điểm thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<decimal> GetPoints(string customerId)
        {
            try
            {
                var customer = _customerRepository.Get(customerId);
                if (customer == null)
                    return Result<decimal>.Fail("Khách hàng không tồn tại");
                    
                return Result<decimal>.Success(customer.LoyaltyPoints);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result UpgradeMemberLevel(string customerId)
        {
            try
            {
                var customer = _customerRepository.Get(customerId);
                if (customer == null)
                    return Result.Fail("Khách hàng không tồn tại");
                
                // Simple upgrade logic
                if (customer.MemberLevel == MemberTier.WalkIn)
                    customer.MemberLevel = MemberTier.Silver;
                else if (customer.MemberLevel == MemberTier.Silver)
                    customer.MemberLevel = MemberTier.Gold;
                else if (customer.MemberLevel == MemberTier.Gold)
                    customer.MemberLevel = MemberTier.Platinum;
                
                _customerRepository.Update(customer);
                _customerRepository.SaveChanges();
                
                return Result.Success("Nâng hạng thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result DowngradeMemberLevel(string customerId)
        {
            try
            {
                var customer = _customerRepository.Get(customerId);
                if (customer == null)
                    return Result.Fail("Khách hàng không tồn tại");
                
                // Simple downgrade logic
                if (customer.MemberLevel == MemberTier.Diamond)
                    customer.MemberLevel = MemberTier.Gold;
                else if (customer.MemberLevel == MemberTier.Gold)
                    customer.MemberLevel = MemberTier.Silver;
                else if (customer.MemberLevel == MemberTier.Silver)
                    customer.MemberLevel = MemberTier.WalkIn;
                
                _customerRepository.Update(customer);
                _customerRepository.SaveChanges();
                
                return Result.Success("Hạ hạng thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<MemberTier> CalculateMemberLevel(decimal totalSpent)
        {
            // Business logic for member level
            if (totalSpent >= 10000000) // 10 triệu
                return Result<MemberTier>.Success(MemberTier.Platinum);
            else if (totalSpent >= 5000000) // 5 triệu
                return Result<MemberTier>.Success(MemberTier.Gold);
            else if (totalSpent >= 1000000) // 1 triệu
                return Result<MemberTier>.Success(MemberTier.Silver);
            else
                return Result<MemberTier>.Success(MemberTier.WalkIn);
        }

        private string GenerateCustomerId()
        {
            var lastCustomer = _customerRepository.GetAll()
                .OrderByDescending(c => c.CustomerId)
                .FirstOrDefault();
                
            if (lastCustomer == null || !lastCustomer.CustomerId.StartsWith("KH"))
                return "KH0001";
                
            int lastNumber = int.Parse(lastCustomer.CustomerId.Substring(2));
            return $"KH{(lastNumber + 1):D4}";
        }
    }
}
