using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs.Publisher.Requests;
using bookstore_Management.DTOs.Publisher.Responses;
using bookstore_Management.Models;

namespace bookstore_Management.Services.Interfaces
{
    public interface IPublisherService
    {
        Task<Result<string>> AddPublisherAsync(CreatePublisherRequestDto dto);
        Task<Result> UpdatePublisherAsync(string publisherId, UpdatePublisherRequestDto dto);
        Task<Result> DeletePublisherAsync(string publisherId);
        Task<Result<PublisherResponseDto>> GetPublisherByIdAsync(string publisherId);
        Task<Result<IEnumerable<PublisherResponseDto>>> GetAllPublishersAsync();
        Task<Result<PublisherResponseDto>> GetPublisherByPhoneAsync(string phone);
        Task<Result<PublisherResponseDto>> GetPublisherByEmailAsync(string email);
        Task<Result<IEnumerable<PublisherResponseDto>>> SearchByNameAsync(string name);
        Task<Result<IEnumerable<Book>>> GetBooksByPublisherAsync(string publisherId);
        Task<Result<decimal>> CalculateTotalImportValueAsync(string publisherId);
        Task<Result<decimal>> CalculateTotalImportValueByDateRangeAsync(string publisherId, DateTime fromDate, DateTime toDate);
    }
}