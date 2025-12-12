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

        internal ImportBillService(
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
        public Result<string> CreateImportBill(ImportBillCreateDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.SupplierId))
                    return Result<string>.Fail("Nhà cung cấp bắt buộc");
                if (string.IsNullOrWhiteSpace(dto.WarehouseId))
                    return Result<string>.Fail("Kho nhập bắt buộc");

                var supplier = _supplierRepository.GetById(dto.SupplierId);
                if (supplier == null || supplier.DeletedDate != null)
                    return Result<string>.Fail("Nhà cung cấp không tồn tại");

                if (dto.ImportBillDetails == null || !dto.ImportBillDetails.Any())
                    return Result<string>.Fail("Hóa đơn phải có ít nhất 1 sách");

                decimal totalAmount = 0;
                var details = new List<ImportBillDetail>();

                foreach (var item in dto.ImportBillDetails)
                {
                    var book = _bookRepository.GetById(item.BookId);
                    if (book == null || book.DeletedDate != null)
                        return Result<string>.Fail($"Sách {item.BookId} không tồn tại");

                    if (item.Quantity <= 0)
                        return Result<string>.Fail($"Số lượng sách {book.Name} phải > 0");
                    if (item.ImportPrice <= 0)
                        return Result<string>.Fail($"Giá nhập sách {book.Name} phải > 0");

                    var lineTotal = item.ImportPrice * item.Quantity;
                    totalAmount += lineTotal;

                    // Đồng bộ giá nhập hiện tại cho Book (phục vụ thống kê/lợi nhuận)
                    book.ImportPrice = item.ImportPrice;
                    book.UpdatedDate = DateTime.Now;
                    _bookRepository.Update(book);

                    details.Add(new ImportBillDetail
                    {
                        ImportId = "", // set sau
                        BookId = item.BookId,
                        Quantity = item.Quantity,
                        ImportPrice = item.ImportPrice
                    });
                }

                var importId = GenerateImportId();
                foreach (var d in details)
                {
                    d.ImportId = importId;
                }

                var importBill = new ImportBill
                {
                    Id = importId,
                    SupplierId = dto.SupplierId,
                    WarehouseId = dto.WarehouseId,
                    TotalAmount = totalAmount,
                    Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim(),
                    CreatedBy = string.IsNullOrWhiteSpace(dto.CreatedBy) ? "SYSTEM" : dto.CreatedBy,
                    CreatedDate = DateTime.Now,
                    ImportBillDetails = details
                };

                _importBillRepository.Add(importBill);
                _importBillRepository.SaveChanges();
                _bookRepository.SaveChanges();

                // Cập nhật tồn kho theo kho nhập
                foreach (var item in details)
                {
                    var stock = _stockRepository.Get(dto.WarehouseId, item.BookId);
                    if (stock != null)
                    {
                        stock.StockQuantity += item.Quantity;
                        stock.UpdatedDate = DateTime.Now;
                        _stockRepository.Update(stock);
                    }
                    else
                    {
                        _stockRepository.Add(new Stock
                        {
                            WarehouseId = dto.WarehouseId,
                            BookId = item.BookId,
                            StockQuantity = item.Quantity,
                            UpdatedDate = DateTime.Now
                        });
                    }
                }
                _stockRepository.SaveChanges();

                return Result<string>.Success(importId, "Tạo hóa đơn nhập thành công");
            }
            catch (Exception ex)
            {
                return Result<string>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- SỬA / XÓA --------------------------------
        // ==================================================================
        public Result UpdateImportBill(string importBillId, ImportBillUpdateDto dto)
        {
            try
            {
                var bill = _importBillRepository.GetById(importBillId);
                if (bill == null || bill.DeletedDate != null)
                    return Result.Fail("Hóa đơn nhập không tồn tại");

                bill.Notes = string.IsNullOrWhiteSpace(dto.Notes) ? bill.Notes : dto.Notes.Trim();
                bill.UpdatedDate = DateTime.Now;

                _importBillRepository.Update(bill);
                _importBillRepository.SaveChanges();
                return Result.Success("Cập nhật hóa đơn nhập thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result DeleteImportBill(string importBillId)
        {
            try
            {
                var bill = _importBillRepository.GetById(importBillId);
                if (bill == null || bill.DeletedDate != null)
                    return Result.Fail("Hóa đơn nhập không tồn tại");

                bill.DeletedDate = DateTime.Now;
                _importBillRepository.Update(bill);
                _importBillRepository.SaveChanges();

                return Result.Success("Đã xóa hóa đơn nhập");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- TRUY VẤN ---------------------------------
        // ==================================================================
        public Result<ImportBill> GetImportBillById(string importBillId)
        {
            try
            {
                var bill = _importBillRepository.GetById(importBillId);
                if (bill == null || bill.DeletedDate != null)
                    return Result<ImportBill>.Fail("Hóa đơn nhập không tồn tại");

                return Result<ImportBill>.Success(bill);
            }
            catch (Exception ex)
            {
                return Result<ImportBill>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<ImportBill>> GetAllImportBills()
        {
            try
            {
                var bills = _importBillRepository.GetAll()
                    .Where(b => b.DeletedDate == null)
                    .OrderByDescending(b => b.CreatedDate);
                return Result<IEnumerable<ImportBill>>.Success(bills);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<ImportBill>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<ImportBill>> GetBySupplier(string supplierId)
        {
            try
            {
                var bills = _importBillRepository.GetBySupplier(supplierId)
                    .Where(b => b.DeletedDate == null)
                    .OrderByDescending(b => b.CreatedDate);
                return Result<IEnumerable<ImportBill>>.Success(bills);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<ImportBill>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<ImportBill>> GetByDateRange(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var bills = _importBillRepository.GetByDateRange(fromDate, toDate)
                    .Where(b => b.DeletedDate == null)
                    .OrderByDescending(b => b.CreatedDate);
                return Result<IEnumerable<ImportBill>>.Success(bills);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<ImportBill>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- CHI TIẾT --------------------------------
        // ==================================================================
        public Result AddImportItem(string importBillId, ImportBillDetailCreateDto item)
        {
            try
            {
                var bill = _importBillRepository.GetById(importBillId);
                if (bill == null || bill.DeletedDate != null)
                    return Result.Fail("Hóa đơn nhập không tồn tại");

                var book = _bookRepository.GetById(item.BookId);
                if (book == null || book.DeletedDate != null)
                    return Result.Fail("Sách không tồn tại");

                if (item.Quantity <= 0 || item.ImportPrice <= 0)
                    return Result.Fail("Số lượng và giá nhập phải > 0");

                var existing = _importBillDetailRepository.GetByImportId(importBillId)
                    .FirstOrDefault(d => d.BookId == item.BookId);
                if (existing != null)
                    return Result.Fail("Sách đã tồn tại trong hóa đơn");

                var detail = new ImportBillDetail
                {
                    ImportId = importBillId,
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    ImportPrice = item.ImportPrice
                };

                _importBillDetailRepository.Add(detail);
                _importBillDetailRepository.SaveChanges();

                // Đồng bộ giá nhập hiện tại cho Book
                book.ImportPrice = item.ImportPrice;
                book.UpdatedDate = DateTime.Now;
                _bookRepository.Update(book);
                _bookRepository.SaveChanges();

                RecalculateBillTotal(importBillId);
                return Result.Success("Đã thêm sách vào hóa đơn");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result RemoveImportItem(string importBillId, string bookId)
        {
            try
            {
                var detail = _importBillDetailRepository.GetByImportId(importBillId)
                    .FirstOrDefault(d => d.BookId == bookId);
                if (detail == null)
                    return Result.Fail("Không tìm thấy sách trong hóa đơn");

                _importBillDetailRepository.SoftDelete(importBillId, bookId);
                _importBillDetailRepository.SaveChanges();

                RecalculateBillTotal(importBillId);
                return Result.Success("Đã xóa sách khỏi hóa đơn");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result UpdateImportItem(string importBillId, string bookId, int newQuantity, decimal? newPrice)
        {
            try
            {
                if (newQuantity <= 0)
                    return Result.Fail("Số lượng phải > 0");

                var detail = _importBillDetailRepository.GetByImportId(importBillId)
                    .FirstOrDefault(d => d.BookId == bookId);
                if (detail == null)
                    return Result.Fail("Không tìm thấy sách trong hóa đơn");

                detail.Quantity = newQuantity;
                if (newPrice.HasValue && newPrice.Value > 0)
                    detail.ImportPrice = newPrice.Value;

                _importBillDetailRepository.Update(detail);
                _importBillDetailRepository.SaveChanges();

                // Nếu có đổi giá nhập, đồng bộ sang Book.ImportPrice
                if (newPrice.HasValue && newPrice.Value > 0)
                {
                    var book = _bookRepository.GetById(bookId);
                    if (book != null && book.DeletedDate == null)
                    {
                        book.ImportPrice = newPrice.Value;
                        book.UpdatedDate = DateTime.Now;
                        _bookRepository.Update(book);
                        _bookRepository.SaveChanges();
                    }
                }

                RecalculateBillTotal(importBillId);
                return Result.Success("Đã cập nhật chi tiết");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<ImportBillDetail>> GetImportDetails(string importBillId)
        {
            try
            {
                var details = _importBillDetailRepository.GetByImportId(importBillId);
                return Result<IEnumerable<ImportBillDetail>>.Success(details);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<ImportBillDetail>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- BÁO CÁO ---------------------------------
        // ==================================================================
        public Result<decimal> CalculateTotalImportBySupplier(string supplierId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var bills = _importBillRepository.GetBySupplier(supplierId)
                    .Where(b => b.CreatedDate >= fromDate && b.CreatedDate <= toDate && b.DeletedDate == null);
                var total = bills.Sum(b => b.TotalAmount);
                return Result<decimal>.Success(total);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<decimal> CalculateTotalImportByDateRange(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var bills = _importBillRepository.GetByDateRange(fromDate, toDate)
                    .Where(b => b.DeletedDate == null);
                var total = bills.Sum(b => b.TotalAmount);
                return Result<decimal>.Success(total);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- HELPER -----------------------------------
        // ==================================================================
        private void RecalculateBillTotal(string importBillId)
        {
            var bill = _importBillRepository.GetById(importBillId);
            if (bill == null) return;

            var details = _importBillDetailRepository.GetByImportId(importBillId);
            bill.TotalAmount = details.Sum(d => d.ImportPrice * d.Quantity);
            bill.UpdatedDate = DateTime.Now;

            _importBillRepository.Update(bill);
            _importBillRepository.SaveChanges();
        }

        private string GenerateImportId()
        {
            var last = _importBillRepository.GetAll()
                .OrderByDescending(b => b.Id)
                .FirstOrDefault();

            if (last == null || !last.Id.StartsWith("PN"))
                return "PN0001";

            var num = int.Parse(last.Id.Substring(2));
            return $"PN{(num + 1):D4}";
        }
    }
}