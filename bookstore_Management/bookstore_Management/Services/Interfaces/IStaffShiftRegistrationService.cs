using System.Collections.Generic;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs;
using bookstore_Management.Models;

namespace bookstore_Management.Services.Interfaces
{
    public interface IStaffShiftRegistrationService
    {
        Result<string> Register(StaffShiftRegistrationCreateDto dto);
        Result<IEnumerable<StaffShiftRegistration>> GetByWeek(string weekId);
        Result<IEnumerable<StaffShiftRegistration>> GetByStaff(string staffId);
        Result<IEnumerable<StaffShiftRegistration>> GetByWeekAndStaff(string weekId, string staffId);
    }
}

