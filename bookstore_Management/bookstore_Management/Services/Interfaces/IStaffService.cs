using System.Collections.Generic;
using System.Threading.Tasks;
using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs.Staff.Requests;
using bookstore_Management.DTOs.Staff.Responses;

namespace bookstore_Management.Services.Interfaces
{
    public interface IStaffService
    {
        Task<Result<string>> AddStaffAsync(CreateStaffRequestDto dto);
        Task<Result> UpdateStaffAsync(string staffId, UpdateStaffRequestDto dto);
        Task<Result> DeleteStaffAsync(string staffId);
        Task<Result<StaffResponseDto>> GetStaffByIdAsync(string staffId);
        Task<Result<IEnumerable<StaffResponseDto>>> GetAllStaffAsync();
        Task<Result<IEnumerable<StaffResponseDto>>> GetByRoleAsync(UserRole userRole);
        Task<Result<IEnumerable<StaffResponseDto>>> SearchByNameAsync(string name);
        Task<Result> ChangeRoleAsync(string staffId, UserRole newUserRole);
    }
}