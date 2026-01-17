using System;
using System.Collections.Generic;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs.Publisher.Requests;
using bookstore_Management.DTOs.Publisher.Responses;
using bookstore_Management.Models;
namespace bookstore_Management.Services.Interfaces
{
    public interface IPublisherService
    {
        // CRUD
        Result<string> AddPublisher(CreatePublisherRequestDto dto);
        Result UpdatePublisher(string publisherId, UpdatePublisherRequestDto dto);
        Result DeletePublisher(string publisherId);
        Result<PublisherResponseDto> GetPublisherById(string publisherId);
        Result<IEnumerable<PublisherResponseDto>> GetAllPublishers();
        
        // Tìm kiếm
        Result<PublisherResponseDto> GetPublisherByPhone(string phone);
        Result<PublisherResponseDto> GetPublisherByEmail(string email);
        Result<IEnumerable<PublisherResponseDto>> SearchByName(string name);

        // Quản lý sách từ NCC
        Result<IEnumerable<Book>> GetBooksByPublisher(string publisherId);

        // Báo cáo
        Result<decimal> CalculateTotalImportValue(string publisherId);
        Result<decimal> CalculateTotalImportValueByDateRange(string publisherId, DateTime fromDate, DateTime toDate);
    }
}