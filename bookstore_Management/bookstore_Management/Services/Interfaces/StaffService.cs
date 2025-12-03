using System;
using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Repositories;
using bookstore_Management.DTOs;
using bookstore_Management.Models;
using bookstore_Management.Services.Interfaces;

namespace bookstore_Management.Services
{
     public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IStaffDailyRevenueRepository _revenueRepository;

        public StaffService(
            IStaffRepository staffRepository,
            IOrderRepository orderRepository,
            IStaffDailyRevenueRepository revenueRepository)
        {
            _staffRepository = staffRepository;
            _orderRepository = orderRepository;
            _revenueRepository = revenueRepository;
        }

        public Result<string> AddStaff(StaffDto dto)
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
                    
                if (string.IsNullOrWhiteSpace(dto.CitizenIdCard))
                    return Result<string>.Fail("CCCD không được trống");
                    
                if (dto.BaseSalary < 3000000)
                    return Result<string>.Fail("Lương cơ bản tối thiểu 3,000,000");
                
                // Check duplicate phone
                var existingPhone = _staffRepository.GetByPhone(dto.Phone);
                if (existingPhone != null)
                    return Result<string>.Fail("Số điện thoại đã được đăng ký");
                
                // Check duplicate citizen ID
                var existingCitizen = _staffRepository.GetByCitizenId(dto.CitizenIdCard);
                if (existingCitizen != null)
                    return Result<string>.Fail("CCCD đã được đăng ký");
                
                // Generate Staff ID
                string staffId = GenerateStaffId();
                
                // Create staff
                var staff = new Staff
                {
                    Id = staffId,
                    Name = dto.Name.Trim(),
                    BaseSalary = dto.BaseSalary,
                    CitizenIdCard = dto.CitizenIdCard,
                    Phone = dto.Phone,
                    Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim(),
                    Status = dto.Status,
                    Role = dto.Role,
                    SalaryRate = dto.SalaryRate
                };
                
                _staffRepository.Add(staff);
                _staffRepository.SaveChanges();
                
                return Result<string>.Success(staffId, "Thêm nhân viên thành công");
            }
            catch (Exception ex)
            {
                return Result<string>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result UpdateStaff(string staffId, StaffDto dto)
        {
            try
            {
                var staff = _staffRepository.GetById(staffId);
                if (staff == null)
                    return Result.Fail("Nhân viên không tồn tại");
                
                // Validate phone (if changed)
                if (dto.Phone != staff.Phone)
                {
                    var existing = _staffRepository.GetByPhone(dto.Phone);
                    if (existing != null && existing.Id != staffId)
                        return Result.Fail("Số điện thoại đã được sử dụng");
                }
                
                // Validate citizen ID (if changed)
                if (dto.CitizenIdCard != staff.CitizenIdCard)
                {
                    var existing = _staffRepository.GetByCitizenId(dto.CitizenIdCard);
                    if (existing != null && existing.Id != staffId)
                        return Result.Fail("CCCD đã được sử dụng");
                }
                
                staff.Name = dto.Name.Trim();
                staff.BaseSalary = dto.BaseSalary;
                staff.CitizenIdCard = dto.CitizenIdCard;
                staff.Phone = dto.Phone;
                staff.Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim();
                staff.Status = dto.Status;
                staff.Role = dto.Role;
                staff.SalaryRate = dto.SalaryRate;
                
                _staffRepository.Update(staff);
                _staffRepository.SaveChanges();
                
                return Result.Success("Cập nhật nhân viên thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result DeleteStaff(string staffId)
        {
            try
            {
                var staff = _staffRepository.GetById(staffId);
                if (staff == null)
                    return Result.Fail("Nhân viên không tồn tại");
                
                // Check if staff has orders
                var staffOrders = _orderRepository.Find(o => o.StaffId == staffId);
                if (staffOrders.Any())
                    return Result.Fail("Không thể xóa nhân viên đã có đơn hàng");
                
                _staffRepository.Delete(staffId);
                _staffRepository.SaveChanges();
                
                return Result.Success("Xóa nhân viên thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<Staff> GetStaffById(string staffId)
        {
            try
            {
                var staff = _staffRepository.GetById(staffId);
                if (staff == null)
                    return Result<Staff>.Fail("Nhân viên không tồn tại");
                    
                return Result<Staff>.Success(staff);
            }
            catch (Exception ex)
            {
                return Result<Staff>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<Staff>> GetAllStaff()
        {
            try
            {
                var staff = _staffRepository.GetAll();
                return Result<IEnumerable<Staff>>.Success(staff);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Staff>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<Staff> GetStaffByPhone(string phone)
        {
            try
            {
                var staff = _staffRepository.GetByPhone(phone);
                if (staff == null)
                    return Result<Staff>.Fail("Không tìm thấy nhân viên");
                    
                return Result<Staff>.Success(staff);
            }
            catch (Exception ex)
            {
                return Result<Staff>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<Staff> GetStaffByCitizenId(string citizenId)
        {
            try
            {
                var staff = _staffRepository.GetByCitizenId(citizenId);
                if (staff == null)
                    return Result<Staff>.Fail("Không tìm thấy nhân viên");
                    
                return Result<Staff>.Success(staff);
            }
            catch (Exception ex)
            {
                return Result<Staff>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<Staff>> SearchByName(string name)
        {
            try
            {
                var staff = _staffRepository.SearchByName(name);
                return Result<IEnumerable<Staff>>.Success(staff);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Staff>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<Staff>> GetByRole(Role role)
        {
            try
            {
                var staff = _staffRepository.GetByRole(role);
                return Result<IEnumerable<Staff>>.Success(staff);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Staff>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<Staff>> GetByStatus(StaffStatus status)
        {
            try
            {
                var staff = _staffRepository.Find(s => s.Status == status);
                return Result<IEnumerable<Staff>>.Success(staff);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Staff>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<decimal> CalculateSalary(string staffId, int workingDays)
        {
            try
            {
                var staff = _staffRepository.GetById(staffId);
                if (staff == null)
                    return Result<decimal>.Fail("Nhân viên không tồn tại");
                
                if (workingDays < 0 || workingDays > 31)
                    return Result<decimal>.Fail("Số ngày công không hợp lệ");
                
                // Basic salary calculation
                decimal dailyRate = staff.BaseSalary / 26; // Assuming 26 working days/month
                decimal baseSalary = dailyRate * workingDays;
                
                // Apply salary rate
                decimal totalSalary = baseSalary * staff.SalaryRate;
                
                return Result<decimal>.Success(totalSalary);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result UpdateSalary(string staffId, decimal newBaseSalary)
        {
            try
            {
                var staff = _staffRepository.GetById(staffId);
                if (staff == null)
                    return Result.Fail("Nhân viên không tồn tại");
                
                if (newBaseSalary < 3000000)
                    return Result.Fail("Lương cơ bản tối thiểu 3,000,000");
                
                staff.BaseSalary = newBaseSalary;
                _staffRepository.Update(staff);
                _staffRepository.SaveChanges();
                
                return Result.Success("Cập nhật lương thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result UpdateSalaryRate(string staffId, decimal newRate)
        {
            try
            {
                var staff = _staffRepository.GetById(staffId);
                if (staff == null)
                    return Result.Fail("Nhân viên không tồn tại");
                
                if (newRate < 0.1m || newRate > 10.0m)
                    return Result.Fail("Hệ số lương phải từ 0.1 đến 10.0");
                
                staff.SalaryRate = newRate;
                _staffRepository.Update(staff);
                _staffRepository.SaveChanges();
                
                return Result.Success("Cập nhật hệ số lương thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result ChangeStatus(string staffId, StaffStatus newStatus)
        {
            try
            {
                var staff = _staffRepository.GetById(staffId);
                if (staff == null)
                    return Result.Fail("Nhân viên không tồn tại");
                
                staff.Status = newStatus;
                _staffRepository.Update(staff);
                _staffRepository.SaveChanges();
                
                return Result.Success($"Đổi trạng thái thành {newStatus} thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result ChangeRole(string staffId, Role newRole)
        {
            try
            {
                var staff = _staffRepository.GetById(staffId);
                if (staff == null)
                    return Result.Fail("Nhân viên không tồn tại");
                
                staff.Role = newRole;
                _staffRepository.Update(staff);
                _staffRepository.SaveChanges();
                
                return Result.Success($"Đổi vai trò thành {newRole} thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        // Additional method for revenue tracking
        public Result AddDailyRevenue(string staffId, DateTime date, decimal revenue)
        {
            try
            {
                var staff = _staffRepository.GetById(staffId);
                if (staff == null)
                    return Result.Fail("Nhân viên không tồn tại");
                
                var dailyRevenue = new StaffDailyRevenue
                {
                    EmployeeId = staffId,
                    Day = date.Date,
                    Revenue = revenue
                };
                
                _revenueRepository.Add(dailyRevenue);
                _revenueRepository.SaveChanges();
                
                return Result.Success("Thêm doanh thu ngày thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<decimal> GetMonthlyRevenue(string staffId, int month, int year)
        {
            try
            {
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                
                var revenues = _revenueRepository
                    .Find(r => r.EmployeeId == staffId && 
                               r.Day >= startDate && r.Day <= endDate);
                
                decimal total = revenues.Sum(r => r.Revenue);
                return Result<decimal>.Success(total);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

        private string GenerateStaffId()
        {
            var lastStaff = _staffRepository.GetAll()
                .OrderByDescending(s => s.Id)
                .FirstOrDefault();
                
            if (lastStaff == null || !lastStaff.Id.StartsWith("NV"))
                return "NV0001";
                
            int lastNumber = int.Parse(lastStaff.Id.Substring(2));
            return $"NV{(lastNumber + 1):D4}";
        }
    }
}