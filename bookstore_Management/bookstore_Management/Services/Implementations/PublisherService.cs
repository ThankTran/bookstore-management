using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IUnitOfWork _unitOfWork;

        internal PublisherService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ==================================================================
        // ---------------------- THÊM DỮ LIỆU ------------------------------
        // ==================================================================
        public async Task<Result<string>> AddPublisherAsync(CreatePublisherRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result<string>.Fail("Tên nhà cung cấp không được để trống");
                
            if (string.IsNullOrWhiteSpace(dto.Phone))
                return Result<string>.Fail("Số điện thoại không được để trống");
            
            if (dto.Phone.Length < 10 || dto.Phone.Length > 20 || !dto.Phone.All(char.IsDigit))
                return Result<string>.Fail("Số điện thoại phải từ 10-20 chữ số");
            
            if (!string.IsNullOrWhiteSpace(dto.Email) && !IsValidEmail(dto.Email))
                return Result<string>.Fail("Email không hợp lệ");
            
            // Batch check phone và email cùng lúc để giảm queries
            var phoneTask = _unitOfWork.Publishers.GetByPhoneAsync(dto.Phone);
            Task<Publisher> emailTask = null;
            
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                emailTask = _unitOfWork.Publishers.GetByEmailAsync(dto.Email.Trim());
                await Task.WhenAll(phoneTask, emailTask);
            }
            else
            {
                await phoneTask;
            }
            
            var existingPhone = phoneTask.Result;
            if (existingPhone != null && existingPhone.DeletedDate == null)
                return Result<string>.Fail("Số điện thoại đã tồn tại trong hệ thống");
            
            if (emailTask != null)
            {
                var existingEmail = emailTask.Result;
                if (existingEmail != null && existingEmail.DeletedDate == null)
                    return Result<string>.Fail("Email đã tồn tại trong hệ thống");
            }
            
            var publisherId = await GeneratePublisherIdAsync();
            
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
            
            await _unitOfWork.Publishers.AddAsync(supplier);
            await _unitOfWork.SaveChangesAsync();
            
            return Result<string>.Success(publisherId, "Thêm nhà cung cấp thành công");
        }

        // ==================================================================
        // ----------------------- SỬA DỮ LIỆU ------------------------------
        // ==================================================================
        public async Task<Result> UpdatePublisherAsync(string publisherId, UpdatePublisherRequestDto dto)
        {
            var supplier = await _unitOfWork.Publishers.GetByIdAsync(publisherId);
            if (supplier == null || supplier.DeletedDate != null)
                return Result.Fail("Nhà cung cấp không tồn tại");
                
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result.Fail("Tên nhà cung cấp không được để trống");
                
            if (string.IsNullOrWhiteSpace(dto.Phone))
                return Result.Fail("Số điện thoại không được để trống");
            
            if (dto.Phone.Length < 10 || dto.Phone.Length > 20 || !dto.Phone.All(char.IsDigit))
                return Result.Fail("Số điện thoại phải từ 10-20 chữ số");

            if (!string.IsNullOrWhiteSpace(dto.Email) && !IsValidEmail(dto.Email))
                return Result.Fail("Email không hợp lệ");
                
            // Kiểm tra số điện thoại đã tồn tại (chỉ khi thay đổi)
            if (dto.Phone != supplier.Phone)
            {
                var existingPhone = await _unitOfWork.Publishers.GetByPhoneAsync(dto.Phone);
                if (existingPhone != null && existingPhone.Id != publisherId && existingPhone.DeletedDate == null)
                    return Result.Fail("Số điện thoại đã tồn tại trong hệ thống");
            }
            
            supplier.Name = dto.Name.Trim();
            supplier.Phone = dto.Phone.Trim();
            supplier.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
            supplier.UpdatedDate = DateTime.Now;
            
            _unitOfWork.Publishers.Update(supplier);
            await _unitOfWork.SaveChangesAsync();
            
            return Result.Success("Cập nhật nhà cung cấp thành công");
        }

        // ==================================================================
        // ---------------------- XÓA DỮ LIỆU -------------------------------
        // ==================================================================
        public async Task<Result> DeletePublisherAsync(string publisherId)
        {
            var supplier = await _unitOfWork.Publishers.GetByIdAsync(publisherId);
            if (supplier == null || supplier.DeletedDate != null)
                return Result.Fail("Nhà cung cấp không tồn tại");
            
            supplier.DeletedDate = DateTime.Now;
            _unitOfWork.Publishers.Update(supplier);
            await _unitOfWork.SaveChangesAsync();
            
            return Result.Success($"Xóa nhà cung cấp {supplier.Name} thành công");
        }

        // ==================================================================
        // ----------------------- LẤY DỮ LIỆU ------------------------------
        // ==================================================================
        public async Task<Result<PublisherResponseDto>> GetPublisherByIdAsync(string publisherId)
        {
            var supplier = await _unitOfWork.Publishers.GetByIdAsync(publisherId);
            if (supplier == null || supplier.DeletedDate != null)
                return Result<PublisherResponseDto>.Fail("Nhà cung cấp không tồn tại");

            var dto = MapToPublisherResponseDto(supplier);
            return Result<PublisherResponseDto>.Success(dto);
        }

        public async Task<Result<IEnumerable<PublisherResponseDto>>> GetAllPublishersAsync()
        {
            var allPublishers = await _unitOfWork.Publishers.GetAllAsync();
            var publishers = allPublishers
                .Where(s => s.DeletedDate == null)
                .OrderBy(s => s.Name)
                .Select(MapToPublisherResponseDto)
                .ToList();

            return Result<IEnumerable<PublisherResponseDto>>.Success(publishers);
        }

        public async Task<Result<PublisherResponseDto>> GetPublisherByPhoneAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return Result<PublisherResponseDto>.Fail("Số điện thoại không được để trống");
                
            var supplier = await _unitOfWork.Publishers.GetByPhoneAsync(phone.Trim());
            if (supplier == null || supplier.DeletedDate != null)
                return Result<PublisherResponseDto>.Fail("Không tìm thấy nhà cung cấp");

            var dto = MapToPublisherResponseDto(supplier);
            return Result<PublisherResponseDto>.Success(dto);
        }

        public async Task<Result<PublisherResponseDto>> GetPublisherByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Result<PublisherResponseDto>.Fail("Email không được để trống");

            var supplier = await _unitOfWork.Publishers.GetByEmailAsync(email.Trim());
            if (supplier == null || supplier.DeletedDate != null)
                return Result<PublisherResponseDto>.Fail("Không tìm thấy nhà cung cấp");

            var dto = MapToPublisherResponseDto(supplier);
            return Result<PublisherResponseDto>.Success(dto);
        }

        public async Task<Result<IEnumerable<PublisherResponseDto>>> SearchByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<IEnumerable<PublisherResponseDto>>.Success(new List<PublisherResponseDto>());

            var allPublishers = await Task.Run(() => 
                _unitOfWork.Publishers.SearchByName(name.Trim())
                    .Where(s => s.DeletedDate == null)
                    .OrderBy(s => s.Name)
                    .ToList()
            );

            var publishers = allPublishers.Select(MapToPublisherResponseDto).ToList();
            return Result<IEnumerable<PublisherResponseDto>>.Success(publishers);
        }

        public async Task<Result<IEnumerable<Book>>> GetBooksByPublisherAsync(string publisherId)
        {
            var supplier = await _unitOfWork.Publishers.GetByIdAsync(publisherId);
            if (supplier == null || supplier.DeletedDate != null)
                return Result<IEnumerable<Book>>.Fail("Nhà cung cấp không tồn tại");

            var books = await _unitOfWork.Books.FindAsync(b => 
                b.PublisherId == publisherId && b.DeletedDate == null);

            var orderedBooks = books.OrderBy(b => b.Name).ToList();
            return Result<IEnumerable<Book>>.Success(orderedBooks);
        }

        // ==================================================================
        // ----------------------- BÁO CÁO NHẬP HÀNG -------------------------
        // ==================================================================
        public async Task<Result<decimal>> CalculateTotalImportValueAsync(string publisherId)
        {
            var supplier = await _unitOfWork.Publishers.GetByIdAsync(publisherId);
            if (supplier == null || supplier.DeletedDate != null)
                return Result<decimal>.Fail("Nhà cung cấp không tồn tại");
                
            var importBills = await _unitOfWork.ImportBills.FindAsync(ib =>
                ib.PublisherId == publisherId && ib.DeletedDate == null);

            var totalValue = importBills.Sum(ib => ib.TotalAmount);
            
            return Result<decimal>.Success(totalValue, $"Tổng giá trị nhập: {totalValue:N0} VND");
        }

        public async Task<Result<decimal>> CalculateTotalImportValueByDateRangeAsync(
            string publisherId, DateTime fromDate, DateTime toDate)
        {
            var supplier = await _unitOfWork.Publishers.GetByIdAsync(publisherId);
            if (supplier == null || supplier.DeletedDate != null)
                return Result<decimal>.Fail("Nhà cung cấp không tồn tại");

            var importBills = await _unitOfWork.ImportBills.FindAsync(ib =>
                ib.PublisherId == publisherId &&
                ib.CreatedDate >= fromDate &&
                ib.CreatedDate <= toDate &&
                ib.DeletedDate == null);

            decimal totalValue = importBills.Sum(ib => ib.TotalAmount);

            return Result<decimal>.Success(totalValue, 
                $"Tổng giá trị nhập {fromDate:dd/MM/yyyy} - {toDate:dd/MM/yyyy}: {totalValue:N0} VND");
        }

        // ==================================================================
        // ----------------------- HÀM HELPER --------------------------------
        // ==================================================================
        private async Task<string> GeneratePublisherIdAsync()
        {
            try
            {
                var allPublishers = await _unitOfWork.Publishers.GetAllAsync();
                var lastPublisher = allPublishers
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

        private static PublisherResponseDto MapToPublisherResponseDto(Publisher publisher)
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