using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs.ImportBill.Requests;
using bookstore_Management.DTOs.ImportBill.Responses;

namespace bookstore_Management.Services.Interfaces
{
    /// <summary>
    /// Service interface quản lý hóa đơn nhập từ nhà cung cấp
    /// </summary>
    public interface IImportBillService
    {
        
        Task<Result<string>> CreateImportBillAsync(CreateImportBillRequestDto dto);

  
        Task<Result> UpdateImportBillAsync(string importBillId, UpdateImportBillRequestDto dto);

   
        Task<Result> DeleteImportBillAsync(string importBillId);

  
        Task<Result<ImportBillResponseDto>> GetImportBillByIdAsync(string importBillId);


        Task<Result<IEnumerable<ImportBillResponseDto>>> GetAllImportBillsAsync();

  
        Task<Result<IEnumerable<ImportBillResponseDto>>> GetBySupplierAsync(string supplierId);

        
        Task<Result<IEnumerable<ImportBillResponseDto>>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);

        
        Task<Result> RemoveImportItemAsync(string importBillId, string bookId);

    
        Task<Result> UpdateImportItemAsync(string importBillId, string bookId, int newQuantity, decimal? newPrice);

        
        Task<Result<IEnumerable<ImportBillDetailResponseDto>>> GetImportDetailsAsync(string importBillId);

        
        Task<Result<decimal>> CalculateTotalImportBySupplierAsync(string supplierId, DateTime fromDate, DateTime toDate);

     
        Task<Result<decimal>> CalculateTotalImportByDateRangeAsync(DateTime fromDate, DateTime toDate);
    }
}