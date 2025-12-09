using System;
using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.DTOs;
using bookstore_Management.Models;
using bookstore_Management.Services.Interfaces;

namespace bookstore_Management.Services.Implementations
{
    /// <summary>
    /// Service quản lý tồn kho
    /// </summary>
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepository;
        private readonly IBookRepository _bookRepository;

        internal StockService(IStockRepository stockRepository, IBookRepository bookRepository)
        {
            _stockRepository = stockRepository;
            _bookRepository = bookRepository;
        }

        // ==================================================================
        // ----------------------- LẤY DỮ LIỆU ------------------------------
        // ==================================================================
        
        /// <summary>
        /// Lấy tồn kho theo mã sách
        /// </summary>
        public Result<Stock> GetStockByBookId(string bookId)
        {
            try
            {
                var stock = _stockRepository.GetById(bookId);
                if (stock == null)
                    return Result<Stock>.Fail("Sách không có tồn kho");

                return Result<Stock>.Success(stock);
            }
            catch (Exception ex)
            {
                return Result<Stock>.Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy tất cả tồn kho
        /// </summary>
        public Result<IEnumerable<Stock>> GetAllStocks()
        {
            try
            {
                var stocks = _stockRepository.GetAll();
                return Result<IEnumerable<Stock>>.Success(stocks);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Stock>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy các sách hết hàng
        /// </summary>
        public Result<IEnumerable<Stock>> GetOutOfStockBooks()
        {
            try
            {
                var outOfStocks = _stockRepository.Find(s => s.StockQuantity == 0);
                return Result<IEnumerable<Stock>>.Success(outOfStocks);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Stock>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy các sách sắp hết hàng
        /// </summary>
        public Result<IEnumerable<Stock>> GetLowStockBooks(int minStock = 5)
        {
            try
            {
                var lowStocks = _stockRepository.Find(s => s.StockQuantity > 0 && s.StockQuantity <= minStock);
                return Result<IEnumerable<Stock>>.Success(lowStocks);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Stock>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ---------------------- CẬP NHẬT TỒN KHO -------------------------
        // ==================================================================

        /// <summary>
        /// Thêm số lượng vào tồn kho (dùng khi nhập hàng)
        /// </summary>
        public Result AddStockQuantity(string bookId, int quantityToAdd)
        {
            try
            {
                if (quantityToAdd <= 0)
                    return Result.Fail("Số lượng thêm phải > 0");

                var stock = _stockRepository.GetById(bookId);
                if (stock == null)
                    return Result.Fail("Sách không tồn tại trong kho");

                stock.StockQuantity += quantityToAdd;
                _stockRepository.Update(stock);
                _stockRepository.SaveChanges();

                return Result.Success($"Thêm {quantityToAdd} sách thành công. Tổng: {stock.StockQuantity}");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Trừ số lượng từ tồn kho (dùng khi bán hàng)
        /// </summary>
        public Result SubtractStockQuantity(string bookId, int quantityToSubtract)
        {
            try
            {
                if (quantityToSubtract <= 0)
                    return Result.Fail("Số lượng trừ phải > 0");

                var stock = _stockRepository.GetById(bookId);
                if (stock == null)
                    return Result.Fail("Sách không tồn tại trong kho");

                if (stock.StockQuantity < quantityToSubtract)
                    return Result.Fail($"Không đủ hàng. Tồn kho: {stock.StockQuantity}");

                stock.StockQuantity -= quantityToSubtract;
                _stockRepository.Update(stock);
                _stockRepository.SaveChanges();

                return Result.Success($"Trừ {quantityToSubtract} sách thành công. Tồn lại: {stock.StockQuantity}");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Set tồn kho bằng giá trị cụ thể
        /// </summary>
        public Result SetStockQuantity(string bookId, int newQuantity)
        {
            try
            {
                if (newQuantity < 0)
                    return Result.Fail("Số lượng không được âm");

                var stock = _stockRepository.GetById(bookId);
                if (stock == null)
                    return Result.Fail("Sách không tồn tại trong kho");

                int oldQuantity = stock.StockQuantity;
                stock.StockQuantity = newQuantity;
                _stockRepository.Update(stock);
                _stockRepository.SaveChanges();

                return Result.Success($"Cập nhật tồn kho thành công. Cũ: {oldQuantity}, Mới: {newQuantity}");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- KIỂM TRA TỒN KHO -------------------------
        // ==================================================================

        /// <summary>
        /// Kiểm tra xem có đủ hàng để bán không
        /// </summary>
        public Result<bool> CheckAvailableStock(string bookId, int requiredQuantity)
        {
            try
            {
                var stock = _stockRepository.GetById(bookId);
                if (stock == null)
                    return Result<bool>.Fail("Sách không tồn tại");

                bool isAvailable = stock.StockQuantity >= requiredQuantity;
                
                if (!isAvailable)
                    return Result<bool>.Success(false, $"Không đủ hàng. Cần: {requiredQuantity}, Tồn: {stock.StockQuantity}");

                return Result<bool>.Success(true, $"Còn hàng. Tồn: {stock.StockQuantity}");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Lỗi: {ex.Message}");
            }
        }
    }
}