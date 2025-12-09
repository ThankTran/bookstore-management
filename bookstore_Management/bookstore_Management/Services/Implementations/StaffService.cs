using System;
using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.DTOs;
using bookstore_Management.Models;
using bookstore_Management.Services.Interfaces;

namespace bookstore_Management.Services.Implementations
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IOrderRepository _orderRepository;

        internal StaffService(
            IStaffRepository staffRepository,
            IOrderRepository orderRepository)
        {
            _staffRepository = staffRepository;
            _orderRepository = orderRepository;
        }

        // ==================================================================
        // ---------------------- THÊM DỮ LIỆU ------------------------------
        // ==================================================================
        public Result<string> AddStaff(StaffDto dto)
        {
            try
            {
                // Validate
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result<string>.Fail("Tên không được trống");
                    
                if (string.IsNullOrWhiteSpace(dto.Phone))
                    return Result<string>.Fail("Số điện thoại không được trống");
                    
                if (dto.Phone.Length < 10 || dto.Phone.Length > 20 || !dto.Phone.All(char.IsDigit))
                    return Result<string>.Fail("Số điện thoại phải từ 10-20 chữ số");
                    
                if (string.IsNullOrWhiteSpace(dto.CitizenIdCard))
                    return Result<string>.Fail("CCCD không được trống");

                if (dto.CitizenIdCard.Length < 9 || dto.CitizenIdCard.Length > 12 || !dto.CitizenIdCard.All(char.IsDigit))
                    return Result<string>.Fail("CCCD phải từ 9-12 chữ số");
                    
                if (dto.BaseSalary <= 0)
                    return Result<string>.Fail("Lương cơ bản phải > 0");
                
                // Check duplicate phone
                var existingPhone = _staffRepository.GetByPhone(dto.Phone);
                if (existingPhone != null && existingPhone.DeletedDate == null)
                    return Result<string>.Fail("Số điện thoại đã được đăng ký");
                
                // Check duplicate citizen ID
                var existingCitizen = _staffRepository.GetById(dto.CitizenIdCard);
                if (existingCitizen != null && existingCitizen.DeletedDate == null)
                    return Result<string>.Fail("CCCD đã được đăng ký");
                
                // Generate Staff ID
                string staffId = GenerateStaffId();
                
                // Create staff
                var staff = new Staff
                {
                    Id = staffId,
                    Name = dto.Name.Trim(),
                    BaseSalary = dto.BaseSalary,
                    CitizenIdCard = dto.CitizenIdCard.Trim(),
                    Phone = dto.Phone.Trim(),
                    Status = dto.Status,
                    Role = dto.Role,
                    SalaryRate = dto.SalaryRate,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = null,
                    DeletedDate = null
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

        // ==================================================================
        // ----------------------- SỬA DỮ LIỆU ------------------------------
        // ==================================================================
        public Result UpdateStaff(string staffId, StaffDto dto)
        {
            try
            {
                var staff = _staffRepository.GetById(staffId);
                if (staff == null || staff.DeletedDate != null)
                    return Result.Fail("Nhân viên không tồn tại");
                
                // Validate
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result.Fail("Tên không được trống");

                if (string.IsNullOrWhiteSpace(dto.Phone))
                    return Result.Fail("Số điện thoại không được trống");

                if (dto.Phone.Length < 10 || dto.Phone.Length > 20 || !dto.Phone.All(char.IsDigit))
                    return Result.Fail("Số điện thoại phải từ 10-20 chữ số");

                if (string.IsNullOrWhiteSpace(dto.CitizenIdCard))
                    return Result.Fail("CCCD không được trống");

                if (dto.CitizenIdCard.Length < 9 || dto.CitizenIdCard.Length > 12 || !dto.CitizenIdCard.All(char.IsDigit))
                    return Result.Fail("CCCD phải từ 9-12 chữ số");

                if (dto.BaseSalary <= 0)
                    return Result.Fail("Lương cơ bản phải > 0");
                
                // Validate phone (if changed)
                if (dto.Phone != staff.Phone)
                {
                    var existing = _staffRepository.GetByPhone(dto.Phone);
                    if (existing != null && existing.Id != staffId && existing.DeletedDate == null)
                        return Result.Fail("Số điện thoại đã được sử dụng");
                }
                
                // Validate citizen ID (if changed)
                if (dto.CitizenIdCard != staff.CitizenIdCard)
                {
                    var existing = _staffRepository.GetById(dto.CitizenIdCard);
                    if (existing != null && existing.Id != staffId && existing.DeletedDate == null)
                        return Result.Fail("CCCD đã được sử dụng");
                }
                
                staff.Name = dto.Name.Trim();
                staff.BaseSalary = dto.BaseSalary;
                staff.CitizenIdCard = dto.CitizenIdCard.Trim();
                staff.Phone = dto.Phone.Trim();
                staff.Status = dto.Status;
                staff.Role = dto.Role;
                staff.SalaryRate = dto.SalaryRate;
                staff.UpdatedDate = DateTime.Now;
                
                _staffRepository.Update(staff);
                _staffRepository.SaveChanges();
                
                return Result.Success("Cập nhật nhân viên thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ---------------------- XÓA DỮ LIỆU -------------------------------
        // ==================================================================
        public Result DeleteStaff(string staffId)
        {
            try
            {
                var staff = _staffRepository.GetById(staffId);
                if (staff == null || staff.DeletedDate != null)
                    return Result.Fail("Nhân viên không tồn tại");
                
                // Check if staff has orders
                var staffOrders = _orderRepository.Find(o => o.StaffId == staffId && o.DeletedDate == null);
                if (staffOrders.Any())
                    return Result.Fail("Không thể xóa nhân viên đã có đơn hàng");
                
                // Soft delete
                staff.DeletedDate = DateTime.Now;
                _staffRepository.Update(staff);
                _staffRepository.SaveChanges();
                
                return Result.Success("Xóa nhân viên thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- LẤY DỮ LIỆU ------------------------------
        // ==================================================================
        public Result<Staff> GetStaffById(string staffId)
        {
            try
            {
                var staff = _staffRepository.GetById(staffId);
                if (staff == null || staff.DeletedDate != null)
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
                var staff = _staffRepository.GetAll()
                    .Where(s => s.DeletedDate == null)
                    .OrderBy(s => s.Name)
                    .ToList();
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
                if (staff == null || staff.DeletedDate != null)
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
                var staff = _staffRepository.GetById(citizenId);
                if (staff == null || staff.DeletedDate != null)
                    return Result<Staff>.Fail("Không tìm thấy nhân viên");
                    
                return Result<Staff>.Success(staff);
            }
            catch (Exception ex)
            {
                return Result<Staff>.Fail($"Lỗi: {ex.Message}");
            }
        }
        

        public Result<IEnumerable<Staff>> GetByRole(Role role)
        {
            try
            {
                var staff = _staffRepository.GetByRole(role)
                    .Where(s => s.DeletedDate == null)
                    .OrderBy(s => s.Name)
                    .ToList();
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
                var staff = _staffRepository.Find(s => 
                    s.Status == status && s.DeletedDate == null)
                    .OrderBy(s => s.Name)
                    .ToList();
                return Result<IEnumerable<Staff>>.Success(staff);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Staff>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ---------------------- QUẢN LÝ LƯƠNG -----------------------------
        // ==================================================================
        public Result<decimal> CalculateSalary(string staffId, int workingDays)
        {
            try
            {
                var staff = _staffRepository.GetById(staffId);
                if (staff == null || staff.DeletedDate != null)
                    return Result<decimal>.Fail("Nhân viên không tồn tại");
                
                if (workingDays < 0 || workingDays > 31)
                    return Result<decimal>.Fail("Số ngày công không hợp lệ (0-31)");
                
                // Calculate: (BaseSalary / 26 working days) * working days * salary rate
                decimal dailyRate = staff.BaseSalary / 26m;
                decimal baseSalary = dailyRate * workingDays;
                decimal totalSalary = baseSalary * staff.SalaryRate;
                
                return Result<decimal>.Success(totalSalary, 
                    $"Lương: {totalSalary:N0} VND (Ngày công: {workingDays}, Hệ số: {staff.SalaryRate})");
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result UpdateBaseSalary(string staffId, decimal newBaseSalary)
        {
            try
            {
                var staff = _staffRepository.GetById(staffId);
                if (staff == null || staff.DeletedDate != null)
                    return Result.Fail("Nhân viên không tồn tại");
                
                if (newBaseSalary <= 0)
                    return Result.Fail("Lương cơ bản phải > 0");
                
                var oldSalary = staff.BaseSalary;
                staff.BaseSalary = newBaseSalary;
                staff.UpdatedDate = DateTime.Now;
                _staffRepository.Update(staff);
                _staffRepository.SaveChanges();
                
                return Result.Success($"Cập nhật lương thành công. Cũ: {oldSalary:N0}, Mới: {newBaseSalary:N0}");
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
                if (staff == null || staff.DeletedDate != null)
                    return Result.Fail("Nhân viên không tồn tại");
                
                if (newRate <= 0 || newRate > 10.0m)
                    return Result.Fail("Hệ số lương phải từ 0.1 đến 10.0");
                
                var oldRate = staff.SalaryRate;
                staff.SalaryRate = newRate;
                staff.UpdatedDate = DateTime.Now;
                _staffRepository.Update(staff);
                _staffRepository.SaveChanges();
                
                return Result.Success($"Cập nhật hệ số lương thành công. Cũ: {oldRate}, Mới: {newRate}");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ---------------------- QUẢN LÝ TRẠNG THÁI -------------------------
        // ==================================================================
        public Result ChangeStatus(string staffId, StaffStatus newStatus)
        {
            try
            {
                var staff = _staffRepository.GetById(staffId);
                if (staff == null || staff.DeletedDate != null)
                    return Result.Fail("Nhân viên không tồn tại");
                
                var oldStatus = staff.Status;
                staff.Status = newStatus;
                staff.UpdatedDate = DateTime.Now;
                _staffRepository.Update(staff);
                _staffRepository.SaveChanges();
                
                return Result.Success($"Đổi trạng thái từ {oldStatus} thành {newStatus} thành công");
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
                if (staff == null || staff.DeletedDate != null)
                    return Result.Fail("Nhân viên không tồn tại");
                
                var oldRole = staff.Role;
                staff.Role = newRole;
                staff.UpdatedDate = DateTime.Now;
                _staffRepository.Update(staff);
                _staffRepository.SaveChanges();
                
                return Result.Success($"Đổi vai trò từ {oldRole} thành {newRole} thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- HÀM HELPER --------------------------------
        // ==================================================================
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