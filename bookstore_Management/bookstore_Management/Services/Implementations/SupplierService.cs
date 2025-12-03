using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Repositories;
using bookstore_Management.DTOs;
using bookstore_Management.Models;
using bookstore_Management.Services.Interfaces;

namespace bookstore_Management.Services.Implementations
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IBookRepository _bookRepository;

        public SupplierService(ISupplierRepository supplierRepository, IBookRepository bookRepository)
        {
            _supplierRepository = supplierRepository;
            _bookRepository = bookRepository;
        }

        public Result<string> AddSupplier(SupplierDto dto)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result<string>.Fail("Tên nhà cung cấp không được để trống");
                    
                if (string.IsNullOrWhiteSpace(dto.Phone))
                    return Result<string>.Fail("Số điện thoại không được để trống");
                    
                // Validate phone format
                if (dto.Phone.Length < 10 || dto.Phone.Length > 12 || !dto.Phone.All(char.IsDigit))
                    return Result<string>.Fail("Số điện thoại không hợp lệ");
                    
                // Validate email if provided
                if (!string.IsNullOrWhiteSpace(dto.Email) && !IsValidEmail(dto.Email))
                    return Result<string>.Fail("Email không hợp lệ");
                    
                // Check duplicate phone
                var existingPhone = _supplierRepository.GetByPhone(dto.Phone);
                if (existingPhone != null)
                    return Result<string>.Fail("Số điện thoại đã tồn tại trong hệ thống");
                    
                // Check duplicate email if provided
                if (!string.IsNullOrWhiteSpace(dto.Email))
                {
                    var existingEmail = _supplierRepository.SearchByEmail(dto.Email).FirstOrDefault();
                    if (existingEmail != null)
                        return Result<string>.Fail("Email đã tồn tại trong hệ thống");
                }
                
                // Generate Supplier ID
                string supplierId = GenerateSupplierId();
                
                // Create new supplier
                var supplier = new Supplier
                {
                    Id = supplierId,
                    Name = dto.Name.Trim(),
                    Phone = dto.Phone.Trim(),
                    Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim(),
                    Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim()
                };
                
                // Save to database
                _supplierRepository.Add(supplier);
                _supplierRepository.SaveChanges();
                
                // Log activity
                LogActivity($"Thêm nhà cung cấp mới: {supplier.Name} ({supplier.Phone})");
                
                return Result<string>.Success(supplierId, "Thêm nhà cung cấp thành công");
            }
            catch (Exception ex)
            {
                return Result<string>.Fail($"Lỗi khi thêm nhà cung cấp: {ex.Message}");
            }
        }

        public Result UpdateSupplier(string supplierId, SupplierDto dto)
        {
            try
            {
                // Get existing supplier
                var supplier = _supplierRepository.GetById(supplierId);
                if (supplier == null)
                    return Result.Fail("Nhà cung cấp không tồn tại");
                    
                // Validate required fields
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result.Fail("Tên nhà cung cấp không được để trống");
                    
                if (string.IsNullOrWhiteSpace(dto.Phone))
                    return Result.Fail("Số điện thoại không được để trống");
                    
                // Validate phone format
                if (dto.Phone.Length < 10 || dto.Phone.Length > 12 || !dto.Phone.All(char.IsDigit))
                    return Result.Fail("Số điện thoại không hợp lệ");
                    
                // Validate email if provided
                if (!string.IsNullOrWhiteSpace(dto.Email) && !IsValidEmail(dto.Email))
                    return Result.Fail("Email không hợp lệ");
                    
                // Check duplicate phone (if changed)
                if (dto.Phone != supplier.Phone)
                {
                    var existingPhone = _supplierRepository.GetByPhone(dto.Phone);
                    if (existingPhone != null && existingPhone.Id != supplierId)
                        return Result.Fail("Số điện thoại đã tồn tại trong hệ thống");
                }
                
                // Check duplicate email (if changed)
                if (dto.Email != supplier.Email && !string.IsNullOrWhiteSpace(dto.Email))
                {
                    var existingEmail = _supplierRepository.SearchByEmail(dto.Email).FirstOrDefault();
                    if (existingEmail != null && existingEmail.Id != supplierId)
                        return Result.Fail("Email đã tồn tại trong hệ thống");
                }
                
                // Update supplier information
                supplier.Name = dto.Name.Trim();
                supplier.Phone = dto.Phone.Trim();
                supplier.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
                supplier.Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim();
                
                // Save changes
                _supplierRepository.Update(supplier);
                _supplierRepository.SaveChanges();
                
                // Log activity
                LogActivity($"Cập nhật nhà cung cấp: {supplier.Name} ({supplier.Phone})");
                
                return Result.Success("Cập nhật nhà cung cấp thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi khi cập nhật nhà cung cấp: {ex.Message}");
            }
        }

        public Result DeleteSupplier(string supplierId)
        {
            try
            {
                // Get existing supplier
                var supplier = _supplierRepository.GetById(supplierId);
                if (supplier == null)
                    return Result.Fail("Nhà cung cấp không tồn tại");
                    
                // Check if supplier has books
                var booksBySupplier = GetBooksBySupplier(supplierId);
                if (booksBySupplier.IsSuccess && booksBySupplier.Data.Any())
                    return Result.Fail("Không thể xóa nhà cung cấp đang cung cấp sách");
                
                // Delete supplier
                _supplierRepository.Delete(supplierId);
                _supplierRepository.SaveChanges();
                
                // Log activity
                LogActivity($"Xóa nhà cung cấp: {supplier.Name} ({supplier.Phone})");
                
                return Result.Success("Xóa nhà cung cấp thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi khi xóa nhà cung cấp: {ex.Message}");
            }
        }

        public Result<Supplier> GetSupplierById(string supplierId)
        {
            try
            {
                var supplier = _supplierRepository.GetById(supplierId);
                if (supplier == null)
                    return Result<Supplier>.Fail("Nhà cung cấp không tồn tại");
                    
                return Result<Supplier>.Success(supplier);
            }
            catch (Exception ex)
            {
                return Result<Supplier>.Fail($"Lỗi khi lấy thông tin nhà cung cấp: {ex.Message}");
            }
        }

        public Result<IEnumerable<Supplier>> GetAllSuppliers()
        {
            try
            {
                var suppliers = _supplierRepository.GetAll()
                    .OrderBy(s => s.Name)
                    .ToList();
                    
                return Result<IEnumerable<Supplier>>.Success(suppliers);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Supplier>>.Fail($"Lỗi khi lấy danh sách nhà cung cấp: {ex.Message}");
            }
        }

        public Result<Supplier> GetSupplierByPhone(string phone)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phone))
                    return Result<Supplier>.Fail("Số điện thoại không được để trống");
                    
                var supplier = _supplierRepository.GetByPhone(phone.Trim());
                if (supplier == null)
                    return Result<Supplier>.Fail("Không tìm thấy nhà cung cấp với số điện thoại này");
                    
                return Result<Supplier>.Success(supplier);
            }
            catch (Exception ex)
            {
                return Result<Supplier>.Fail($"Lỗi khi tìm kiếm nhà cung cấp: {ex.Message}");
            }
        }

        public Result<IEnumerable<Supplier>> SearchByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return Result<IEnumerable<Supplier>>.Success(new List<Supplier>());
                    
                var suppliers = _supplierRepository.SearchByName(name.Trim())
                    .OrderBy(s => s.Name)
                    .ToList();
                    
                return Result<IEnumerable<Supplier>>.Success(suppliers);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Supplier>>.Fail($"Lỗi khi tìm kiếm nhà cung cấp: {ex.Message}");
            }
        }

        public Result<IEnumerable<Supplier>> SearchByEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return Result<IEnumerable<Supplier>>.Success(new List<Supplier>());
                    
                var suppliers = _supplierRepository.SearchByEmail(email.Trim())
                    .OrderBy(s => s.Name)
                    .ToList();
                    
                return Result<IEnumerable<Supplier>>.Success(suppliers);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Supplier>>.Fail($"Lỗi khi tìm kiếm nhà cung cấp: {ex.Message}");
            }
        }

        public Result<IEnumerable<Book>> GetBooksBySupplier(string supplierId)
        {
            try
            {
                // Check if supplier exists
                var supplier = _supplierRepository.GetById(supplierId);
                if (supplier == null)
                    return Result<IEnumerable<Book>>.Fail("Nhà cung cấp không tồn tại");
                    
                // Get books by supplier
                var books = _bookRepository.Find(b => b.SupplierId == supplierId)
                    .OrderBy(b => b.Name)
                    .ToList();
                    
                return Result<IEnumerable<Book>>.Success(books);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Book>>.Fail($"Lỗi khi lấy danh sách sách: {ex.Message}");
            }
        }

        public Result<int> CountBooksBySupplier(string supplierId)
        {
            try
            {
                // Check if supplier exists
                var supplier = _supplierRepository.GetById(supplierId);
                if (supplier == null)
                    return Result<int>.Fail("Nhà cung cấp không tồn tại");
                    
                // Count books by supplier
                int count = _bookRepository.Count(b => b.SupplierId == supplierId);
                
                return Result<int>.Success(count);
            }
            catch (Exception ex)
            {
                return Result<int>.Fail($"Lỗi khi đếm sách: {ex.Message}");
            }
        }

        public Result<decimal> CalculateTotalImportValue(string supplierId)
        {
            try
            {
                // Check if supplier exists
                var supplier = _supplierRepository.GetById(supplierId);
                if (supplier == null)
                    return Result<decimal>.Fail("Nhà cung cấp không tồn tại");
                    
                // Get all books from this supplier
                var books = _bookRepository.Find(b => b.SupplierId == supplierId);
                
                // Calculate total import value
                decimal totalValue = books.Sum(b => b.ImportPrice);
                
                return Result<decimal>.Success(totalValue, $"Tổng giá trị nhập: {totalValue:N0} VND");
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi khi tính tổng giá trị nhập: {ex.Message}");
            }
        }

        #region Helper Methods

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
                    
                string lastNumber = lastSupplier.Id.Substring(3); // Remove "NCC" prefix
                if (int.TryParse(lastNumber, out int number))
                {
                    return $"NCC{(number + 1):D3}";
                }
                
                return "NCC001";
            }
            catch
            {
                return $"NCC{DateTime.Now:yyyyMMddHHmmss}";
            }
        }

        private bool IsValidEmail(string email)
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

        private void LogActivity(string message)
        {
            // Implementation for logging activities
            // You can use logging framework like Serilog, NLog, or write to database/file
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] SUPPLIER SERVICE: {message}");
            
            // Example with file logging:
            // File.AppendAllText("activity_log.txt", $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n");
        }

        #endregion
    }
}
