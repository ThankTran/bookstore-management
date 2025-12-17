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
    public class StaffShiftRegistrationService : IStaffShiftRegistrationService
    {
        private readonly IStaffShiftRegistrationRepository _registrationRepository;
        private readonly IWorkWeekRepository _workWeekRepository;
        private readonly IShiftTemplateRepository _shiftTemplateRepository;
        private readonly IStaffRepository _staffRepository;

        internal StaffShiftRegistrationService(
            IStaffShiftRegistrationRepository registrationRepository,
            IWorkWeekRepository workWeekRepository,
            IShiftTemplateRepository shiftTemplateRepository,
            IStaffRepository staffRepository)
        {
            _registrationRepository = registrationRepository;
            _workWeekRepository = workWeekRepository;
            _shiftTemplateRepository = shiftTemplateRepository;
            _staffRepository = staffRepository;
        }

        public Result<string> Register(StaffShiftRegistrationCreateDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.WeekId) ||
                    string.IsNullOrWhiteSpace(dto.StaffId) ||
                    string.IsNullOrWhiteSpace(dto.ShiftTemplateId))
                    return Result<string>.Fail("Thiếu thông tin bắt buộc");

                var week = _workWeekRepository.GetById(dto.WeekId);
                if (week == null)
                    return Result<string>.Fail("Tuần làm việc không tồn tại");

                var staff = _staffRepository.GetById(dto.StaffId);
                if (staff == null || staff.DeletedDate != null)
                    return Result<string>.Fail("Nhân viên không tồn tại");

                var template = _shiftTemplateRepository.GetById(dto.ShiftTemplateId);
                if (template == null || template.DeletedDate != null)
                    return Result<string>.Fail("Ca làm không tồn tại");

                var existing = _registrationRepository.GetByWeekAndStaffAndTemplate(dto.WeekId, dto.StaffId, dto.ShiftTemplateId);
                if (existing != null)
                    return Result<string>.Fail("Đã đăng ký ca này trong tuần");

                var id = GenerateId();
                var entity = new StaffShiftRegistration
                {
                    Id = id,
                    WeekId = dto.WeekId,
                    StaffId = dto.StaffId,
                    ShiftTemplateId = dto.ShiftTemplateId,
                    Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim(),
                    CreatedDate = DateTime.Now
                };

                _registrationRepository.Add(entity);
                _registrationRepository.SaveChanges();
                return Result<string>.Success(id, "Đăng ký ca thành công");
            }
            catch (Exception ex)
            {
                return Result<string>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<StaffShiftRegistration>> GetByWeek(string weekId)
        {
            try
            {
                var items = _registrationRepository.GetByWeek(weekId).ToList();
                return Result<IEnumerable<StaffShiftRegistration>>.Success(items);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<StaffShiftRegistration>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<StaffShiftRegistration>> GetByStaff(string staffId)
        {
            try
            {
                var items = _registrationRepository.GetByStaff(staffId).ToList();
                return Result<IEnumerable<StaffShiftRegistration>>.Success(items);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<StaffShiftRegistration>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<StaffShiftRegistration>> GetByWeekAndStaff(string weekId, string staffId)
        {
            try
            {
                var items = _registrationRepository.GetByWeek(weekId)
                    .Where(r => r.StaffId == staffId)
                    .ToList();
                return Result<IEnumerable<StaffShiftRegistration>>.Success(items);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<StaffShiftRegistration>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        private string GenerateId()
        {
            var last = _registrationRepository.GetAll()
                .Where(r => r.DeletedDate == null)
                .OrderByDescending(r => r.Id)
                .FirstOrDefault();
            if (last == null || last.Id.Length < 2 || !last.Id.StartsWith("SR"))
                return "SR0001";
            var num = int.Parse(last.Id.Substring(2));
            return $"SR{(num + 1):D4}";
        }
    }
}

