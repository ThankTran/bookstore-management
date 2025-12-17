using System.Collections.Generic;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs;
using bookstore_Management.Models;

namespace bookstore_Management.Services.Interfaces
{
    public interface IWorkWeekService
    {
        Result<string> Create(WorkWeekCreateDto dto);
        Result SetActive(string weekId, bool isActive);
        Result<WorkWeek> GetById(string id);
        Result<WorkWeek> GetActive();
        Result<IEnumerable<WorkWeek>> GetAll();
    }
}

