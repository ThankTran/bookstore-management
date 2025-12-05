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
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result<string>.Fail("Tên không được trống");

                if (string.IsNullOrWhiteSpace(dto.Phone))
                    return Result<string>.Fail("Số điện thoại không được trống");

                if (dto.Phone.Length != 10 || !dto.Phone.All(char.IsDigit))
                    return Result<string>.Fail("Số điện thoại phải 10 chữ số");

                // kiểm tra trùng số điện thoại
                var existing = _customerRepository.SearchByPhone(dto.Phone);
                if (existing != null)
                    return Result<string>.Fail("Số điện thoại đã được đăng ký");

                var customerId = GenerateCustomerId();

                var customer = new Customer
                {
                    CustomerId = customerId,
                    Name = dto.Name.Trim(),
                    Phone = dto.Phone,
                    Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim(),
                    Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim(),
                    LoyaltyPoints = 0,
                    MemberLevel = MemberTier.Bronze,
                    DeletedDate = null
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
        
        
        // Hàm update khách hàng
        public Result UpdateCustomer(string customerId, CustomerDto dto)
        {
            try
            {
                var customer = _customerRepository.GetById(customerId);
                if (customer == null || customer.DeletedDate != null)
                    return Result.Fail("Khách hàng không tồn tại");

                if (dto.Phone != customer.Phone)
                {
                    var existing = _customerRepository.SearchByPhone(dto.Phone);
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
                var customer = _customerRepository.GetById(customerId);
                if (customer == null || customer.DeletedDate != null)
                    return Result.Fail("Khách hàng không tồn tại");

                var orders = _orderRepository.Find(o => o.CustomerId == customerId);
                if (orders.Any())
                    return Result.Fail("Không thể xóa khách hàng đã có đơn hàng");

                // Soft delete
                customer.DeletedDate = DateTime.Now;
                _customerRepository.Update(customer);
                _customerRepository.SaveChanges();

                return Result.Success("Xóa khách hàng thành công (soft delete)");
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
                var customer = _customerRepository.GetById(customerId);
                return (customer == null) ?
                    Result<Customer>.Fail("Khách hàng không tồn tại"): 
                    Result<Customer>.Success(customer);
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
                var customer = _customerRepository.SearchByPhone(phone);
                return (customer == null) ? 
                    Result<Customer>.Fail("Không tìm thấy khách hàng") : 
                    Result<Customer>.Success(customer);
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
                if (points <= 0)
                    return Result.Fail("Điểm phải lớn hơn 0");

                var customer = _customerRepository.GetById(customerId);
                if (customer == null || customer.DeletedDate != null)
                    return Result.Fail("Khách hàng không tồn tại");

                customer.LoyaltyPoints += points;
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
                if (points <= 0)
                    return Result.Fail("Điểm phải lớn hơn 0");

                var customer = _customerRepository.GetById(customerId);
                if (customer == null || customer.DeletedDate != null)
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

        // lấy số điểm hện tại của khách hàng
        public Result<decimal> GetPoints(string customerId)
        {
            try
            {
                var customer = _customerRepository.GetById(customerId);
                return (customer == null) ? 
                    Result<decimal>.Fail("Khách hàng không tồn tại") : 
                    Result<decimal>.Success(customer.LoyaltyPoints);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // hàm tăng hạng thành viên
        public Result UpgradeMemberLevel(string customerId)
        {
            try
            {
                var customer = _customerRepository.GetById(customerId);
                if (customer == null || customer.DeletedDate != null)
                    return Result.Fail("Khách hàng không tồn tại");
                
                switch (customer.MemberLevel)
                {
                    case MemberTier.Bronze:
                        customer.MemberLevel = MemberTier.Silver;
                        break;

                    case MemberTier.Silver:
                        customer.MemberLevel = MemberTier.Gold;
                        break;

                    case MemberTier.Gold:
                        customer.MemberLevel = MemberTier.Diamond;
                        break;
                    case MemberTier.Diamond:
                    default:
                        break;
                }
                _customerRepository.Update(customer);
                _customerRepository.SaveChanges();

                return Result.Success("Nâng hạng thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        // hàm giảm hạng thành viên
        public Result DowngradeMemberLevel(string customerId)
        {
            try
            {
                var customer = _customerRepository.GetById(customerId);
                if (customer == null)
                    return Result.Fail("Khách hàng không tồn tại");
                
                // Simple downgrade logic
                switch (customer.MemberLevel)
                {
                    case MemberTier.Diamond:
                        customer.MemberLevel = MemberTier.Gold;
                        break;

                    case MemberTier.Gold:
                        customer.MemberLevel = MemberTier.Silver;
                        break;

                    case MemberTier.Silver:
                        customer.MemberLevel = MemberTier.Bronze;
                        break;
                    
                    case MemberTier.Bronze:
                    default:
                        break;
                }
                _customerRepository.Update(customer);
                _customerRepository.SaveChanges();
                return Result.Success("Hạ hạng thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        // Hàm tính toán để set hạng thành viên - có thể thay đổi
        public Result<MemberTier> CalculateMemberLevel(decimal totalSpent)
        {
            // Business logic for member level
            if (totalSpent >= 10000000) // 10 triệu
                return Result<MemberTier>.Success(MemberTier.Diamond);
            else if (totalSpent >= 5000000) // 5 triệu
                return Result<MemberTier>.Success(MemberTier.Gold);
            else if (totalSpent >= 1000000) // 1 triệu
                return Result<MemberTier>.Success(MemberTier.Silver);
            else
                return Result<MemberTier>.Success(MemberTier.Bronze);
        }

        // Hàm khởi tạo mã khách hàng
        private string GenerateCustomerId()
        {
            // trả về khách hàng cuối cùng
            var lastCustomer = _customerRepository.GetAll()
                .OrderByDescending(c => c.CustomerId)
                .FirstOrDefault();
                
            // nếu chưa tồn tại khách hàng --> khởi tạo khách hàng
            if (lastCustomer == null || !lastCustomer.CustomerId.StartsWith("KH"))
                return "KH0001";
            
            var lastNumber = int.Parse(lastCustomer.CustomerId.Substring(2));
            return $"KH{(lastNumber + 1):D4}";
        }
    }
}
