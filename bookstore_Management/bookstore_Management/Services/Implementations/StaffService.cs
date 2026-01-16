using System;
using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.DTOs.Staff.Requests;
using bookstore_Management.DTOs.Staff.Responses;
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
        public Result<string> AddStaff(CreateStaffRequestDto dto)
        {
            try
            {
                // Validate
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result<string>.Fail("Tên không được trống");
                
                
                // Gen Id
                var staffId = GenerateStaffId();
                
                var staff = new Staff
                {
                    Id = staffId,
                    Name = dto.Name.Trim(),
                    CitizenId = dto.CitizenId,
                    Phone = dto.Phone,
                    UserRole = dto.UserRole,
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
        public Result UpdateStaff(string staffId, UpdateStaffRequestDto dto)
        {
            try
            {
                var staff = _staffRepository.GetById(staffId);
                if (staff == null || staff.DeletedDate != null)
                    return Result.Fail("Nhân viên không tồn tại");
                
                // Validate
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result.Fail("Tên không được trống");
                
                
                staff.Name = dto.Name.Trim();
                staff.CitizenId = dto.CitizenId;
                staff.Phone = dto.Phone;
                if (dto.UserRole.HasValue)
                    staff.UserRole = dto.UserRole.Value;
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
        public Result<StaffResponseDto> GetStaffById(string staffId)
        {
            try
            {
                var staff = _staffRepository.GetById(staffId);
                if (staff == null || staff.DeletedDate != null)
                    return Result<StaffResponseDto>.Fail("Nhân viên không tồn tại");
                    
                var dto = MapToStaffResponseDto(staff);
                return Result<StaffResponseDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return Result<StaffResponseDto>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<StaffResponseDto>> GetAllStaff()
        {
            try
            {
                var staff = _staffRepository.GetAll()
                    .Where(s => s.DeletedDate == null)
                    .OrderBy(s => s.Name)
                    .Select(MapToStaffResponseDto);
                return Result<IEnumerable<StaffResponseDto>>.Success(staff);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<StaffResponseDto>>.Fail($"Lỗi: {ex.Message}");
            }
        }
        

        public Result<IEnumerable<StaffResponseDto>> GetByRole(UserRole userRole)
        {
            try
            {
                var staff = _staffRepository.GetByRole(userRole)
                    .Where(s => s.DeletedDate == null)
                    .OrderBy(s => s.Name)
                    .Select(MapToStaffResponseDto);
                return Result<IEnumerable<StaffResponseDto>>.Success(staff);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<StaffResponseDto>>.Fail($"Lỗi: {ex.Message}");
            }
        }
        
        // ==================================================================
        // ----------------------- Hàm Logic - ------------------------------
        // ==================================================================

        public Result ChangeRole(string staffId, UserRole newUserRole)
        {
            try
            {
                var staff = _staffRepository.GetById(staffId);
                if (staff == null || staff.DeletedDate != null)
                    return Result.Fail("Nhân viên không tồn tại");
                
                var oldRole = staff.UserRole;
                staff.UserRole = newUserRole;
                staff.UpdatedDate = DateTime.Now;
                _staffRepository.Update(staff);
                _staffRepository.SaveChanges();
                
                return Result.Success($"Đổi vai trò từ {oldRole} thành {newUserRole} thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- HÀM HELPER --------------------------------
        // ==================================================================
        /// <summary>
        /// Maps Staff entity to StaffResponseDto
        /// </summary>
        private StaffResponseDto MapToStaffResponseDto(Staff staff)
        {
            var totalOrders = _orderRepository.GetByStaff(staff.Id)
                .Count(o => o.DeletedDate == null);

            return new StaffResponseDto
            {
                Id = staff.Id,
                Name = staff.Name,
                CitizenId = staff.CitizenId,
                Phone = staff.Phone,
                UserRole = staff.UserRole,
                CreatedDate = staff.CreatedDate,
                TotalOrders = totalOrders
            };
        }

        private string GenerateStaffId()
        {
            var lastStaff = _staffRepository.GetAll()
                .OrderByDescending(s => s.Id)
                .FirstOrDefault();
                
            if (lastStaff == null || !lastStaff.Id.StartsWith("NV"))
                return "NV0001";
                
            var lastNumber = int.Parse(lastStaff.Id.Substring(2));
            return $"NV{(lastNumber + 1):D4}";
        }
    }
}