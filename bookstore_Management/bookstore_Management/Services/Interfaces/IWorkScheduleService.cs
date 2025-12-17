using System;
using System.Collections.Generic;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs;
using bookstore_Management.Models;

namespace bookstore_Management.Services.Interfaces
{
    public interface IWorkScheduleService
    {
        Result<string> Assign(WorkScheduleCreateDto dto);
        Result Update(string id, WorkScheduleUpdateDto dto);
        Result Delete(string id);
        Result<IEnumerable<WorkSchedule>> GetByWeek(string weekId);
        Result<IEnumerable<WorkSchedule>> GetByStaff(string staffId);
        Result<IEnumerable<WorkSchedule>> GetByDate(DateTime date);
    }
}

