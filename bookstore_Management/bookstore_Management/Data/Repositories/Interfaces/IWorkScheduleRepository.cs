using System;
using System.Collections.Generic;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    internal interface IWorkScheduleRepository : IRepository<WorkSchedule, string>
    {
        IEnumerable<WorkSchedule> GetByWeek(string weekId);
        IEnumerable<WorkSchedule> GetByStaff(string staffId);
        IEnumerable<WorkSchedule> GetByDate(DateTime date);
    }
}

