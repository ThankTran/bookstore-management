using System;
using System.Collections.Generic;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs;
using bookstore_Management.Models;
namespace bookstore_Management.Services.Interfaces
{
    public interface IImportBillService
    {
        // CRUD
        Result<string> CreateImportBill(ImportBillCreateDto dto);
        Result UpdateImportBill(string importBillId, ImportBillUpdateDto dto);
        Result DeleteImportBill(string importBillId);
        Result<ImportBill> GetImportBillById(string importBillId);
        Result<IEnumerable<ImportBill>> GetAllImportBills();
        // Tìm kiếm
        Result<IEnumerable<ImportBill>> GetBySupplier(string supplierId);
        Result<IEnumerable<ImportBill>> GetByDateRange(DateTime fromDate, DateTime toDate);

        // Quản lý chi tiết
        Result AddImportItem(string importBillId, ImportBillDetailCreateDto item);
        Result RemoveImportItem(string importBillId, string bookId);
        Result UpdateImportItem(string importBillId, string bookId, int newQuantity, decimal? newPrice);
        Result<IEnumerable<ImportBillDetail>> GetImportDetails(string importBillId);

        // Báo cáo
        Result<decimal> CalculateTotalImportBySupplier(string supplierId, DateTime fromDate, DateTime toDate);
        Result<decimal> CalculateTotalImportByDateRange(DateTime fromDate, DateTime toDate);
    }
}