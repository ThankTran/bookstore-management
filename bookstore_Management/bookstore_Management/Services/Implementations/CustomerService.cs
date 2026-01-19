using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.DTOs.Customer.Requests;
using bookstore_Management.DTOs.Customer.Responses;
using bookstore_Management.Models;
using bookstore_Management.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;

namespace bookstore_Management.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;

        internal CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ============================================================
        // ADD CUSTOMER
        // ============================================================
        public async Task<Result<string>> AddCustomerAsync(CreateCustomerRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result<string>.Fail("Tên không được trống");

            if (string.IsNullOrWhiteSpace(dto.Phone) || !dto.Phone.All(char.IsDigit))
                return Result<string>.Fail("Số điện thoại không hợp lệ");

            var existing = await _unitOfWork.Customers.SearchByPhoneAsync(dto.Phone);
            if (existing != null)
                return Result<string>.Fail("Số điện thoại đã tồn tại");

            var customerId = await GenerateCustomerIdAsync();

            var customer = new Customer
            {
                CustomerId = customerId,
                Name = dto.Name.Trim(),
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                LoyaltyPoints = 0,
                MemberLevel = MemberTier.Bronze,
                CreatedDate = DateTime.Now
            };

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.Customers.SaveChangesAsync();

            return Result<string>.Success(customerId, "Thêm khách hàng thành công");
        }

        // ============================================================
        // UPDATE CUSTOMER
        // ============================================================
        public async Task<Result> UpdateCustomerAsync(string customerId, UpdateCustomerRequestDto dto)
        {
            var c = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (c == null || c.DeletedDate != null)
                return Result.Fail("Khách hàng không tồn tại");

            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result.Fail("Tên không được trống");

            if (string.IsNullOrWhiteSpace(dto.Phone) || !dto.Phone.All(char.IsDigit))
                return Result.Fail("Số điện thoại không hợp lệ");

            if (dto.Phone != c.Phone)
            {
                var dup = await _unitOfWork.Customers.SearchByPhoneAsync(dto.Phone);
                if (dup != null && dup.CustomerId != c.CustomerId)
                    return Result.Fail("Số điện thoại đã được sử dụng");
            }

            c.Name = dto.Name;
            c.Phone = dto.Phone;
            c.Email = dto.Email;
            c.Address = dto.Address;

            if (dto.MemberLevel.HasValue) c.MemberLevel = dto.MemberLevel.Value;
            if (dto.LoyaltyPoints.HasValue) c.LoyaltyPoints = dto.LoyaltyPoints.Value;

            c.UpdatedDate = DateTime.Now;

            _unitOfWork.Customers.Update(c);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success("Cập nhật thành công");
        }

        // ============================================================
        // DELETE CUSTOMER
        // ============================================================
        public async Task<Result> DeleteCustomerAsync(string customerId)
        {
            var c = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (c == null || c.DeletedDate != null)
                return Result.Fail("Khách hàng không tồn tại");

            var hasOrders = await _unitOfWork.Orders
                .Query(o => o.CustomerId == customerId && o.DeletedDate == null)
                .AnyAsync();

            if (hasOrders)
                return Result.Fail("Không thể xóa khách có đơn hàng");

            c.DeletedDate = DateTime.Now;

            _unitOfWork.Customers.Update(c);
            await _unitOfWork.Customers.SaveChangesAsync();

            return Result.Success("Đã xóa");
        }

        // ============================================================
        // GET BY ID
        // ============================================================
        public async Task<Result<CustomerDetailResponseDto>> GetCustomerByIdAsync(string customerId)
        {
            var c = await _unitOfWork.Customers.Query(x => x.CustomerId == customerId && x.DeletedDate == null)
                .Select(cus => new CustomerDetailResponseDto
                {
                    CustomerId = cus.CustomerId,
                    Name = cus.Name,
                    Address = cus.Address,
                    Email = cus.Email,
                    Phone = cus.Phone,
                    MemberLevel = cus.MemberLevel,
                    LoyaltyPoints = cus.LoyaltyPoints,
                    CreatedDate = cus.CreatedDate,

                    TotalOrders = _unitOfWork.Orders.Query(o => o.CustomerId == cus.CustomerId && o.DeletedDate == null).Count(),
                    TotalSpent = _unitOfWork.Orders.Query(o => o.CustomerId == cus.CustomerId && o.DeletedDate == null).Sum(o => (decimal?)o.TotalPrice) ?? 0
                })
                .FirstOrDefaultAsync();

            if (c == null)
                return Result<CustomerDetailResponseDto>.Fail("Không tìm thấy");

            return Result<CustomerDetailResponseDto>.Success(c);
        }

        // ============================================================
        // GET ALL
        // ============================================================
        public async Task<Result<IEnumerable<CustomerDetailResponseDto>>> GetAllCustomersAsync()
        {
            var result = await _unitOfWork.Customers.Query(c => c.DeletedDate == null)
                .Select(cus => new CustomerDetailResponseDto
                {
                    CustomerId = cus.CustomerId,
                    Name = cus.Name,
                    Address = cus.Address,
                    Email = cus.Email,
                    Phone = cus.Phone,
                    MemberLevel = cus.MemberLevel,
                    LoyaltyPoints = cus.LoyaltyPoints,
                    CreatedDate = cus.CreatedDate,
                    TotalOrders = _unitOfWork.Orders.Query(o => o.CustomerId == cus.CustomerId).Count(),
                    TotalSpent = _unitOfWork.Orders.Query(o => o.CustomerId == cus.CustomerId).Sum(o => (decimal?)o.TotalPrice) ?? 0
                })
                .OrderBy(x => x.Name)
                .ToListAsync();

            return Result<IEnumerable<CustomerDetailResponseDto>>.Success(result);
        }
        
        public async Task<Result<CustomerDetailResponseDto>> GetCustomerByPhoneAsync(string phone)
        {
            var customer = await _unitOfWork.Customers
                .Query(c => c.Phone == phone && c.DeletedDate == null)
                .FirstOrDefaultAsync();

            if (customer == null)
                return Result<CustomerDetailResponseDto>.Fail("Không tìm thấy");

            return Result<CustomerDetailResponseDto>.Success(new CustomerDetailResponseDto
            {
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                Phone = customer.Phone,
                Email = customer.Email
            });
        }


        // ============================================================
        // SEARCH BY NAME
        // ============================================================
        public async Task<Result<IEnumerable<CustomerDetailResponseDto>>> SearchByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<IEnumerable<CustomerDetailResponseDto>>.Success(new List<CustomerDetailResponseDto>());

            var result = await _unitOfWork.Customers
                .SearchByName(name)
                .Where(c => c.DeletedDate == null)
                .OrderBy(c => c.Name)
                .Select(cus => new CustomerDetailResponseDto
                {
                    CustomerId = cus.CustomerId,
                    Name = cus.Name,
                    Phone = cus.Phone,
                    Email = cus.Email,
                    Address = cus.Address,
                    LoyaltyPoints = cus.LoyaltyPoints,
                    MemberLevel = cus.MemberLevel,
                    CreatedDate = cus.CreatedDate,
                    TotalOrders = _unitOfWork.Orders.Query(o => o.CustomerId == cus.CustomerId).Count(),
                    TotalSpent = _unitOfWork.Orders.Query(o => o.CustomerId == cus.CustomerId).Sum(o => (decimal?)o.TotalPrice) ?? 0
                })
                .ToListAsync();

            return Result<IEnumerable<CustomerDetailResponseDto>>.Success(result);
        }

        // ============================================================
        // GET BY MEMBER LEVEL
        // ============================================================
        public async Task<Result<IEnumerable<CustomerDetailResponseDto>>> GetByMemberLevelAsync(MemberTier level)
        {
            var result = await _unitOfWork.Customers
                .Query(c => c.MemberLevel == level && c.DeletedDate == null)
                .OrderBy(c => c.Name)
                .Select(cus => new CustomerDetailResponseDto
                {
                    CustomerId = cus.CustomerId,
                    Name = cus.Name,
                    Phone = cus.Phone,
                    Email = cus.Email,
                    Address = cus.Address,
                    LoyaltyPoints = cus.LoyaltyPoints,
                    MemberLevel = cus.MemberLevel,
                    CreatedDate = cus.CreatedDate,
                    TotalOrders = _unitOfWork.Orders.Query(o => o.CustomerId == cus.CustomerId).Count(),
                    TotalSpent = _unitOfWork.Orders.Query(o => o.CustomerId == cus.CustomerId).Sum(o => (decimal?)o.TotalPrice) ?? 0
                })
                .ToListAsync();

            return Result<IEnumerable<CustomerDetailResponseDto>>.Success(result);
        }

        // ============================================================
        // SEARCH BY TOTAL SPENT
        // ============================================================
        public async Task<Result<IEnumerable<CustomerDetailResponseDto>>> SearchByTotalSpentAsync(
            decimal min, decimal max, DateTime start, DateTime end)
        {
            var result = await _unitOfWork.Customers.Query(c => c.DeletedDate == null)
                .Select(cus => new
                {
                    Customer = cus,
                    TotalSpent = _unitOfWork.Orders.Query(o =>
                        o.CustomerId == cus.CustomerId &&
                        o.DeletedDate == null &&
                        o.CreatedDate >= start &&
                        o.CreatedDate <= end
                    ).Sum(o => (decimal?)o.TotalPrice) ?? 0
                })
                .Where(x => x.TotalSpent >= min && x.TotalSpent <= max)
                .Select(x => new CustomerDetailResponseDto
                {
                    CustomerId = x.Customer.CustomerId,
                    Name = x.Customer.Name,
                    Phone = x.Customer.Phone,
                    Email = x.Customer.Email,
                    Address = x.Customer.Address,
                    LoyaltyPoints = x.Customer.LoyaltyPoints,
                    MemberLevel = x.Customer.MemberLevel,
                    CreatedDate = x.Customer.CreatedDate,
                    TotalSpent = x.TotalSpent,
                    TotalOrders = _unitOfWork.Orders.Query(o => o.CustomerId == x.Customer.CustomerId).Count()
                })
                .ToListAsync();

            return Result<IEnumerable<CustomerDetailResponseDto>>.Success(result);
        }

        // ============================================================
        // MEMBER LEVEL UP/DOWN
        // ============================================================
        public async Task<Result> UpgradeMemberLevelAsync(string customerId)
        {
            var c = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (c == null || c.DeletedDate != null)
                return Result.Fail("Không tìm thấy");

            if (c.MemberLevel == MemberTier.Diamond)
                return Result.Fail("Đã cao nhất");

            c.MemberLevel += 1;
            c.UpdatedDate = DateTime.Now;

            _unitOfWork.Customers.Update(c);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success("Nâng hạng thành công");
        }

        public async Task<Result> DowngradeMemberLevelAsync(string customerId)
        {
            var c = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (c == null || c.DeletedDate != null)
                return Result.Fail("Không tìm thấy");

            if (c.MemberLevel == MemberTier.Bronze)
                return Result.Fail("Đã thấp nhất");

            c.MemberLevel -= 1;
            c.UpdatedDate = DateTime.Now;

            _unitOfWork.Customers.Update(c);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success("Hạ hạng thành công");
        }
        
        public Task<Result<MemberTier>> CalculateMemberTierAsync(decimal totalSpent)
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

            return Task.FromResult(Result<MemberTier>.Success(level));
        }

        // ============================================================
        // TOTAL SPENT
        // ============================================================
        public async Task<Result<decimal>> CustomerTotalSpentPerDayAsync(string customerId, DateTime date)
        {
            var total = await _unitOfWork.Orders.Query(o =>
                o.CustomerId == customerId &&
                o.DeletedDate == null &&
                DbFunctions.TruncateTime(o.CreatedDate) == date.Date
            ).SumAsync(o => (decimal?)o.TotalPrice) ?? 0;

            return Result<decimal>.Success(total);
        }

        public async Task<Result<decimal>> CustomerTotalSpentPerMonthAsync(string customerId, int month, int year)
        {
            var total = await _unitOfWork.Orders.Query(o =>
                o.CustomerId == customerId &&
                o.DeletedDate == null &&
                o.CreatedDate.Month == month &&
                o.CreatedDate.Year == year
            ).SumAsync(o => (decimal?)o.TotalPrice) ?? 0;

            return Result<decimal>.Success(total);
        }

        public async Task<Result<decimal>> CustomerTotalSpentPerYearAsync(string customerId, int year)
        {
            var total = await _unitOfWork.Orders.Query(o =>
                o.CustomerId == customerId &&
                o.DeletedDate == null &&
                o.CreatedDate.Year == year
            ).SumAsync(o => (decimal?)o.TotalPrice) ?? 0;

            return Result<decimal>.Success(total);
        }
        
        public async Task<Result<IEnumerable<Order>>> GetCustomerOrderHistoryAsync(string customerId, DateTime from, DateTime to)
        {
            var orders = await _unitOfWork.Orders
                .Query(o => o.CustomerId == customerId && o.CreatedDate >= from && o.CreatedDate <= to)
                .ToListAsync();

            return Result<IEnumerable<Order>>.Success(orders);
        }



        // ============================================================
        // HELPER FUNCTIONS
        // ============================================================
        private async Task<string> GenerateCustomerIdAsync()
        {
            var last = await _unitOfWork.Customers.Query(x => x.CustomerId.StartsWith("KH"))
                .OrderByDescending(x => x.CustomerId)
                .FirstOrDefaultAsync();

            if (last == null)
                return "KH0001";

            var number = int.Parse(last.CustomerId.Substring(2)) + 1;
            return $"KH{number:D4}";
        }
    }
}
