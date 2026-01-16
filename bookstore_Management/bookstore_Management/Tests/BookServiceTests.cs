// using NUnit.Framework;
// using bookstore_Management.Data;
// using bookstore_Management.Models;
// using bookstore_Management.Data.Repositories;
// using bookstore_Management.Services;
// using System;
// using System.Linq;
// using bookstore_Management.Core.Enums;
// using bookstore_Management.Data.Context;
// using bookstore_Management.Data.Repositories.Implementations;
// using bookstore_Management.DTOs.Book.Requests;
// using bookstore_Management.Services.Implementations;
//
// namespace BookstoreTests
// {
//     [TestFixture]
//     public class RealDatabaseTests
//     {
//         [Test]
//         public void CreateBook_RealDb_ShouldInsert()
//         {
//             TestContext.WriteLine("=== BẮT ĐẦU TEST REAL DB ===");
//
//             using (var context = new BookstoreDbContext())
//             {
//                 TestContext.WriteLine("Kết nối DB thành công!");
//                 
//
//                 var bookRepo = new BookRepository(context);
//                 var supplierRepo = new PublisherRepository(context);
//                 var stockRepo = new StockRepository(context);
//                 var importRepo = new ImportBillDetailRepository(context);
//                 var service = new BookService(bookRepo, stockRepo, supplierRepo, importRepo);
//
//                 var dto = new CreateBookRequestDto
//                 {
//                     Id = "B200",
//                     Name = "Real Test Book",
//                     Author = "Tester",
//                     Category = BookCategory.Children,
//                     PublisherId = "NXB001",
//                     SalePrice = 15000
//                 };
//
//                 TestContext.WriteLine("Gọi CreateBook...");
//
//                 var result = service.CreateBook(dto);
//
//                 // In log kết quả service
//                 Console.WriteLine($"Success: {result.IsSuccess} - Message: {result.ErrorMessage}");
//                 TestContext.WriteLine($"[SERVICE] Success: {result.IsSuccess} - Message: {result.ErrorMessage}");
//
//                 // Kiểm tra DB thật có record
//                 var inserted = context.Books.Find("B100");
//
//                 Console.WriteLine($"Inserted Name: {inserted?.Name}");
//                 TestContext.WriteLine($"[DB] Inserted Name: {inserted?.Name}");
//
//                 Assert.IsTrue(result.IsSuccess);
//                 Assert.IsNotNull(inserted);
//
//                 TestContext.WriteLine("=== TEST HOÀN THÀNH ===");
//             }
//         }
//         
//         [Test]
//         public void GetBookById_RealDb_ShouldReturnBook()
//         {
//             using (var context = new BookstoreDbContext())
//             {
//                 var bookRepo = new BookRepository(context);
//                 var supplierRepo = new PublisherRepository(context);
//                 var stockRepo = new StockRepository(context);
//                 var importRepo = new ImportBillDetailRepository(context);
//                 var service = new BookService(bookRepo, stockRepo, supplierRepo, importRepo);
//
//                 var id = "B100";
//
//                 TestContext.WriteLine($"[TEST] Lấy sách ID: {id}");
//
//                 var result = service.GetBookById(id);
//
//                 TestContext.WriteLine($"Success: {result.IsSuccess}");
//                 TestContext.WriteLine($"Book Name: {result.Data?.Name}");
//
//                 Assert.IsTrue(result.IsSuccess);
//                 Assert.IsNotNull(result.Data);
//             }
//         }
//
//         [Test]
//         public void UpdateBook_RealDb_ShouldUpdate()
//         {
//             using (var context = new BookstoreDbContext())
//             {
//                 var bookRepo = new BookRepository(context);
//                 var supplierRepo = new PublisherRepository(context);
//                 var stockRepo = new StockRepository(context);
//                 var importRepo = new ImportBillDetailRepository(context);
//                 var service = new BookService(bookRepo, stockRepo, supplierRepo, importRepo);
//         
//                 var dto = new UpdateBookRequestDto
//                 {
//                     Name = "Updated Book Name",
//                     Author = "New Author",
//                     SalePrice = 20000,
//                     Category = BookCategory.Children,
//                     PublisherId = "NXB001"
//                 };
//         
//                 TestContext.WriteLine("[TEST] UpdateBook gọi...");
//                 var result = service.UpdateBook("B100",dto);
//         
//                 TestContext.WriteLine($"Success: {result.IsSuccess}");
//         
//                 var updated = context.Books.Find("B100");
//         
//                 TestContext.WriteLine($"Updated Name: {updated?.Name}");
//         
//                 Assert.IsTrue(result.IsSuccess);
//                 Assert.AreEqual("Updated Book Name", updated.Name);
//             }
//         }
//
//         [Test]
//         public void DeleteBook_RealDb_ShouldDelete()
//         {
//             using (var context = new BookstoreDbContext())
//             {
//                 var bookRepo = new BookRepository(context);
//                 var supplierRepo = new PublisherRepository(context);
//                 var stockRepo = new StockRepository(context);
//                 var importRepo = new ImportBillDetailRepository(context);
//                 var service = new BookService(bookRepo, stockRepo, supplierRepo, importRepo);
//
//                 TestContext.WriteLine("[TEST] Xóa sách S00001...");
//                 var result = service.DeleteBook("S00003");
//
//                 TestContext.WriteLine($"Success: {result.IsSuccess}");
//
//                 var deleted = context.Books.Find("S00001");
//
//                 Assert.IsTrue(result.IsSuccess);
//             }
//         }
//
//         [Test]
//         public void GetAllBooks_RealDb_ShouldReturnList()
//         {
//             using (var context = new BookstoreDbContext())
//             {
//                 var bookRepo = new BookRepository(context);
//                 var supplierRepo = new PublisherRepository(context);
//                 var stockRepo = new StockRepository(context);
//                 var importRepo = new ImportBillDetailRepository(context);
//                 var service = new BookService(bookRepo, stockRepo, supplierRepo, importRepo);
//
//                 TestContext.WriteLine("[TEST] Lấy danh sách tất cả sách...");
//
//                 var result = service.GetAllBooks();
//                 
//                 TestContext.WriteLine($"Count: {result.Data.Count()}");
//
//                 Assert.IsTrue(result.IsSuccess);
//                 Assert.Greater(result.Data.Count(), 0);
//             }
//         }
//
//         [Test]
//         public void CheckStock_RealDb_ShouldReturnCorrectQuantity()
//         {
//             using (var context = new BookstoreDbContext())
//             {
//                 var bookRepo = new BookRepository(context);
//                 var stockRepo = new StockRepository(context); // cần khởi tạo
//                 var supplierRepo = new PublisherRepository(context);
//                 var importBillDetailRepo = new ImportBillDetailRepository(context);
//
//                 var service = new BookService(bookRepo, stockRepo, supplierRepo, importBillDetailRepo);
//
//                 var result = service.GetLowStockBooks(30);
//
//                 foreach (var i in result.Data)
//                 {
//                     TestContext.WriteLine($"{i.BookId}, {i.Name}, {i.Author}, {i.Category}, {i.SalePrice}, {i.PublisherName}");
//                 }
//
//                 Assert.IsTrue(result.IsSuccess);
//             }
//
//
//         }
//
//     }
// }
