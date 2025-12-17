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
    public class WorkScheduleService : IWorkScheduleService
    {
        private readonly IWorkScheduleRepository _workScheduleRepository;
        private readonly IWorkWeekRepository _workWeekRepository;
        private readonly IShiftTemplateRepository _shiftTemplateRepository;
        private readonly IStaffRepository _staffRepository;

        internal WorkScheduleService(
            IWorkScheduleRepository workScheduleRepository,
            IWorkWeekRepository workWeekRepository,
            IShiftTemplateRepository shiftTemplateRepository,
            IStaffRepository staffRepository)
        {
            _workScheduleRepository = workScheduleRepository;
            _workWeekRepository = workWeekRepository;
            _shiftTemplateRepository = shiftTemplateRepository;
            _staffRepository = staffRepository;
        }

        public Result<string> Assign(WorkScheduleCreateDto dto)
        {
            try
            {
                var week = _workWeekRepository.GetById(dto.WeekId);
                if (week == null)
                    return Result<string>.Fail("Tuần làm việc không tồn tại");

                var staff = _staffRepository.GetById(dto.StaffId);
                if (staff == null || staff.DeletedDate != null)
                    return Result<string>.Fail("Nhân viên không tồn tại");

                var shift = _shiftTemplateRepository.GetById(dto.ShiftTemplateId);
                if (shift == null || shift.DeletedDate != null)
                    return Result<string>.Fail("Ca làm không tồn tại");

                var id = GenerateId();
                var entity = new WorkSchedule
                {
                    Id = id,
                    WeekId = dto.WeekId,
                    StaffId = dto.StaffId,
                    ShiftTemplateId = dto.ShiftTemplateId,
                    WorkDate = dto.WorkDate.Date,
                    Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim(),
                    AssignedBy = dto.AssignedBy,
                    CreatedDate = DateTime.Now
                };

                _workScheduleRepository.Add(entity);
                _workScheduleRepository.SaveChanges();
                return Result<string>.Success(id, "Phân công ca làm thành công");
            }
            catch (Exception ex)
            {
                return Result<string>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result Update(string id, WorkScheduleUpdateDto dto)
        {
            try
            {
                var entity = _workScheduleRepository.GetById(id);
                if (entity == null)
                    return Result.Fail("Lịch làm không tồn tại");

                if (!string.IsNullOrWhiteSpace(dto.ShiftTemplateId))
                {
                    var shift = _shiftTemplateRepository.GetById(dto.ShiftTemplateId);
                    if (shift == null || shift.DeletedDate != null)
                        return Result.Fail("Ca làm không tồn tại");
                    entity.ShiftTemplateId = dto.ShiftTemplateId;
                }

                if (!string.IsNullOrWhiteSpace(dto.Notes))
                    entity.Notes = dto.Notes.Trim();

                entity.UpdatedDate = DateTime.Now;
                _workScheduleRepository.Update(entity);
                _workScheduleRepository.SaveChanges();
                return Result.Success("Cập nhật lịch làm thành công");
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
                var entity = _workScheduleRepository.GetById(id);
                if (entity == null || entity.DeletedDate != null)
                    return Result.Fail("Lịch làm không tồn tại");

                entity.DeletedDate = DateTime.Now;
                entity.UpdatedDate = DateTime.Now;
                _workScheduleRepository.Update(entity);
                _workScheduleRepository.SaveChanges();

                return Result.Success("Xóa lịch làm (soft delete) thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<WorkSchedule>> GetByWeek(string weekId)
        {
            try
            {
                var items = _workScheduleRepository.GetByWeek(weekId).ToList();
                return Result<IEnumerable<WorkSchedule>>.Success(items);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<WorkSchedule>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<WorkSchedule>> GetByStaff(string staffId)
        {
            try
            {
                var items = _workScheduleRepository.GetByStaff(staffId).ToList();
                return Result<IEnumerable<WorkSchedule>>.Success(items);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<WorkSchedule>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<WorkSchedule>> GetByDate(DateTime date)
        {
            try
            {
                var day = date.Date;
                var items = _workScheduleRepository.GetByDate(day).ToList();
                return Result<IEnumerable<WorkSchedule>>.Success(items);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<WorkSchedule>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        private string GenerateId()
        {
            var last = _workScheduleRepository.GetAll()
                .Where(w => w.DeletedDate == null)
                .OrderByDescending(w => w.Id)
                .FirstOrDefault();
            if (last == null || last.Id.Length < 2 || !last.Id.StartsWith("SC"))
                return "SC0001";
            var num = int.Parse(last.Id.Substring(2));
            return $"SC{(num + 1):D4}";
        }
    }
}

