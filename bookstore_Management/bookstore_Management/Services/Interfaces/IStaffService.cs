using System;
using System.Collections.Generic;
using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs;
using bookstore_Management.Models;
namespace bookstore_Management.Services.Interfaces
{
    public interface IStaffService
    {
        // CRUD
        Result<string> AddStaff(StaffCreateDto dto);
        Result UpdateStaff(string staffId, StaffUpdateDto dto);
        Result DeleteStaff(string staffId);
        Result<Staff> GetStaffById(string staffId);
        Result<IEnumerable<Staff>> GetAllStaff();
        // Tìm kiếm
        Result<IEnumerable<Staff>> GetByRole(UserRole userRole);
        

        // Quản lý trạng thái
        Result ChangeRole(string staffId, UserRole newUserRole);
    }
}