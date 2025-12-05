using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs;
using bookstore_Management.Models;

namespace bookstore_Management.Services.Interfaces
{
    public interface ISupplierService
    {
        // CRUD cho Supplier
        Result<string> AddSupplier(SupplierDto dto);
        Result UpdateSupplier(string supplierId, SupplierDto dto);
        Result DeleteSupplier(string supplierId);
        Result<Supplier> GetSupplierById(string supplierId);
        Result<IEnumerable<Supplier>> GetAllSuppliers();
        
        // Tìm kiếm
        Result<Supplier> GetSupplierByPhone(string phone);
        Result<IEnumerable<Supplier>> SearchByName(string name);
        Result<Supplier> SearchByEmail(string email);
        
        // Quản lý sách từ NCC
        Result<IEnumerable<Book>> GetBooksBySupplier(string supplierId);
        Result<int> CountBooksBySupplier(string supplierId);
        
        // Báo cáo
        Result<decimal> CalculateTotalImportValue(string supplierId);
    }
}
