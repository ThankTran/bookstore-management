using System;
using System.Collections.Generic;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs;
using bookstore_Management.Models;
namespace bookstore_Management.Services.Interfaces
{
    public interface ISupplierService
    {
        // CRUD
        Result<string> AddSupplier(SupplierDto dto);
        Result UpdateSupplier(string supplierId, SupplierDto dto);
        Result DeleteSupplier(string supplierId);
        Result<Supplier> GetSupplierById(string supplierId);
        Result<IEnumerable<Supplier>> GetAllSuppliers();
        // Tìm kiếm
        Result<Supplier> GetSupplierByPhone(string phone);
        Result<Supplier> GetSupplierByEmail(string email);

        // Quản lý sách từ NCC
        Result<IEnumerable<Book>> GetBooksBySupplier(string supplierId);

        // Báo cáo
        Result<decimal> CalculateTotalImportValue(string supplierId);
        Result<decimal> CalculateTotalImportValueByDateRange(string supplierId, DateTime fromDate, DateTime toDate);
    }
}