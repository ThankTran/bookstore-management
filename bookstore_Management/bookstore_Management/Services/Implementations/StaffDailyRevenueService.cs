using System;
using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;
using bookstore_Management.Services.Interfaces;

namespace bookstore_Management.Services.Implementations
{
    /// <summary>
    /// Service quản lý doanh thu hàng ngày của nhân viên
    /// </summary>
    public class StaffDailyRevenueService : IStaffDailyRevenueService
    {
        private readonly IStaffDailyRevenueRepository _revenueRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IOrderRepository _orderRepository;

        public StaffDailyRevenueService(
            IStaffDailyRevenueRepository revenueRepository,
            IStaffRepository staffRepository,
            IOrderRepository orderRepository)
        {
            _revenueRepository = revenueRepository;
            _staffRepository = staffRepository;
            _orderRepository = orderRepository;
        }

        // ==================================================================
        // ---------------------- THÊM DỮ LIỆU ------------------------------
        // ==================================================================

        /// <summary>
        /// Ghi nhận doanh thu của nhân viên trong 1 ngày
        /// </summary>
        public Result RecordDailyRevenue(string staffId, DateTime day, decimal revenue)
        {
            try
            {
                if (revenue < 0)
                    return Result.Fail("Doanh thu không được âm");

                var staff = _staffRepository.GetById(staffId);
                if (staff == null)
                    return Result.Fail("Nhân viên không tồn tại");

                // Kiểm tra xem ngày này có record rồi không
                var existingRecord = _revenueRepository
                    .Find(r => r.EmployeeId == staffId && r.Day.Date == day.Date)
                    .FirstOrDefault();

                if (existingRecord != null)
                {
                    // Update record cũ
                    existingRecord.Revenue = revenue;
                    _revenueRepository.Update(existingRecord);
                }
                else
                {
                    // Tạo record mới
                    var dailyRevenue = new StaffDailyRevenue
                    {
                        EmployeeId = staffId,
                        Day = day.Date,
                        Revenue = revenue
                    };
                    _revenueRepository.Add(dailyRevenue);
                }

                _revenueRepository.SaveChanges();
                return Result.Success("Ghi nhận doanh thu thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- LẤY DỮ LIỆU ------------------------------
        // ==================================================================

        /// <summary>
        /// Lấy doanh thu của nhân viên trong 1 ngày cụ thể
        /// </summary>
        public Result<StaffDailyRevenue> GetDailyRevenue(string staffId, DateTime day)
        {
            try
            {
                var revenue = _revenueRepository
                    .Find(r => r.EmployeeId == staffId && r.Day.Date == day.Date)
                    .FirstOrDefault();

                if (revenue == null)
                    return Result<StaffDailyRevenue>.Fail("Không có dữ liệu doanh thu cho ngày này");

                return Result<StaffDailyRevenue>.Success(revenue);
            }
            catch (Exception ex)
            {
                return Result<StaffDailyRevenue>.Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy tất cả doanh thu hàng ngày
        /// </summary>
        public Result<IEnumerable<StaffDailyRevenue>> GetAllDailyRevenues()
        {
            try
            {
                var revenues = _revenueRepository.GetAll()
                    .OrderByDescending(r => r.Day)
                    .ToList();

                return Result<IEnumerable<StaffDailyRevenue>>.Success(revenues);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<StaffDailyRevenue>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy doanh thu của nhân viên trong khoảng ngày
        /// </summary>
        public Result<IEnumerable<StaffDailyRevenue>> GetRevenueByStaff(string staffId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var staff = _staffRepository.GetById(staffId);
                if (staff == null)
                    return Result<IEnumerable<StaffDailyRevenue>>.Fail("Nhân viên không tồn tại");

                var revenues = _revenueRepository.Find(r =>
                    r.EmployeeId == staffId &&
                    r.Day >= fromDate.Date &&
                    r.Day <= toDate.Date)
                    .OrderByDescending(r => r.Day)
                    .ToList();

                return Result<IEnumerable<StaffDailyRevenue>>.Success(revenues);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<StaffDailyRevenue>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy doanh thu của tất cả nhân viên trong 1 ngày
        /// </summary>
        public Result<IEnumerable<StaffDailyRevenue>> GetRevenueByDate(DateTime date)
        {
            try
            {
                var revenues = _revenueRepository.Find(r => r.Day.Date == date.Date)
                    .OrderByDescending(r => r.Revenue)
                    .ToList();

                return Result<IEnumerable<StaffDailyRevenue>>.Success(revenues);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<StaffDailyRevenue>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- BÁO CÁO & THỐNG KÊ ----------------------
        // ==================================================================

        /// <summary>
        /// Tính tổng doanh thu của nhân viên trong khoảng ngày
        /// </summary>
        public Result<decimal> CalculateTotalRevenue(string staffId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var staff = _staffRepository.GetById(staffId);
                if (staff == null)
                    return Result<decimal>.Fail("Nhân viên không tồn tại");

                var revenues = _revenueRepository.Find(r =>
                    r.EmployeeId == staffId &&
                    r.Day >= fromDate.Date &&
                    r.Day <= toDate.Date);

                decimal totalRevenue = revenues.Sum(r => r.Revenue);
                return Result<decimal>.Success(totalRevenue);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Tính doanh thu trung bình hàng ngày của nhân viên
        /// </summary>
        public Result<decimal> CalculateDailyAverageRevenue(string staffId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var staff = _staffRepository.GetById(staffId);
                if (staff == null)
                    return Result<decimal>.Fail("Nhân viên không tồn tại");

                var revenues = _revenueRepository.Find(r =>
                    r.EmployeeId == staffId &&
                    r.Day >= fromDate.Date &&
                    r.Day <= toDate.Date);

                if (!revenues.Any())
                    return Result<decimal>.Success(0, "Không có dữ liệu doanh thu");

                decimal averageRevenue = revenues.Average(r => r.Revenue);
                return Result<decimal>.Success(averageRevenue);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy những nhân viên có doanh thu cao nhất trong khoảng ngày
        /// </summary>
        public Result<IEnumerable<StaffDailyRevenue>> GetTopPerformers(DateTime fromDate, DateTime toDate, int topCount = 5)
        {
            try
            {
                var topPerformers = _revenueRepository.Find(r =>
                    r.Day >= fromDate.Date &&
                    r.Day <= toDate.Date)
                    .OrderByDescending(r => r.Revenue)
                    .Take(topCount)
                    .ToList();

                return Result<IEnumerable<StaffDailyRevenue>>.Success(topPerformers);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<StaffDailyRevenue>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy doanh thu cao nhất của nhân viên trong khoảng ngày
        /// </summary>
        public Result<decimal> GetHighestRevenueDay(string staffId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var staff = _staffRepository.GetById(staffId);
                if (staff == null)
                    return Result<decimal>.Fail("Nhân viên không tồn tại");

                var revenues = _revenueRepository.Find(r =>
                    r.EmployeeId == staffId &&
                    r.Day >= fromDate.Date &&
                    r.Day <= toDate.Date);

                if (!revenues.Any())
                    return Result<decimal>.Success(0, "Không có dữ liệu doanh thu");

                decimal highestRevenue = revenues.Max(r => r.Revenue);
                return Result<decimal>.Success(highestRevenue);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- HÀM AUTO CALCULATE ----------------------
        // ==================================================================

        /// <summary>
        /// Tự động tính doanh thu từ Orders (gọi vào khi tạo/cập nhật Order)
        /// </summary>
        public Result CalculateRevenueFromOrders(string staffId, DateTime day)
        {
            try
            {
                var staff = _staffRepository.GetById(staffId);
                if (staff == null)
                    return Result.Fail("Nhân viên không tồn tại");

                var orders = _orderRepository.Find(o =>
                    o.StaffId == staffId &&
                    o.CreatedDate.Date == day.Date &&
                    o.DeletedDate == null);

                decimal totalRevenue = orders.Sum(o => o.TotalPrice);

                return RecordDailyRevenue(staffId, day, totalRevenue);
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }
    }
}