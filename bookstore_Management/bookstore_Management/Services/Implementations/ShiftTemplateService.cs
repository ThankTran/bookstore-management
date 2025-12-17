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
    public class ShiftTemplateService : IShiftTemplateService
    {
        private readonly IShiftTemplateRepository _shiftTemplateRepository;

        internal ShiftTemplateService(IShiftTemplateRepository shiftTemplateRepository)
        {
            _shiftTemplateRepository = shiftTemplateRepository;
        }

        public Result<string> Create(ShiftTemplateCreateDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result<string>.Fail("Tên ca làm bắt buộc");
                if (dto.StartTime >= dto.EndTime)
                    return Result<string>.Fail("Giờ bắt đầu phải nhỏ hơn giờ kết thúc");

                var id = GenerateId();

                var entity = new ShiftTemplate
                {
                    Id = id,
                    Name = dto.Name.Trim(),
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    WorkingDays = dto.WorkingDays?.Trim(),
                    Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
                    CreatedDate = DateTime.Now
                };

                _shiftTemplateRepository.Add(entity);
                _shiftTemplateRepository.SaveChanges();
                return Result<string>.Success(id, "Tạo ca làm thành công");
            }
            catch (Exception ex)
            {
                return Result<string>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result Update(string id, ShiftTemplateUpdateDto dto)
        {
            try
            {
                var entity = _shiftTemplateRepository.GetById(id);
                if (entity == null || entity.DeletedDate != null)
                    return Result.Fail("Ca làm không tồn tại");

                if (!string.IsNullOrWhiteSpace(dto.Name))
                    entity.Name = dto.Name.Trim();
                if (dto.StartTime.HasValue)
                    entity.StartTime = dto.StartTime.Value;
                if (dto.EndTime.HasValue)
                    entity.EndTime = dto.EndTime.Value;
                if (!string.IsNullOrWhiteSpace(dto.WorkingDays))
                    entity.WorkingDays = dto.WorkingDays.Trim();
                if (!string.IsNullOrWhiteSpace(dto.Description))
                    entity.Description = dto.Description.Trim();

                entity.UpdatedDate = DateTime.Now;
                _shiftTemplateRepository.Update(entity);
                _shiftTemplateRepository.SaveChanges();
                return Result.Success("Cập nhật ca làm thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result Delete(string id)
        {
            try
            {
                var entity = _shiftTemplateRepository.GetById(id);
                if (entity == null || entity.DeletedDate != null)
                    return Result.Fail("Ca làm không tồn tại");

                entity.DeletedDate = DateTime.Now;
                _shiftTemplateRepository.Update(entity);
                _shiftTemplateRepository.SaveChanges();
                return Result.Success("Xóa ca làm thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<ShiftTemplate> GetById(string id)
        {
            try
            {
                var entity = _shiftTemplateRepository.GetById(id);
                if (entity == null || entity.DeletedDate != null)
                    return Result<ShiftTemplate>.Fail("Ca làm không tồn tại");
                return Result<ShiftTemplate>.Success(entity);
            }
            catch (Exception ex)
            {
                return Result<ShiftTemplate>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<ShiftTemplate>> GetAll()
        {
            try
            {
                var items = _shiftTemplateRepository.GetActive().OrderBy(t => t.Name).ToList();
                return Result<IEnumerable<ShiftTemplate>>.Success(items);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<ShiftTemplate>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        private string GenerateId()
        {
            var last = _shiftTemplateRepository.GetAll()
                .Where(s => s.DeletedDate == null)
                .OrderByDescending(s => s.Id)
                .FirstOrDefault();
            if (last == null || last.Id.Length < 2 || !last.Id.StartsWith("CD"))
                return "CD0001";
            var num = int.Parse(last.Id.Substring(2));
            return $"CD{(num + 1):D4}";
        }
    }
}

