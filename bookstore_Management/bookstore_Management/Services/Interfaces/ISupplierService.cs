using System;
using System.Collections.Generic;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs.Supplier.Requests;
using bookstore_Management.DTOs.Supplier.Responses;
using bookstore_Management.Models;
namespace bookstore_Management.Services.Interfaces
{
    public interface ISupplierService
    {
        // CRUD
        Result<string> AddSupplier(CreateSupplierRequestDto dto);
        Result UpdateSupplier(string supplierId, UpdateSupplierRequestDto dto);
        Result DeleteSupplier(string supplierId);
        Result<SupplierResponseDto> GetSupplierById(string supplierId);
        Result<IEnumerable<SupplierResponseDto>> GetAllSuppliers();
        
        // Tìm kiếm
        Result<SupplierResponseDto> GetSupplierByPhone(string phone);
        Result<SupplierResponseDto> GetSupplierByEmail(string email);

        // Quản lý sách từ NCC
        Result<IEnumerable<Book>> GetBooksBySupplier(string supplierId);

        // Báo cáo
        Result<decimal> CalculateTotalImportValue(string supplierId);
        Result<decimal> CalculateTotalImportValueByDateRange(string supplierId, DateTime fromDate, DateTime toDate);
    }
}