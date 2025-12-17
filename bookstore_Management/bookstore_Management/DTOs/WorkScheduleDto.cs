using System;
using System.Collections.Generic;

namespace bookstore_Management.DTOs
{
    public class WorkWeekCreateDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class WorkWeekUpdateDto
    {
        public bool IsActive { get; set; }
    }

    public class WorkWeekResponseDto
    {
        public string Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TotalRegistrations { get; set; }
        public int TotalSchedules { get; set; }
    }

    public class ShiftTemplateCreateDto
    {
        public string Name { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string WorkingDays { get; set; }
        public string Description { get; set; }
    }

    public class ShiftTemplateUpdateDto
    {
        public string Name { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string WorkingDays { get; set; }
        public string Description { get; set; }
    }

    public class ShiftTemplateResponseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string WorkingDays { get; set; }
        public List<int> WorkingDaysList { get; set; }
        public string Description { get; set; }
        public decimal TotalHours { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class StaffShiftRegistrationCreateDto
    {
        public string WeekId { get; set; }
        public string StaffId { get; set; }
        public string ShiftTemplateId { get; set; }
        public string Notes { get; set; }
    }

    public class StaffShiftRegistrationResponseDto
    {
        public string Id { get; set; }
        public string WeekId { get; set; }
        public string StaffId { get; set; }
        public string StaffName { get; set; }
        public string ShiftTemplateId { get; set; }
        public string ShiftName { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string WorkingDays { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class WorkScheduleCreateDto
    {
        public string WeekId { get; set; }
        public string StaffId { get; set; }
        public string ShiftTemplateId { get; set; }
        public DateTime WorkDate { get; set; }
        public string Notes { get; set; }
        public string AssignedBy { get; set; }
    }

    public class WorkScheduleUpdateDto
    {
        public string ShiftTemplateId { get; set; }
        public string Notes { get; set; }
    }

    public class WorkScheduleResponseDto
    {
        public string Id { get; set; }
        public string WeekId { get; set; }
        public string StaffId { get; set; }
        public string StaffName { get; set; }
        public string ShiftTemplateId { get; set; }
        public string ShiftName { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DateTime WorkDate { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public string Notes { get; set; }
        public string AssignedBy { get; set; }
        public string AssignedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal Hours { get; set; }
    }
}