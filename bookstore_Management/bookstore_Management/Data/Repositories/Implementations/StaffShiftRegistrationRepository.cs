using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Implementations
{
    internal class StaffShiftRegistrationRepository : Repository<StaffShiftRegistration, string>, IStaffShiftRegistrationRepository
    {
        public StaffShiftRegistrationRepository(BookstoreDbContext context) : base(context) { }

        public IEnumerable<StaffShiftRegistration> GetByWeek(string weekId)
        {
            return Find(r => r.WeekId == weekId && r.DeletedDate == null);
        }

        public IEnumerable<StaffShiftRegistration> GetByStaff(string staffId)
        {
            return Find(r => r.StaffId == staffId && r.DeletedDate == null);
        }

        public StaffShiftRegistration GetByWeekAndStaffAndTemplate(string weekId, string staffId, string templateId)
        {
            return Find(r =>
                    r.WeekId == weekId &&
                    r.StaffId == staffId &&
                    r.ShiftTemplateId == templateId &&
                    r.DeletedDate == null)
                .FirstOrDefault();
        }
    }
}

