using System;
using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.DTOs;
using bookstore_Management.Models;
using bookstore_Management.Services.Interfaces;

namespace bookstore_Management.Services.Implementations
{
    public class WorkWeekService : IWorkWeekService
    {
        private readonly IWorkWeekRepository _workWeekRepository;

        internal WorkWeekService(IWorkWeekRepository workWeekRepository)
        {
            _workWeekRepository = workWeekRepository;
        }

        public Result<string> Create(WorkWeekCreateDto dto)
        {
            try
            {
                if (dto.EndDate < dto.StartDate)
                    return Result<string>.Fail("Ngày kết thúc phải >= ngày bắt đầu");

                var id = GenerateId();
                var entity = new WorkWeek
                {
                    Id = id,
                    StartDate = dto.StartDate.Date,
                    EndDate = dto.EndDate.Date,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                _workWeekRepository.Add(entity);
                _workWeekRepository.SaveChanges();
                return Result<string>.Success(id, "Tạo tuần làm việc thành công");
            }
            catch (Exception ex)
            {
                return Result<string>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result SetActive(string weekId, bool isActive)
        {
            try
            {
                var entity = _workWeekRepository.GetById(weekId);
                if (entity == null || entity.DeletedDate != null)
                    return Result.Fail("Tuần làm việc không tồn tại");

                entity.IsActive = isActive;
                entity.UpdatedDate = DateTime.Now;
                _workWeekRepository.Update(entity);
                _workWeekRepository.SaveChanges();
                return Result.Success("Cập nhật trạng thái tuần làm việc thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<WorkWeek> GetById(string id)
        {
            try
            {
                var entity = _workWeekRepository.GetById(id);
                if (entity == null || entity.DeletedDate != null)
                    return Result<WorkWeek>.Fail("Tuần làm việc không tồn tại");
                return Result<WorkWeek>.Success(entity);
            }
            catch (Exception ex)
            {
                return Result<WorkWeek>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<WorkWeek> GetActive()
        {
            try
            {
                var entity = _workWeekRepository.GetActive();
                if (entity == null)
                    return Result<WorkWeek>.Fail("Chưa có tuần hoạt động");
                return Result<WorkWeek>.Success(entity);
            }
            catch (Exception ex)
            {
                return Result<WorkWeek>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<WorkWeek>> GetAll()
        {
            try
            {
                var items = _workWeekRepository.GetAll()
                    .Where(w => w.DeletedDate == null)
                    .OrderByDescending(w => w.StartDate)
                    .ToList();
                return Result<IEnumerable<WorkWeek>>.Success(items);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<WorkWeek>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        private string GenerateId()
        {
            var last = _workWeekRepository.GetAll()
                .Where(w => w.DeletedDate == null)
                .OrderByDescending(w => w.Id)
                .FirstOrDefault();
            if (last == null || last.Id.Length < 2 || !last.Id.StartsWith("WW"))
                return "WW0001";
            var num = int.Parse(last.Id.Substring(2));
            return $"WW{(num + 1):D4}";
        }
    }
}

