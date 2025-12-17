using System;
using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Implementations
{
    internal class WorkScheduleRepository : Repository<WorkSchedule, string>, IWorkScheduleRepository
    {
        public WorkScheduleRepository(BookstoreDbContext context) : base(context) { }

        public IEnumerable<WorkSchedule> GetByWeek(string weekId)
        {
            return Find(ws => ws.WeekId == weekId && ws.DeletedDate == null);
        }

        public IEnumerable<WorkSchedule> GetByStaff(string staffId)
        {
            return Find(ws => ws.StaffId == staffId && ws.DeletedDate == null);
        }

        public IEnumerable<WorkSchedule> GetByDate(DateTime date)
        {
            var day = date.Date;
            return Find(ws => ws.WorkDate == day && ws.DeletedDate == null);
        }
    }
}

