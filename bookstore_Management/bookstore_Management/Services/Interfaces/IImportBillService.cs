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
        Result<int> CreateImportBill(ImportBillDto dto);
        Result UpdateImportBill(int importBillId, ImportBillDto dto);
        Result DeleteImportBill(int importBillId);
        Result<ImportBill> GetImportBillById(int importBillId);
        Result<IEnumerable<ImportBill>> GetAllImportBills();
        // Tìm kiếm
        Result<IEnumerable<ImportBill>> GetBySupplier(string supplierId);
        Result<IEnumerable<ImportBill>> GetByDateRange(DateTime fromDate, DateTime toDate);

        // Quản lý chi tiết
        Result AddImportItem(int importBillId, ImportBillDetailDto item);
        Result RemoveImportItem(int importBillId, string bookId);
        Result UpdateImportItem(int importBillId, string bookId, int newQuantity);
        Result<IEnumerable<ImportBillDetail>> GetImportDetails(int importBillId);

        // Báo cáo
        Result<decimal> CalculateTotalImportBySupplier(string supplierId, DateTime fromDate, DateTime toDate);
        Result<decimal> CalculateTotalImportByDateRange(DateTime fromDate, DateTime toDate);
    }
}