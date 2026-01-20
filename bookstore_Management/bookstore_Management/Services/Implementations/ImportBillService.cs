using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.DTOs.ImportBill.Requests;
using bookstore_Management.DTOs.ImportBill.Responses;
using bookstore_Management.Models;
using bookstore_Management.Services.Interfaces;

namespace bookstore_Management.Services.Implementations
{
    public class ImportBillService : IImportBillService
    {
        private readonly IUnitOfWork _unitOfWork;

        internal ImportBillService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ==================================================================
        // ---------------------- THÊM DỮ LIỆU ------------------------------
        // ==================================================================
        public async Task<Result<string>> CreateImportBillAsync(CreateImportBillRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.PublisherId))
                return Result<string>.Fail("Nhà cung cấp bắt buộc");

            var supplier = await _unitOfWork.Publishers.GetByIdAsync(dto.PublisherId);
            if (supplier == null || supplier.DeletedDate != null)
                return Result<string>.Fail("Nhà cung cấp không tồn tại");

            if (dto.ImportBillDetails == null || !dto.ImportBillDetails.Any())
                return Result<string>.Fail("Hóa đơn phải có ít nhất 1 sách");

            // Lấy toàn bộ Book 1 lần
            var bookIds = dto.ImportBillDetails.Select(x => x.BookId).ToList();
            var books = await _unitOfWork.Books.Query(x => bookIds.Contains(x.BookId) && x.DeletedDate == null)
                                                .ToListAsync();

            if (books.Count != bookIds.Count)
                return Result<string>.Fail("Một số sách không tồn tại");

            var importId = await GenerateImportIdAsync();

            decimal totalAmount = 0;
            var details = new List<ImportBillDetail>();

            foreach (var item in dto.ImportBillDetails)
            {
                if (item.Quantity <= 0)
                    return Result<string>.Fail("Số lượng phải > 0");

                if (item.ImportPrice <= 0)
                    return Result<string>.Fail("Giá nhập phải > 0");

                var line = item.Quantity * item.ImportPrice;
                totalAmount += line;

                details.Add(new ImportBillDetail
                {
                    ImportId = importId,
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    ImportPrice = item.ImportPrice
                });

                var book = books.First(x => x.BookId == item.BookId);
                book.Stock += item.Quantity;
                book.UpdatedDate = DateTime.Now;
            }

            var importBill = new ImportBill
            {
                Id = importId,
                PublisherId = dto.PublisherId,
                TotalAmount = totalAmount,
                Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim(),
                CreatedBy = string.IsNullOrWhiteSpace(dto.CreatedBy) ? "SYSTEM" : dto.CreatedBy,
                CreatedDate = DateTime.Now,
                ImportBillDetails = details
            };

            await _unitOfWork.ImportBills.AddAsync(importBill);

            // Save all changes only once
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Success(importId, "Tạo hóa đơn nhập thành công");
        }

        // ==================================================================
        // ----------------------- SỬA / XÓA --------------------------------
        // ==================================================================
        public async Task<Result> UpdateImportBillAsync(string importBillId, UpdateImportBillRequestDto dto)
        {
            var bill = await _unitOfWork.ImportBills.GetByIdAsync(importBillId);
            if (bill == null || bill.DeletedDate != null)
                return Result.Fail("Hóa đơn nhập không tồn tại");

            bill.Notes = string.IsNullOrWhiteSpace(dto.Notes) ? bill.Notes : dto.Notes.Trim();
            bill.UpdatedDate = DateTime.Now;

            _unitOfWork.ImportBills.Update(bill);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success("Cập nhật hóa đơn nhập thành công");
        }

        public async Task<Result> DeleteImportBillAsync(string importBillId)
        {
            var bill = await _unitOfWork.ImportBills.GetByIdAsync(importBillId);
            if (bill == null || bill.DeletedDate != null)
                return Result.Fail("Hóa đơn nhập không tồn tại");

            bill.DeletedDate = DateTime.Now;

            _unitOfWork.ImportBills.Update(bill);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success("Đã xóa hóa đơn nhập");
        }

        // ==================================================================
        // ----------------------- TRUY VẤN ---------------------------------
        // ==================================================================
        public async Task<Result<ImportBillResponseDto>> GetImportBillByIdAsync(string importBillId)
        {
            var bill = await _unitOfWork.ImportBills
                .Query(b => b.Id == importBillId && b.DeletedDate == null)
                .Include(b => b.Publisher)
                .Include(b => b.ImportBillDetails)
                .Include(b => b.ImportBillDetails.Select(d => d.Book))
                .FirstOrDefaultAsync();

            if (bill == null)
                return Result<ImportBillResponseDto>.Fail("Hóa đơn nhập không tồn tại");

            return Result<ImportBillResponseDto>.Success(MapToImportBillResponseDto(bill));
        }


        public async Task<Result<IEnumerable<ImportBillResponseDto>>> GetAllImportBillsAsync()
        {
            var list = await _unitOfWork.ImportBills.Query(b => b.DeletedDate == null)
                .Include(b => b.Publisher)
                .Include(b => b.ImportBillDetails)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync();

            return Result<IEnumerable<ImportBillResponseDto>>.Success(
                list.Select(MapToImportBillResponseDto).ToList()
            );
        }
        
        public async Task<Result<IEnumerable<ImportBillResponseDto>>> SearchImportBillsAsync(string keyword)
        {
            keyword = keyword?.Trim().ToLower() ?? "";

            var list = await _unitOfWork.ImportBills
                .Query(x => x.DeletedDate == null &&
                            x.Id.ToString().ToLower().Contains(keyword))
                .Include(x => x.Publisher)
                .Include(x => x.ImportBillDetails)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();

            return list.Select(MapToImportBillResponseDto).ToList();
        }


        public async Task<Result<IEnumerable<ImportBillResponseDto>>> GetBySupplierAsync(string supplierId)
        {
            var list = await _unitOfWork.ImportBills.Query(
                b => b.PublisherId == supplierId && b.DeletedDate == null)
                .Include(b => b.Publisher)
                .Include(b => b.ImportBillDetails)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync(); 

            return Result<IEnumerable<ImportBillResponseDto>>.Success(
                list.Select(MapToImportBillResponseDto).ToList()
            );
        }

        public async Task<Result<IEnumerable<ImportBillResponseDto>>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            var list = await _unitOfWork.ImportBills.Query(
                b => b.CreatedDate >= fromDate && b.CreatedDate <= toDate && b.DeletedDate == null)
                .Include(b => b.Publisher)
                .Include(b => b.ImportBillDetails)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync();

            return Result<IEnumerable<ImportBillResponseDto>>.Success(
                list.Select(MapToImportBillResponseDto).ToList()
            );
        }

        // ==================================================================
        // ----------------------- CHI TIẾT --------------------------------
        // ==================================================================
        public async Task<Result> RemoveImportItemAsync(string importBillId, string bookId)
        {
            await _unitOfWork.ImportBillDetails.SoftDeleteAsync(importBillId, bookId);
            await RecalculateBillTotalAsync(importBillId);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success("Đã xóa sách khỏi hóa đơn");
        }

        public async Task<Result> UpdateImportItemAsync(string importBillId, string bookId, int newQuantity, decimal? newPrice)
        {
            if (newQuantity <= 0)
                return Result.Fail("Số lượng phải > 0");

            var detail = await _unitOfWork.ImportBillDetails.Query(
                d => d.ImportId == importBillId && d.BookId == bookId && d.DeletedDate == null)
                .FirstOrDefaultAsync();

            if (detail == null)
                return Result.Fail("Không tìm thấy sách trong hóa đơn");

            detail.Quantity = newQuantity;
            if (newPrice.HasValue && newPrice.Value > 0)
                detail.ImportPrice = newPrice.Value;

            _unitOfWork.ImportBillDetails.Update(detail);

            await RecalculateBillTotalAsync(importBillId);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success("Đã cập nhật chi tiết");
        }

        public async Task<Result<IEnumerable<ImportBillDetailResponseDto>>> GetImportDetailsAsync(string importBillId)
        {
            var details = await _unitOfWork.ImportBillDetails.Query(
                d => d.ImportId == importBillId && d.DeletedDate == null)
                .Include(d => d.Book)
                .ToListAsync();

            var list = details.Select(d => new ImportBillDetailResponseDto
            {
                BookId = d.BookId,
                BookName = d.Book?.Name,
                Author = d.Book?.Author,
                Quantity = d.Quantity,
                ImportPrice = d.ImportPrice,
                Subtotal = d.Quantity * d.ImportPrice
            }).ToList();

            return Result<IEnumerable<ImportBillDetailResponseDto>>.Success(list);
        }


        // ==================================================================
        // ----------------------- BÁO CÁO ---------------------------------
        // ==================================================================
        public async Task<Result<decimal>> CalculateTotalImportBySupplierAsync(string supplierId, DateTime fromDate, DateTime toDate)
        {
            var total = await _unitOfWork.ImportBills.Query(
                b => b.PublisherId == supplierId &&
                     b.CreatedDate >= fromDate &&
                     b.CreatedDate <= toDate &&
                     b.DeletedDate == null)
                .SumAsync(b => b.TotalAmount);

            return Result<decimal>.Success(total);
        }

        public async Task<Result<decimal>> CalculateTotalImportByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            var total = await _unitOfWork.ImportBills.Query(
                b => b.CreatedDate >= fromDate &&
                     b.CreatedDate <= toDate &&
                     b.DeletedDate == null)
                .SumAsync(b => b.TotalAmount);

            return Result<decimal>.Success(total);
        }

        // ==================================================================
        // ----------------------- HELPER -----------------------------------
        // ==================================================================
        private async Task RecalculateBillTotalAsync(string importBillId)
        {
            var details = await _unitOfWork.ImportBillDetails.Query(
                d => d.ImportId == importBillId && d.DeletedDate == null)
                .ToListAsync();

            var bill = await _unitOfWork.ImportBills.GetByIdAsync(importBillId);
            if (bill == null) return;

            bill.TotalAmount = details.Sum(d => d.ImportPrice * d.Quantity);
            bill.UpdatedDate = DateTime.Now;

            _unitOfWork.ImportBills.Update(bill);
        }

        private async Task<string> GenerateImportIdAsync()
        {
            var last = await _unitOfWork.ImportBills
                .Query()
                .OrderByDescending(b => b.Id)
                .FirstOrDefaultAsync();

            if (last == null || !last.Id.StartsWith("PN"))
                return "PN0001";

            var num = int.Parse(last.Id.Substring(2));
            return $"PN{(num + 1):D4}";
        }

        private static ImportBillResponseDto MapToImportBillResponseDto(ImportBill bill)
        {
            return new ImportBillResponseDto
            {
                Id = bill.Id,
                PublisherId = bill.PublisherId,
                PublisherName = bill.Publisher?.Name,
                TotalAmount = bill.TotalAmount,
                Notes = bill.Notes,
                CreatedBy = bill.CreatedBy,
                CreatedDate = bill.CreatedDate,
                ImportBillDetails = bill.ImportBillDetails?
                    .Where(d => d.DeletedDate == null)
                    .Select(d => new ImportBillDetailResponseDto
                    {
                        BookId = d.BookId,
                        BookName = d.Book?.Name,
                        Author = d.Book?.Author,
                        Quantity = d.Quantity,
                        ImportPrice = d.ImportPrice,
                        Subtotal = d.Quantity * d.ImportPrice
                    }).ToList()
            };
        }
    }
}
