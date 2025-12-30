using System;
using System.Collections.Generic;
using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs.Staff.Requests;
using bookstore_Management.DTOs.Staff.Responses;
using bookstore_Management.Models;
namespace bookstore_Management.Services.Interfaces
{
    public interface IStaffService
    {
        // CRUD
        Result<string> AddStaff(CreateStaffRequestDto dto);
        Result UpdateStaff(string staffId, UpdateStaffRequestDto dto);
        Result DeleteStaff(string staffId);
        Result<StaffResponseDto> GetStaffById(string staffId);
        Result<IEnumerable<StaffResponseDto>> GetAllStaff();
        // Tìm kiếm
        Result<IEnumerable<StaffResponseDto>> GetByRole(UserRole userRole);
        

        // Quản lý trạng thái
        Result ChangeRole(string staffId, UserRole newUserRole);
    }
}