using System;
using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.DTOs.Publisher.Requests;
using bookstore_Management.DTOs.Publisher.Responses;
using bookstore_Management.Models;
using bookstore_Management.Services.Interfaces;

namespace bookstore_Management.Services.Implementations
{
    public class PublisherService : IPublisherService
    {
        private readonly IPublisherRepository _publisherRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IImportBillRepository _importBillRepository;

        internal PublisherService(
            IPublisherRepository publisherRepository, 
            IBookRepository bookRepository,
            IImportBillRepository importBillRepository)
        {
            _publisherRepository = publisherRepository;
            _bookRepository = bookRepository;
            _importBillRepository = importBillRepository;
        }

        // ==================================================================
        // ---------------------- THÊM DỮ LIỆU ------------------------------
        // ==================================================================
        public Result<string> AddPublisher(CreatePublisherRequestDto dto)
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
                var existingPhone = _publisherRepository.GetByPhone(dto.Phone);
                if (existingPhone != null && existingPhone.DeletedDate == null)
                    return Result<string>.Fail("Số điện thoại đã tồn tại trong hệ thống");
                
                if (!string.IsNullOrWhiteSpace(dto.Email))
                {
                    var existingEmail = _publisherRepository.GetByEmail(dto.Email.Trim());
                    if (existingEmail != null && existingEmail.DeletedDate == null)
                        return Result<string>.Fail("Email đã tồn tại trong hệ thống");
                }
                
                
                // Generate ID
                var publisherId = GeneratePublisherId();
                
                var supplier = new Publisher
                {
                    Id = publisherId,
                    Name = dto.Name.Trim(),
                    Phone = dto.Phone.Trim(),
                    Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim(),
                    CreatedDate = DateTime.Now,
                    UpdatedDate = null,
                    DeletedDate = null
                };
                
                _publisherRepository.Add(supplier);
                _publisherRepository.SaveChanges();
                
                return Result<string>.Success(publisherId, "Thêm nhà cung cấp thành công");
            }
            catch (Exception ex)
            {
                return Result<string>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- SỬA DỮ LIỆU ------------------------------
        // ==================================================================
        public Result UpdatePublisher(string publisherId, UpdatePublisherRequestDto dto)
        {
            try
            {
                var supplier = _publisherRepository.GetById(publisherId);
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
                    var existingPhone = _publisherRepository.GetByPhone(dto.Phone);
                    if (existingPhone != null && existingPhone.Id != publisherId && existingPhone.DeletedDate == null)
                        return Result.Fail("Số điện thoại đã tồn tại trong hệ thống");
                }
                
                
                supplier.Name = dto.Name.Trim();
                supplier.Phone = dto.Phone.Trim();
                supplier.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
                supplier.UpdatedDate = DateTime.Now;
                
                _publisherRepository.Update(supplier);
                _publisherRepository.SaveChanges();
                
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
        public Result DeletePublisher(string publisherId)
        {
            try
            {
                var supplier = _publisherRepository.GetById(publisherId);
                if (supplier == null || supplier.DeletedDate != null)
                    return Result.Fail("Nhà cung cấp không tồn tại");
                
                // Soft delete
                supplier.DeletedDate = DateTime.Now;
                _publisherRepository.Update(supplier);
                _publisherRepository.SaveChanges();
                
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
        public Result<PublisherResponseDto> GetPublisherById(string publisherId)
        {
            try
            {
                var supplier = _publisherRepository.GetById(publisherId);
                if (supplier == null || supplier.DeletedDate != null)
                    return Result<PublisherResponseDto>.Fail("Nhà cung cấp không tồn tại");

                var dto = MapToPublisherResponseDto(supplier);
                return Result<PublisherResponseDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return Result<PublisherResponseDto>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<PublisherResponseDto>> GetAllPublishers()
        {
            try
            {
                var publishers = _publisherRepository.GetAll()
                    .Where(s => s.DeletedDate == null)
                    .OrderBy(s => s.Name)
                    .Select(MapToPublisherResponseDto);
                return Result<IEnumerable<PublisherResponseDto>>.Success(publishers);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<PublisherResponseDto>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<PublisherResponseDto> GetPublisherByPhone(string phone)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phone))
                    return Result<PublisherResponseDto>.Fail("Số điện thoại không được để trống");
                    
                var supplier = _publisherRepository.GetByPhone(phone.Trim());
                if (supplier == null || supplier.DeletedDate != null)
                    return Result<PublisherResponseDto>.Fail("Không tìm thấy nhà cung cấp");

                var dto = MapToPublisherResponseDto(supplier);
                return Result<PublisherResponseDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return Result<PublisherResponseDto>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<PublisherResponseDto> GetPublisherByEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return Result<PublisherResponseDto>.Fail("Email không được để trống");

                var supplier = _publisherRepository.GetByEmail(email.Trim());
                if (supplier == null || supplier.DeletedDate != null)
                    return Result<PublisherResponseDto>.Fail("Không tìm thấy nhà cung cấp");

                var dto = MapToPublisherResponseDto(supplier);
                return Result<PublisherResponseDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return Result<PublisherResponseDto>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<Publisher>> SearchByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return Result<IEnumerable<Publisher>>.Success(new List<Publisher>());
                    
                var publishers = _publisherRepository.SearchByName(name.Trim())
                    .Where(s => s.DeletedDate == null)
                    .OrderBy(s => s.Name)
                    .ToList();
                    
                return Result<IEnumerable<Publisher>>.Success(publishers);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Publisher>>.Fail($"Lỗi: {ex.Message}");
            }
        }
        

        public Result<IEnumerable<Book>> GetBooksByPublisher(string publisherId)
        {
            try
            {
                var supplier = _publisherRepository.GetById(publisherId);
                if (supplier == null || supplier.DeletedDate != null)
                    return Result<IEnumerable<Book>>.Fail("Nhà cung cấp không tồn tại");

                var books = _bookRepository.Find(b => b.PublisherId == publisherId && b.DeletedDate == null)
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
        public Result<decimal> CalculateTotalImportValue(string publisherId)
        {
            try
            {
                var supplier = _publisherRepository.GetById(publisherId);
                if (supplier == null || supplier.DeletedDate != null)
                    return Result<decimal>.Fail("Nhà cung cấp không tồn tại");
                    
                // Lấy từ ImportBill, không phải Book
                var importBills = _importBillRepository.Find(ib =>
                    ib.PublisherId == publisherId);

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
        public Result<decimal> CalculateTotalImportValueByDateRange(string publisherId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var supplier = _publisherRepository.GetById(publisherId);
                if (supplier == null || supplier.DeletedDate != null)
                    return Result<decimal>.Fail("Nhà cung cấp không tồn tại");

                var importBills = _importBillRepository.Find(ib =>
                    ib.PublisherId == publisherId &&
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
        private string GeneratePublisherId()
        {
            try
            {
                var lastPublisher = _publisherRepository.GetAll()
                    .Where(s => s.Id.StartsWith("NCC"))
                    .OrderByDescending(s => s.Id)
                    .FirstOrDefault();
                    
                if (lastPublisher == null)
                    return "NCC001";
                    
                var lastNumber = lastPublisher.Id.Substring(3);
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
        /// Maps Publisher entity to PublisherResponseDto
        /// </summary>
        private PublisherResponseDto MapToPublisherResponseDto(Publisher publisher)
        {
            return new PublisherResponseDto
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