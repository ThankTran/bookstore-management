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
    /// <summary>
    /// Service quản lý hóa đơn nhập từ nhà cung cấp
    /// </summary>
    public class ImportBillService : IImportBillService
    {
        private readonly IImportBillRepository _importBillRepository;
        private readonly IImportBillDetailRepository _importBillDetailRepository;
        private readonly IBookRepository _bookRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IStockRepository _stockRepository;

        public ImportBillService(
            IImportBillRepository importBillRepository,
            IImportBillDetailRepository importBillDetailRepository,
            IBookRepository bookRepository,
            ISupplierRepository supplierRepository,
            IStockRepository stockRepository)
        {
            _importBillRepository = importBillRepository;
            _importBillDetailRepository = importBillDetailRepository;
            _bookRepository = bookRepository;
            _supplierRepository = supplierRepository;
            _stockRepository = stockRepository;
        }

        // ==================================================================
        // ---------------------- THÊM DỮ LIỆU ------------------------------
        // ==================================================================

        /// <summary>
        /// Tạo hóa đơn nhập mới
        /// </summary>
        public Result<int> CreateImportBill(ImportBillDto dto)
        {
            try
            {
                // Validate
                if (string.IsNullOrWhiteSpace(dto.ImportBillCode))
                    return Result<int>.Fail("Mã hóa đơn không được để trống");

                if (string.IsNullOrWhiteSpace(dto.SupplierId))
                    return Result<int>.Fail("Mã nhà cung cấp không được để trống");

                var supplier = _supplierRepository.GetById(dto.SupplierId);
                if (supplier == null)
                    return Result<int>.Fail("Nhà cung cấp không tồn tại");

                if (dto.Items == null || !dto.Items.Any())
                    return Result<int>.Fail("Hóa đơn phải có ít nhất 1 sách");

                decimal totalAmount = 0;
                var importDetails = new List<ImportBillDetail>();

                // Validate & tính tổng
                foreach (var item in dto.Items)
                {
                    var book = _bookRepository.GetById(item.BookId);
                    if (book == null)
                        return Result<int>.Fail($"Sách {item.BookId} không tồn tại");

                    if (item.Quantity <= 0)
                        return Result<int>.Fail($"Số lượng sách {book.Name} phải > 0");

                    if (item.ImportPrice <= 0)
                        return Result<int>.Fail($"Giá nhập sách {book.Name} phải > 0");

                    var itemTotal = item.ImportPrice * item.Quantity;
                    totalAmount += itemTotal;

                    importDetails.Add(new ImportBillDetail
                    {
                        BookId = item.BookId,
                        Quantity = item.Quantity,
                        ImportPrice = item.ImportPrice,
                        TotalPrice = itemTotal
                    });
                }

                // Tạo hóa đơn
                var importBill = new ImportBill
                {
                    ImportBillCode = dto.ImportBillCode.Trim(),
                    ImportDate = dto.ImportDate,
                    SupplierId = dto.SupplierId,
                    TotalAmount = totalAmount,
                    Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim(),
                    CreatedBy = "SYSTEM", // Sẽ được set từ Controller
                    CreatedDate = DateTime.Now
                };

                _importBillRepository.Add(importBill);
                _importBillRepository.SaveChanges();

                // Thêm chi tiết hóa đơn
                foreach (var detail in importDetails)
                {
                    detail.ImportId = importBill.ImportBillId;
                    _importBillDetailRepository.Add(detail);
                }
                _importBillDetailRepository.SaveChanges();

                // Cập nhật tồn kho
                foreach (var item in dto.Items)
                {
                    var stock = _stockRepository.GetByBookId(item.BookId);
                    if (stock != null)
                    {
                        stock.StockQuantity += item.Quantity;
                        _stockRepository.Update(stock);
                    }
                    else
                    {
                        // Tạo stock mới nếu chưa có
                        _stockRepository.Add(new Stock
                        {
                            BookId = item.BookId,
                            StockQuantity = item.Quantity
                        });
                    }
                }
                _stockRepository.SaveChanges();

                return Result<int>.Success(importBill.ImportBillId, "Tạo hóa đơn nhập thành công");
            }
            catch (Exception ex)
            {
                return Result<int>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- SỬA DỮ LIỆU ------------------------------
        // ==================================================================

        /// <summary>
        /// Cập nhật hóa đơn nhập
        /// </summary>
        public Result UpdateImportBill(int importBillId, ImportBillDto dto)
        {
            try
            {
                var importBill = _importBillRepository.GetById(importBillId);
                if (importBill == null)
                    return Result.Fail("Hóa đơn nhập không tồn tại");

                var supplier = _supplierRepository.GetById(dto.SupplierId);
                if (supplier == null)
                    return Result.Fail("Nhà cung cấp không tồn tại");

                importBill.ImportBillCode = dto.ImportBillCode.Trim();
                importBill.ImportDate = dto.ImportDate;
                importBill.SupplierId = dto.SupplierId;
                importBill.Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim();
                importBill.UpdatedDate = DateTime.Now;

                _importBillRepository.Update(importBill);
                _importBillRepository.SaveChanges();

                return Result.Success("Cập nhật hóa đơn nhập thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ---------------------- XÓA DỮ LIỆU -------------------------------
        // ==================================================================

        /// <summary>
        /// Xóa hóa đơn nhập
        /// </summary>
        public Result DeleteImportBill(int importBillId)
        {
            try
            {
                var importBill = _importBillRepository.GetById(importBillId);
                if (importBill == null)
                    return Result.Fail("Hóa đơn nhập không tồn tại");

                // Soft delete
                importBill.DeletedDate = DateTime.Now;
                _importBillRepository.Update(importBill);
                _importBillRepository.SaveChanges();

                return Result.Success("Xóa hóa đơn nhập thành công");
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
        /// Lấy hóa đơn nhập theo ID
        /// </summary>
        public Result<ImportBill> GetImportBillById(int importBillId)
        {
            try
            {
                var importBill = _importBillRepository.GetById(importBillId);
                if (importBill == null)
                    return Result<ImportBill>.Fail("Hóa đơn nhập không tồn tại");

                return Result<ImportBill>.Success(importBill);
            }
            catch (Exception ex)
            {
                return Result<ImportBill>.Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy tất cả hóa đơn nhập
        /// </summary>
        public Result<IEnumerable<ImportBill>> GetAllImportBills()
        {
            try
            {
                var importBills = _importBillRepository.GetAll()
                    .Where(ib => ib.DeletedDate == null)
                    .OrderByDescending(ib => ib.CreatedDate)
                    .ToList();

                return Result<IEnumerable<ImportBill>>.Success(importBills);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<ImportBill>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy hóa đơn nhập theo nhà cung cấp
        /// </summary>
        public Result<IEnumerable<ImportBill>> GetBySupplier(string supplierId)
        {
            try
            {
                var importBills = _importBillRepository.Find(ib => 
                    ib.SupplierId == supplierId && ib.DeletedDate == null)
                    .OrderByDescending(ib => ib.CreatedDate)
                    .ToList();

                return Result<IEnumerable<ImportBill>>.Success(importBills);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<ImportBill>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy hóa đơn nhập theo khoảng ngày
        /// </summary>
        public Result<IEnumerable<ImportBill>> GetByDateRange(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var importBills = _importBillRepository.Find(ib =>
                    ib.ImportDate >= fromDate && ib.ImportDate <= toDate && ib.DeletedDate == null)
                    .OrderByDescending(ib => ib.ImportDate)
                    .ToList();

                return Result<IEnumerable<ImportBill>>.Success(importBills);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<ImportBill>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- QUẢN LÝ CHI TIẾT -------------------------
        // ==================================================================

        /// <summary>
        /// Thêm sách vào hóa đơn nhập
        /// </summary>
        public Result AddImportItem(int importBillId, ImportBillDetailDto item)
        {
            try
            {
                var importBill = _importBillRepository.GetById(importBillId);
                if (importBill == null)
                    return Result.Fail("Hóa đơn nhập không tồn tại");

                var book = _bookRepository.GetById(item.BookId);
                if (book == null)
                    return Result.Fail("Sách không tồn tại");

                var existingDetail = _importBillDetailRepository
                    .Find(ibd => ibd.ImportId == importBillId && ibd.BookId == item.BookId)
                    .FirstOrDefault();

                if (existingDetail != null)
                    return Result.Fail("Sách này đã có trong hóa đơn");

                var detail = new ImportBillDetail
                {
                    ImportId = importBillId,
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    ImportPrice = item.ImportPrice,
                    TotalPrice = item.ImportPrice * item.Quantity
                };

                _importBillDetailRepository.Add(detail);
                _importBillDetailRepository.SaveChanges();

                // Cập nhật tổng tiền hóa đơn
                RecalculateBillTotal(importBillId);

                return Result.Success("Thêm sách vào hóa đơn thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa sách khỏi hóa đơn nhập
        /// </summary>
        public Result RemoveImportItem(int importBillId, string bookId)
        {
            try
            {
                var detail = _importBillDetailRepository
                    .Find(ibd => ibd.ImportId == importBillId && ibd.BookId == bookId)
                    .FirstOrDefault();

                if (detail == null)
                    return Result.Fail("Sách không tồn tại trong hóa đơn");

                _importBillDetailRepository.Delete(detail.BookId); // Assuming Delete by key
                _importBillDetailRepository.SaveChanges();

                RecalculateBillTotal(importBillId);

                return Result.Success("Xóa sách khỏi hóa đơn thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật số lượng sách trong hóa đơn
        /// </summary>
        public Result UpdateImportItem(int importBillId, string bookId, int newQuantity)
        {
            try
            {
                if (newQuantity <= 0)
                    return Result.Fail("Số lượng phải > 0");

                var detail = _importBillDetailRepository
                    .Find(ibd => ibd.ImportId == importBillId && ibd.BookId == bookId)
                    .FirstOrDefault();

                if (detail == null)
                    return Result.Fail("Sách không tồn tại trong hóa đơn");

                detail.Quantity = newQuantity;
                detail.TotalPrice = detail.ImportPrice * newQuantity;

                _importBillDetailRepository.Update(detail);
                _importBillDetailRepository.SaveChanges();

                RecalculateBillTotal(importBillId);

                return Result.Success("Cập nhật số lượng thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy chi tiết hóa đơn nhập
        /// </summary>
        public Result<IEnumerable<ImportBillDetail>> GetImportDetails(int importBillId)
        {
            try
            {
                var details = _importBillDetailRepository.Find(ibd => ibd.ImportId == importBillId);
                return Result<IEnumerable<ImportBillDetail>>.Success(details);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<ImportBillDetail>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- BÁO CÁO & THỐNG KÊ ----------------------
        // ==================================================================

        /// <summary>
        /// Tính tổng giá trị nhập theo nhà cung cấp và khoảng ngày
        /// </summary>
        public Result<decimal> CalculateTotalImportBySupplier(string supplierId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var importBills = _importBillRepository.Find(ib =>
                    ib.SupplierId == supplierId &&
                    ib.ImportDate >= fromDate &&
                    ib.ImportDate <= toDate &&
                    ib.DeletedDate == null);

                decimal totalValue = importBills.Sum(ib => ib.TotalAmount);
                return Result<decimal>.Success(totalValue);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Tính tổng giá trị nhập theo khoảng ngày
        /// </summary>
        public Result<decimal> CalculateTotalImportByDateRange(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var importBills = _importBillRepository.Find(ib =>
                    ib.ImportDate >= fromDate &&
                    ib.ImportDate <= toDate &&
                    ib.DeletedDate == null);

                decimal totalValue = importBills.Sum(ib => ib.TotalAmount);
                return Result<decimal>.Success(totalValue);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- HÀM HELPER -------------------------------
        // ==================================================================

        /// <summary>
        /// Tính lại tổng tiền hóa đơn
        /// </summary>
        private void RecalculateBillTotal(int importBillId)
        {
            try
            {
                var importBill = _importBillRepository.GetById(importBillId);
                if (importBill == null) return;

                var details = _importBillDetailRepository.Find(ibd => ibd.ImportId == importBillId);
                importBill.TotalAmount = details.Sum(d => d.TotalPrice);

                _importBillRepository.Update(importBill);
                _importBillRepository.SaveChanges();
            }
            catch { }
        }
    }
}