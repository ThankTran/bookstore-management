using System;
using System.Collections.Generic;
using bookstore_Management.Core.Results;
using bookstore_Management.Models;
namespace bookstore_Management.Services.Interfaces
{
    public interface IStaffDailyRevenueService
    {
        // CRUD
        Result RecordDailyRevenue(string staffId, DateTime day, decimal revenue);
        Result<StaffDailyRevenue> GetDailyRevenue(string staffId, DateTime day);
        Result<IEnumerable<StaffDailyRevenue>> GetAllDailyRevenues();
        // Tìm kiếm
        Result<IEnumerable<StaffDailyRevenue>> GetRevenueByStaff(string staffId, DateTime fromDate, DateTime toDate);
        Result<IEnumerable<StaffDailyRevenue>> GetRevenueByDate(DateTime date);

        // Báo cáo
        Result<decimal> CalculateTotalRevenue(string staffId, DateTime fromDate, DateTime toDate);
        Result<decimal> CalculateDailyAverageRevenue(string staffId, DateTime fromDate, DateTime toDate);
        Result<IEnumerable<StaffDailyRevenue>> GetTopPerformers(DateTime fromDate, DateTime toDate, int topCount = 5);
        Result<decimal> GetHighestRevenueDay(string staffId, DateTime fromDate, DateTime toDate);
    }
}