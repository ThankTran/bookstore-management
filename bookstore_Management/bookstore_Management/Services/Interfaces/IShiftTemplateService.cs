using System.Collections.Generic;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs;
using bookstore_Management.Models;

namespace bookstore_Management.Services.Interfaces
{
    public interface IShiftTemplateService
    {
        Result<string> Create(ShiftTemplateCreateDto dto);
        Result Update(string id, ShiftTemplateUpdateDto dto);
        Result Delete(string id);
        Result<ShiftTemplate> GetById(string id);
        Result<IEnumerable<ShiftTemplate>> GetAll();
    }
}

