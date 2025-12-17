using System.Collections.Generic;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    internal interface IStaffShiftRegistrationRepository : IRepository<StaffShiftRegistration, string>
    {
        IEnumerable<StaffShiftRegistration> GetByWeek(string weekId);
        IEnumerable<StaffShiftRegistration> GetByStaff(string staffId);
        StaffShiftRegistration GetByWeekAndStaffAndTemplate(string weekId, string staffId, string templateId);
    }
}

