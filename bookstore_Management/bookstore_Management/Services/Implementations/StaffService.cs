using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IUnitOfWork _unitOfWork;

        internal StaffService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ==================================================================
        // ---------------------- THÊM DỮ LIỆU ------------------------------
        // ==================================================================
        public async Task<Result<string>> AddStaffAsync(CreateStaffRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result<string>.Fail("Tên không được trống");
            
            var staffId = await GenerateStaffIdAsync();
            
            var staff = new Staff
            {
                Id = staffId,
                Name = dto.Name.Trim(),
                CitizenId = dto.CitizenId?.Trim(),
                Phone = dto.Phone,
                UserRole = dto.UserRole,
                CreatedDate = DateTime.Now,
                UpdatedDate = null,
                DeletedDate = null
            };
            
            await _unitOfWork.Staffs.AddAsync(staff);
            await _unitOfWork.SaveChangesAsync();
            
            return Result<string>.Success(staffId, "Thêm nhân viên thành công");
        }

        // ==================================================================
        // ----------------------- SỬA DỮ LIỆU ------------------------------
        // ==================================================================
        public async Task<Result> UpdateStaffAsync(string staffId, UpdateStaffRequestDto dto)
        {
            var staff = await _unitOfWork.Staffs.GetByIdAsync(staffId);
            if (staff == null || staff.DeletedDate != null)
                return Result.Fail("Nhân viên không tồn tại");
            
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result.Fail("Tên không được trống");
            
            staff.Name = dto.Name.Trim();
            staff.CitizenId = dto.CitizenId?.Trim();
            staff.Phone = dto.Phone;
            if (dto.UserRole.HasValue)
                staff.UserRole = dto.UserRole.Value;
            staff.UpdatedDate = DateTime.Now;
            
            _unitOfWork.Staffs.Update(staff);
            await _unitOfWork.SaveChangesAsync();
            
            return Result.Success("Cập nhật nhân viên thành công");
        }

        // ==================================================================
        // ---------------------- XÓA DỮ LIỆU -------------------------------
        // ==================================================================
        public async Task<Result> DeleteStaffAsync(string staffId)
        {
            var staff = await _unitOfWork.Staffs.GetByIdAsync(staffId);
            if (staff == null || staff.DeletedDate != null)
                return Result.Fail("Nhân viên không tồn tại");

            staff.DeletedDate = DateTime.Now;
            _unitOfWork.Staffs.Update(staff);
            await _unitOfWork.SaveChangesAsync();
            
            return Result.Success("Xóa nhân viên thành công");
        }

        // ==================================================================
        // ----------------------- LẤY DỮ LIỆU ------------------------------
        // ==================================================================
        public async Task<Result<StaffResponseDto>> GetStaffByIdAsync(string staffId)
        {
            var staff = await _unitOfWork.Staffs.GetByIdAsync(staffId);
            if (staff == null || staff.DeletedDate != null)
                return Result<StaffResponseDto>.Fail("Nhân viên không tồn tại");
                
            var dto = await MapToStaffResponseDtoAsync(staff);
            return Result<StaffResponseDto>.Success(dto);
        }

        public async Task<Result<IEnumerable<StaffResponseDto>>> GetAllStaffAsync()
        {
            var allStaff = await _unitOfWork.Staffs.GetAllAsync();
            var staffList = allStaff
                .Where(s => s.DeletedDate == null)
                .OrderBy(s => s.Name)
                .ToList();

            var tasks = staffList.Select(MapToStaffResponseDtoAsync);
            var result = await Task.WhenAll(tasks);

            return Result<IEnumerable<StaffResponseDto>>.Success(result);
        }

        public async Task<Result<IEnumerable<StaffResponseDto>>> GetByRoleAsync(UserRole userRole)
        {
            var allStaff = await _unitOfWork.Staffs.GetByRoleAsync(userRole);
            var staffList = allStaff
                .Where(s => s.DeletedDate == null)
                .OrderBy(s => s.Name)
                .ToList();

            var tasks = staffList.Select(MapToStaffResponseDtoAsync);
            var result = await Task.WhenAll(tasks);

            return Result<IEnumerable<StaffResponseDto>>.Success(result);
        }

        public async Task<Result<IEnumerable<StaffResponseDto>>> SearchByNameAsync(string name)
        {
            var allStaff = await Task.Run(() => 
                _unitOfWork.Staffs.SearchByName(name)
                    .Where(s => s.DeletedDate == null)
                    .OrderBy(s => s.Name)
                    .ToList()
            );

            var tasks = allStaff.Select(MapToStaffResponseDtoAsync);
            var result = await Task.WhenAll(tasks);

            return Result<IEnumerable<StaffResponseDto>>.Success(result);
        }
        
        // ==================================================================
        // ----------------------- Hàm Logic --------------------------------
        // ==================================================================
        public async Task<Result> ChangeRoleAsync(string staffId, UserRole newUserRole)
        {
            var staff = await _unitOfWork.Staffs.GetByIdAsync(staffId);
            if (staff == null || staff.DeletedDate != null)
                return Result.Fail("Nhân viên không tồn tại");
            
            var oldRole = staff.UserRole;
            staff.UserRole = newUserRole;
            staff.UpdatedDate = DateTime.Now;
            _unitOfWork.Staffs.Update(staff);
            await _unitOfWork.SaveChangesAsync();
            
            return Result.Success($"Đổi vai trò từ {oldRole} thành {newUserRole} thành công");
        }

        // ==================================================================
        // ----------------------- HÀM HELPER --------------------------------
        // ==================================================================
        private async Task<StaffResponseDto> MapToStaffResponseDtoAsync(Staff staff)
        {
            // Tối ưu: Sử dụng CountAsync thay vì GetAllAsync rồi filter
            var totalOrders = await _unitOfWork.Orders.CountAsync(o => 
                o.StaffId == staff.Id && o.DeletedDate == null);

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

        private async Task<string> GenerateStaffIdAsync()
        {
            var allStaff = await _unitOfWork.Staffs.GetAllAsync();
            var lastStaff = allStaff
                .OrderByDescending(s => s.Id)
                .FirstOrDefault();
                
            if (lastStaff == null || !lastStaff.Id.StartsWith("NV"))
                return "NV0001";
                
            var lastNumber = int.Parse(lastStaff.Id.Substring(2));
            return $"NV{(lastNumber + 1):D4}";
        }
    }
}