using System;
using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.DTOs.Customer.Requests;
using bookstore_Management.DTOs.Customer.Responses;
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
        
        // ==================================================================
        // ---------------------- THÊM DỮ LIỆU ------------------------------
        // ==================================================================
        public Result<string> AddCustomer(CreateCustomerRequestDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result<string>.Fail("Tên không được trống");

                if (string.IsNullOrWhiteSpace(dto.Phone))
                    return Result<string>.Fail("Số điện thoại không được trống");

                if (dto.Phone.Length < 10 || dto.Phone.Length > 20 || !dto.Phone.All(char.IsDigit))
                    return Result<string>.Fail("Số điện thoại phải từ 10-20 chữ số");

                // Kiểm tra trùng số điện thoại
                var existing = _customerRepository.SearchByPhone(dto.Phone);
                if (existing != null)
                    return Result<string>.Fail("Số điện thoại đã được đăng ký");

                var customerId = GenerateCustomerId();

                var customer = new Customer
                {
                    CustomerId = customerId,
                    Name = dto.Name.Trim(),
                    Phone = dto.Phone,
                    LoyaltyPoints = 0,
                    MemberLevel = MemberTier.Bronze,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = null,
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
        
        // ==================================================================
        // ----------------------- SỬA DỮ LIỆU ------------------------------
        // ==================================================================
        public Result UpdateCustomer(string customerId, UpdateCustomerRequestDto dto)
        {
            try
            {
                var customer = _customerRepository.GetById(customerId);
                if (customer == null || customer.DeletedDate != null)
                    return Result.Fail("Khách hàng không tồn tại");

                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result.Fail("Tên không được trống");

                if (string.IsNullOrWhiteSpace(dto.Phone))
                    return Result.Fail("Số điện thoại không được trống");

                if (dto.Phone.Length < 10 || dto.Phone.Length > 20 || !dto.Phone.All(char.IsDigit))
                    return Result.Fail("Số điện thoại phải từ 10-20 chữ số");

                // Kiểm tra phone trùng
                if (dto.Phone != customer.Phone)
                {
                    var existing = _customerRepository.SearchByPhone(dto.Phone);
                    if (existing != null && existing.CustomerId != customerId)
                        return Result.Fail("Số điện thoại đã được sử dụng");
                }

                customer.Name = dto.Name.Trim();
                customer.Phone = dto.Phone;
                if (dto.MemberLevel.HasValue)
                    customer.MemberLevel = dto.MemberLevel.Value;
                if (dto.LoyaltyPoints.HasValue)
                    customer.LoyaltyPoints = dto.LoyaltyPoints.Value;
                customer.UpdatedDate = DateTime.Now;

                _customerRepository.Update(customer);
                _customerRepository.SaveChanges();

                return Result.Success("Cập nhật khách hàng thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ---------------------- XÓA DỮ LIỆU -------------------------------
        // ==================================================================
        public Result DeleteCustomer(string customerId)
        {
            try
            {
                var customer = _customerRepository.GetById(customerId);
                if (customer == null || customer.DeletedDate != null)
                    return Result.Fail("Khách hàng không tồn tại");

                var orders = _orderRepository.Find(o => o.CustomerId == customerId && o.DeletedDate == null);
                if (orders.Any())
                    return Result.Fail("Không thể xóa khách hàng đã có đơn hàng");

                // Soft delete
                customer.DeletedDate = DateTime.Now;
                _customerRepository.Update(customer);
                _customerRepository.SaveChanges();

                return Result.Success("Xóa khách hàng thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- LẤY DỮ LIỆU ------------------------------
        // ==================================================================
        public Result<CustomerDetailResponseDto> GetCustomerById(string customerId)
        {
            try
            {
                var c = _customerRepository.GetById(customerId);
                if (c == null || c.DeletedDate != null)
                    return Result<CustomerDetailResponseDto>.Fail("Khách hàng không tồn tại");

                var orders = c.Orders?.Where(o => o.DeletedDate == null) ?? Enumerable.Empty<Order>();

                var dto = new CustomerDetailResponseDto
                {
                    CustomerId = c.CustomerId,
                    Name = c.Name,
                    Phone = c.Phone,
                    MemberLevel = c.MemberLevel,
                    LoyaltyPoints = c.LoyaltyPoints,
                    CreatedDate = c.CreatedDate,
                    TotalOrders = orders.Count(),
                    TotalSpent = orders.Sum(o => o.TotalPrice)
                };

                return Result<CustomerDetailResponseDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return Result<CustomerDetailResponseDto>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<CustomerDetailResponseDto>> GetAllCustomers()
        {
            try
            {
                var customers = _customerRepository.GetAll()
                    .Where(c => c.DeletedDate == null)
                    .OrderBy(c => c.Name)
                    .Select(c =>
                    {
                        var orders = c.Orders.Where(o => o.DeletedDate == null);
                        return new CustomerDetailResponseDto
                        {
                            CustomerId = c.CustomerId,
                            Name = c.Name,
                            Phone = c.Phone,
                            MemberLevel = c.MemberLevel,
                            LoyaltyPoints = c.LoyaltyPoints,
                            CreatedDate = c.CreatedDate,
                            TotalOrders = orders.Count(),
                            TotalSpent = orders.Sum(o => o.TotalPrice)
                        };
                    })
                    .ToList();

                return Result<IEnumerable<CustomerDetailResponseDto>>.Success(customers);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<CustomerDetailResponseDto>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<CustomerDetailResponseDto> GetCustomerByPhone(string phone)
        {
            try
            {
                var c = _customerRepository.SearchByPhone(phone);
                if (c == null || c.DeletedDate != null)
                    return Result<CustomerDetailResponseDto>.Fail("Không tìm thấy khách hàng");

                var orders = c.Orders?.Where(o => o.DeletedDate == null) ?? Enumerable.Empty<Order>();

                var dto = new CustomerDetailResponseDto
                {
                    CustomerId = c.CustomerId,
                    Name = c.Name,
                    Phone = c.Phone,
                    MemberLevel = c.MemberLevel,
                    LoyaltyPoints = c.LoyaltyPoints,
                    CreatedDate = c.CreatedDate,
                    TotalOrders = orders.Count(),
                    TotalSpent = orders.Sum(o => o.TotalPrice)
                };

                return Result<CustomerDetailResponseDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return Result<CustomerDetailResponseDto>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<CustomerDetailResponseDto>> SearchByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return Result<IEnumerable<CustomerDetailResponseDto>>.Success(new List<CustomerDetailResponseDto>());

                var customers = _customerRepository.SearchByName(name)
                    .Where(c => c.DeletedDate == null)
                    .OrderBy(c => c.Name)
                    .Select(c =>
                    {
                        var orders = c.Orders.Where(o => o.DeletedDate == null);
                        return new CustomerDetailResponseDto
                        {
                            CustomerId = c.CustomerId,
                            Name = c.Name,
                            Phone = c.Phone,
                            MemberLevel = c.MemberLevel,
                            LoyaltyPoints = c.LoyaltyPoints,
                            CreatedDate = c.CreatedDate,
                            TotalOrders = orders.Count(),
                            TotalSpent = orders.Sum(o => o.TotalPrice)
                        };
                    })
                    .ToList();

                return Result<IEnumerable<CustomerDetailResponseDto>>.Success(customers);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<CustomerDetailResponseDto>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<CustomerDetailResponseDto>> GetByMemberLevel(MemberTier level)
        {
            try
            {
                var customers = _customerRepository.GetAll()
                    .Where(c => c.DeletedDate == null && c.MemberLevel == level)
                    .OrderBy(c => c.Name)
                    .Select(c =>
                    {
                        var orders = c.Orders.Where(o => o.DeletedDate == null);
                        return new CustomerDetailResponseDto
                        {
                            CustomerId = c.CustomerId,
                            Name = c.Name,
                            Phone = c.Phone,
                            MemberLevel = c.MemberLevel,
                            LoyaltyPoints = c.LoyaltyPoints,
                            CreatedDate = c.CreatedDate,
                            TotalOrders = orders.Count(),
                            TotalSpent = orders.Sum(o => o.TotalPrice)
                        };
                    })
                    .ToList();

                return Result<IEnumerable<CustomerDetailResponseDto>>.Success(customers);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<CustomerDetailResponseDto>>.Fail($"Lỗi: {ex.Message}");
            }
        }
        
        public Result<IEnumerable<CustomerDetailResponseDto>> SearchByTotalSpent(decimal minimum, decimal maximum, DateTime startDate, DateTime endDate)
        {
            try
            {
                var customers = _customerRepository.GetAll()
                    .Where(c => c.DeletedDate == null)
                    .Where(cus =>
                    {
                        var totalSpent = cus.Orders
                            .Where(o => o.CreatedDate >= startDate && o.CreatedDate <= endDate && o.DeletedDate == null)
                            .Sum(o => o.TotalPrice);

                        return totalSpent >= minimum && totalSpent <= maximum;
                    })
                    .OrderBy(c => c.Name)
                    .Select(s =>
                    {
                        var orders = s.Orders.Where(o => o.DeletedDate == null);
                        return new CustomerDetailResponseDto
                        {
                            CustomerId = s.CustomerId,
                            Name = s.Name,
                            Phone = s.Phone,
                            MemberLevel = s.MemberLevel,
                            LoyaltyPoints = s.LoyaltyPoints,
                            CreatedDate = s.CreatedDate,
                            TotalOrders = orders.Count(),
                            TotalSpent = orders.Sum(o => o.TotalPrice)
                        };
                    });

                return Result<IEnumerable<CustomerDetailResponseDto>>.Success(customers);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<CustomerDetailResponseDto>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- QUẢN LÝ ĐIỂM TÍCH LŨY --------------------
        // ==================================================================
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
                customer.UpdatedDate = DateTime.Now;

                _customerRepository.Update(customer);
                _customerRepository.SaveChanges();

                return Result.Success($"Thêm {points} điểm thành công. Tổng: {customer.LoyaltyPoints}");
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
                    return Result.Fail($"Không đủ điểm. Hiện có: {customer.LoyaltyPoints}");

                customer.LoyaltyPoints -= points;
                customer.UpdatedDate = DateTime.Now;

                _customerRepository.Update(customer);
                _customerRepository.SaveChanges();

                return Result.Success($"Sử dụng {points} điểm thành công. Còn lại: {customer.LoyaltyPoints}");
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
                var customer = _customerRepository.GetById(customerId);
                if (customer == null || customer.DeletedDate != null)
                    return Result<decimal>.Fail("Khách hàng không tồn tại");
                    
                return Result<decimal>.Success(customer.LoyaltyPoints);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- QUẢN LÝ HẠN THÀNH VIÊN -------------------
        // ==================================================================
        public Result UpgradeMemberLevel(string customerId)
        {
            try
            {
                var customer = _customerRepository.GetById(customerId);
                if (customer == null || customer.DeletedDate != null)
                    return Result.Fail("Khách hàng không tồn tại");
                
                var currentLevel = customer.MemberLevel;
                
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
                        return Result.Fail("Khách hàng đã ở mức cao nhất");
                }

                customer.UpdatedDate = DateTime.Now;
                _customerRepository.Update(customer);
                _customerRepository.SaveChanges();

                return Result.Success($"Nâng hạng từ {currentLevel} lên {customer.MemberLevel} thành công");
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
                var customer = _customerRepository.GetById(customerId);
                if (customer == null || customer.DeletedDate != null)
                    return Result.Fail("Khách hàng không tồn tại");
                
                var currentLevel = customer.MemberLevel;
                
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
                        return Result.Fail("Khách hàng đã ở mức thấp nhất");
                }

                customer.UpdatedDate = DateTime.Now;
                _customerRepository.Update(customer);
                _customerRepository.SaveChanges();

                return Result.Success($"Hạ hạng từ {currentLevel} xuống {customer.MemberLevel} thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<MemberTier> CalculateMemberLevel(decimal totalSpent)
        {
            try
            {
                MemberTier level;
                
                if (totalSpent >= 10000000) // 10 triệu
                    level = MemberTier.Diamond;
                else if (totalSpent >= 5000000) // 5 triệu
                    level = MemberTier.Gold;
                else if (totalSpent >= 1000000) // 1 triệu
                    level = MemberTier.Silver;
                else
                    level = MemberTier.Bronze;

                return Result<MemberTier>.Success(level);
            }
            catch (Exception ex)
            {
                return Result<MemberTier>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- LỊCH SỬ MUA HÀNG -------------------------
        // ==================================================================

        /// <summary>
        /// Lấy lịch sử mua hàng của khách hàng
        /// </summary>
        public Result<IEnumerable<Order>> GetCustomerOrderHistory(string customerId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var customer = _customerRepository.GetById(customerId);
                if (customer == null || customer.DeletedDate != null)
                    return Result<IEnumerable<Order>>.Fail("Khách hàng không tồn tại");

                var orders = _orderRepository.Find(o =>
                    o.CustomerId == customerId &&
                    o.CreatedDate >= fromDate &&
                    o.CreatedDate <= toDate &&
                    o.DeletedDate == null)
                    .OrderByDescending(o => o.CreatedDate)
                    .ToList();

                return Result<IEnumerable<Order>>.Success(orders);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Order>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Tính tổng tiền đã chi của khách hàng
        /// </summary>
        public Result<decimal> CustomerTotalSpentPerDay(string customerId, DateTime date)
        {
            try
            {
                var customer = _customerRepository.GetById(customerId);
                if (customer == null || customer.DeletedDate != null)
                    return Result<decimal>.Fail("Khách hàng không tồn tại");

                var orders = _orderRepository.Find(o =>
                    o.CustomerId == customerId &&
                    o.DeletedDate == null &&
                    o.CreatedDate == date );

                var totalSpent = orders.Sum(o => o.TotalPrice);
                return Result<decimal>.Success(totalSpent);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }
        
        public Result<decimal> CustomerTotalSpentPerMonth(string customerId, int month, int year)
        {
            try
            {
                var customer = _customerRepository.GetById(customerId);
                if (customer == null || customer.DeletedDate != null)
                    return Result<decimal>.Fail("Khách hàng không tồn tại");

                var orders = _orderRepository.Find(o =>
                    o.CustomerId == customerId &&
                    o.DeletedDate == null &&
                    o.CreatedDate.Month == month &&
                    o.CreatedDate.Year == year);

                var totalSpent = orders.Sum(o => o.TotalPrice);
                return Result<decimal>.Success(totalSpent);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }
        
        public Result<decimal> CustomerTotalSpentPerYear(string customerId, int year)
        {
            try
            {
                var customer = _customerRepository.GetById(customerId);
                if (customer == null || customer.DeletedDate != null)
                    return Result<decimal>.Fail("Khách hàng không tồn tại");

                var orders = _orderRepository.Find(o =>
                    o.CustomerId == customerId &&
                    o.DeletedDate == null &&
                    o.CreatedDate.Year == year);

                var totalSpent = orders.Sum(o => o.TotalPrice);
                return Result<decimal>.Success(totalSpent);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- HÀM HELPER --------------------------------
        // ==================================================================
        private string GenerateCustomerId()
        {
            var lastCustomer = _customerRepository.GetAll()
                .OrderByDescending(c => c.CustomerId)
                .FirstOrDefault();
                
            if (lastCustomer == null || !lastCustomer.CustomerId.StartsWith("KH"))
                return "KH0001";
            
            var lastNumber = int.Parse(lastCustomer.CustomerId.Substring(2));
            return $"KH{(lastNumber + 1):D4}";
        }

        // ==================================================================
        // ----------------------- LIST VIEW METHODS -------------------------
        // ==================================================================
        public Result<IEnumerable<CustomerListResponseDto>> GetCustomerList()
        {
            try
            {
                // Get all active customers (already filtered by DeletedDate in repository)
                var customers = _customerRepository.GetAllForListView().ToList();

                // Map to DTOs (only required ListView fields)
                var result = customers.Select(customer => new CustomerListResponseDto
                {
                    CustomerId = customer.CustomerId,
                    Name = customer.Name,
                    Phone = customer.Phone,
                    MemberLevel = customer.MemberLevel,
                    LoyaltyPoints = customer.LoyaltyPoints
                }).ToList();

                return Result<IEnumerable<CustomerListResponseDto>>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<CustomerListResponseDto>>.Fail($"Lỗi: {ex.Message}");
            }
        }
    }
}