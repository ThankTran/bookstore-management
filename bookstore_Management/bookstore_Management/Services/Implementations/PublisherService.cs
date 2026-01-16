using System;
using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.DTOs.Supplier.Requests;
using bookstore_Management.DTOs.Supplier.Responses;
using bookstore_Management.Models;
using bookstore_Management.Services.Interfaces;

namespace bookstore_Management.Services.Implementations
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IImportBillRepository _importBillRepository;

        internal SupplierService(
            ISupplierRepository supplierRepository, 
            IBookRepository bookRepository,
            IImportBillRepository importBillRepository)
        {
            _supplierRepository = supplierRepository;
            _bookRepository = bookRepository;
            _importBillRepository = importBillRepository;
        }

        // ==================================================================
        // ---------------------- THÊM DỮ LIỆU ------------------------------
        // ==================================================================
        public Result<string> AddSupplier(CreateSupplierRequestDto dto)
        {
            try
            {
                // Validate 
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result<string>.Fail("Tên nhà cung cấp không được để trống");
                    
                if (string.IsNullOrWhiteSpace(dto.Phone))
                    return Result<string>.Fail("Số điện thoại không được để trống");
                
                if (dto.Phone.Length < 10 || dto.Phone.Length > 20 || !dto.Phone.All(char.IsDigit))
                    return Result<string>.Fail("Số điện thoại phải từ 10-20 chữ số");
                
                if (!string.IsNullOrWhiteSpace(dto.Email) && !IsValidEmail(dto.Email))
                    return Result<string>.Fail("Email không hợp lệ");
                    
                // Kiểm tra số điện thoại
                var existingPhone = _supplierRepository.GetByPhone(dto.Phone);
                if (existingPhone != null && existingPhone.DeletedDate == null)
                    return Result<string>.Fail("Số điện thoại đã tồn tại trong hệ thống");
                
                if (!string.IsNullOrWhiteSpace(dto.Email))
                {
                    var existingEmail = _supplierRepository.GetByEmail(dto.Email.Trim());
                    if (existingEmail != null && existingEmail.DeletedDate == null)
                        return Result<string>.Fail("Email đã tồn tại trong hệ thống");
                }
                
                
                // Generate ID
                var supplierId = GenerateSupplierId();
                
                var supplier = new Publisher
                {
                    Id = supplierId,
                    Name = dto.Name.Trim(),
                    Phone = dto.Phone.Trim(),
                    Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim(),
                    CreatedDate = DateTime.Now,
                    UpdatedDate = null,
                    DeletedDate = null
                };
                
                _supplierRepository.Add(supplier);
                _supplierRepository.SaveChanges();
                
                return Result<string>.Success(supplierId, "Thêm nhà cung cấp thành công");
            }
            catch (Exception ex)
            {
                return Result<string>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- SỬA DỮ LIỆU ------------------------------
        // ==================================================================
        public Result UpdateSupplier(string supplierId, UpdateSupplierRequestDto dto)
        {
            try
            {
                var supplier = _supplierRepository.GetById(supplierId);
                if (supplier == null || supplier.DeletedDate != null)
                    return Result.Fail("Nhà cung cấp không tồn tại");
                    
                // Validate
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result.Fail("Tên nhà cung cấp không được để trống");
                    
                if (string.IsNullOrWhiteSpace(dto.Phone))
                    return Result.Fail("Số điện thoại không được để trống");
                
                if (dto.Phone.Length < 10 || dto.Phone.Length > 20 || !dto.Phone.All(char.IsDigit))
                    return Result.Fail("Số điện thoại phải từ 10-20 chữ số");

                if (!string.IsNullOrWhiteSpace(dto.Email) && !IsValidEmail(dto.Email))
                    return Result.Fail("Email không hợp lệ");
                    
                // Kiểm tra số điện thoại đã tồn tại 
                if (dto.Phone != supplier.Phone)
                {
                    var existingPhone = _supplierRepository.GetByPhone(dto.Phone);
                    if (existingPhone != null && existingPhone.Id != supplierId && existingPhone.DeletedDate == null)
                        return Result.Fail("Số điện thoại đã tồn tại trong hệ thống");
                }
                
                
                supplier.Name = dto.Name.Trim();
                supplier.Phone = dto.Phone.Trim();
                supplier.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
                supplier.UpdatedDate = DateTime.Now;
                
                _supplierRepository.Update(supplier);
                _supplierRepository.SaveChanges();
                
                return Result.Success("Cập nhật nhà cung cấp thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ---------------------- XÓA DỮ LIỆU -------------------------------
        // ==================================================================
        public Result DeleteSupplier(string supplierId)
        {
            try
            {
                var supplier = _supplierRepository.GetById(supplierId);
                if (supplier == null || supplier.DeletedDate != null)
                    return Result.Fail("Nhà cung cấp không tồn tại");
                
                // Soft delete
                supplier.DeletedDate = DateTime.Now;
                _supplierRepository.Update(supplier);
                _supplierRepository.SaveChanges();
                
                return Result.Success($"Xóa nhà cung cấp {supplier.Name} thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- LẤY DỮ LIỆU ------------------------------
        // ==================================================================
        public Result<SupplierResponseDto> GetSupplierById(string supplierId)
        {
            try
            {
                var supplier = _supplierRepository.GetById(supplierId);
                if (supplier == null || supplier.DeletedDate != null)
                    return Result<SupplierResponseDto>.Fail("Nhà cung cấp không tồn tại");

                var dto = MapToSupplierResponseDto(supplier);
                return Result<SupplierResponseDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return Result<SupplierResponseDto>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<SupplierResponseDto>> GetAllSuppliers()
        {
            try
            {
                var suppliers = _supplierRepository.GetAll()
                    .Where(s => s.DeletedDate == null)
                    .OrderBy(s => s.Name)
                    .Select(MapToSupplierResponseDto);
                return Result<IEnumerable<SupplierResponseDto>>.Success(suppliers);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<SupplierResponseDto>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<SupplierResponseDto> GetSupplierByPhone(string phone)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phone))
                    return Result<SupplierResponseDto>.Fail("Số điện thoại không được để trống");
                    
                var supplier = _supplierRepository.GetByPhone(phone.Trim());
                if (supplier == null || supplier.DeletedDate != null)
                    return Result<SupplierResponseDto>.Fail("Không tìm thấy nhà cung cấp");

                var dto = MapToSupplierResponseDto(supplier);
                return Result<SupplierResponseDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return Result<SupplierResponseDto>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<SupplierResponseDto> GetSupplierByEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return Result<SupplierResponseDto>.Fail("Email không được để trống");

                var supplier = _supplierRepository.GetByEmail(email.Trim());
                if (supplier == null || supplier.DeletedDate != null)
                    return Result<SupplierResponseDto>.Fail("Không tìm thấy nhà cung cấp");

                var dto = MapToSupplierResponseDto(supplier);
                return Result<SupplierResponseDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return Result<SupplierResponseDto>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<Publisher>> SearchByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return Result<IEnumerable<Publisher>>.Success(new List<Publisher>());
                    
                var suppliers = _supplierRepository.SearchByName(name.Trim())
                    .Where(s => s.DeletedDate == null)
                    .OrderBy(s => s.Name)
                    .ToList();
                    
                return Result<IEnumerable<Publisher>>.Success(suppliers);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Publisher>>.Fail($"Lỗi: {ex.Message}");
            }
        }
        

        public Result<IEnumerable<Book>> GetBooksBySupplier(string supplierId)
        {
            try
            {
                var supplier = _supplierRepository.GetById(supplierId);
                if (supplier == null || supplier.DeletedDate != null)
                    return Result<IEnumerable<Book>>.Fail("Nhà cung cấp không tồn tại");

                var books = _bookRepository.Find(b => b.SupplierId == supplierId && b.DeletedDate == null)
                    .OrderBy(b => b.Name)
                    .ToList();

                return Result<IEnumerable<Book>>.Success(books);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Book>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- BÁO CÁO NHẬP HÀNG -------------------------
        // ==================================================================
        public Result<decimal> CalculateTotalImportValue(string supplierId)
        {
            try
            {
                var supplier = _supplierRepository.GetById(supplierId);
                if (supplier == null || supplier.DeletedDate != null)
                    return Result<decimal>.Fail("Nhà cung cấp không tồn tại");
                    
                // Lấy từ ImportBill, không phải Book
                var importBills = _importBillRepository.Find(ib =>
                    ib.SupplierId == supplierId);

                var totalValue = importBills.Sum(ib => ib.TotalAmount);
                
                return Result<decimal>.Success(totalValue, $"Tổng giá trị nhập: {totalValue:N0} VND");
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Tính tổng giá trị nhập theo khoảng ngày
        /// </summary>
        public Result<decimal> CalculateTotalImportValueByDateRange(string supplierId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var supplier = _supplierRepository.GetById(supplierId);
                if (supplier == null || supplier.DeletedDate != null)
                    return Result<decimal>.Fail("Nhà cung cấp không tồn tại");

                var importBills = _importBillRepository.Find(ib =>
                    ib.SupplierId == supplierId &&
                    ib.CreatedDate >= fromDate &&
                    ib.CreatedDate <= toDate);

                decimal totalValue = importBills.Sum(ib => ib.TotalAmount);

                return Result<decimal>.Success(totalValue, 
                    $"Tổng giá trị nhập {fromDate:dd/MM/yyyy} - {toDate:dd/MM/yyyy}: {totalValue:N0} VND");
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- HÀM HELPER --------------------------------
        // ==================================================================
        private string GenerateSupplierId()
        {
            try
            {
                var lastSupplier = _supplierRepository.GetAll()
                    .Where(s => s.Id.StartsWith("NCC"))
                    .OrderByDescending(s => s.Id)
                    .FirstOrDefault();
                    
                if (lastSupplier == null)
                    return "NCC001";
                    
                var lastNumber = lastSupplier.Id.Substring(3);
                return (int.TryParse(lastNumber, out int number)) ?
                    $"NCC{(number + 1):D3}" :
                    "NCC001";
            }
            catch
            {
                return $"NCC{DateTime.Now:yyyyMMddHHmmss}";
            }
        }

        /// <summary>
        /// Maps Supplier entity to SupplierResponseDto
        /// </summary>
        private SupplierResponseDto MapToSupplierResponseDto(Publisher publisher)
        {
            return new SupplierResponseDto
            {
                Id = publisher.Id,
                Name = publisher.Name,
                Phone = publisher.Phone,
                Email = publisher.Email,
                CreatedDate = publisher.CreatedDate
            };
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email.Trim();
            }
            catch
            {
                return false;
            }
        }
    }
}