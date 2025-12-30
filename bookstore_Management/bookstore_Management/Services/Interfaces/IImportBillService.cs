using System;
using System.Collections.Generic;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs.ImportBill.Requests;
using bookstore_Management.DTOs.ImportBill.Responses;
using bookstore_Management.Models;
namespace bookstore_Management.Services.Interfaces
{
    public interface IImportBillService
    {
        // CRUD
        Result<string> CreateImportBill(CreateImportBillRequestDto dto);
        Result UpdateImportBill(string importBillId, UpdateImportBillRequestDto dto);
        Result DeleteImportBill(string importBillId);
        Result<ImportBillResponseDto> GetImportBillById(string importBillId);
        Result<IEnumerable<ImportBillResponseDto>> GetAllImportBills();
        
        // Tìm kiếm
        Result<IEnumerable<ImportBillResponseDto>> GetBySupplier(string supplierId);
        Result<IEnumerable<ImportBillResponseDto>> GetByDateRange(DateTime fromDate, DateTime toDate);

        // Quản lý chi tiết
        Result AddImportItem(string importBillId, ImportBillDetailCreateRequestDto item);
        Result RemoveImportItem(string importBillId, string bookId);
        Result UpdateImportItem(string importBillId, string bookId, int newQuantity, decimal? newPrice);
        Result<IEnumerable<ImportBillDetailResponseDto>> GetImportDetails(string importBillId);

        // Báo cáo
        Result<decimal> CalculateTotalImportBySupplier(string supplierId, DateTime fromDate, DateTime toDate);
        Result<decimal> CalculateTotalImportByDateRange(DateTime fromDate, DateTime toDate);
    }
}