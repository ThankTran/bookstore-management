using System.Collections.Generic;
using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs;
using bookstore_Management.Models;

namespace bookstore_Management.Services.Interfaces
{
    public interface IStaffService
    {
        // CRUD cho Staff
        Result<string> AddStaff(StaffDto dto);
        Result UpdateStaff(string staffId, StaffDto dto);
        Result DeleteStaff(string staffId);
        Result<Staff> GetStaffById(string staffId);
        Result<IEnumerable<Staff>> GetAllStaff();
        
        // Tìm kiếm
        Result<Staff> GetStaffByPhone(string phone);
        Result<Staff> GetStaffByCitizenId(string citizenId);
        Result<IEnumerable<Staff>> SearchByName(string name);
        Result<IEnumerable<Staff>> GetByRole(Role role);
        Result<IEnumerable<Staff>> GetByStatus(StaffStatus status);
        
        // Quản lý lương
        Result<decimal> CalculateSalary(string staffId, int workingDays);
        Result UpdateSalary(string staffId, decimal newBaseSalary);
        Result UpdateSalaryRate(string staffId, decimal newRate);
        
        // Quản lý trạng thái
        Result ChangeStatus(string staffId, StaffStatus newStatus);
        Result ChangeRole(string staffId, Role newRole);
    }
}