// using NUnit.Framework;
// using bookstore_Management.Services.Interfaces;
// using bookstore_Management.DTOs.Book.Requests;
// using System;
//
// namespace bookstore_Management.Tests
// {
//     [TestFixture]
//     public class BookServiceTests
//     {
//         [SetUp]
//         public void Setup()
//         {
//             Console.WriteLine("[BookServiceTest] Đã kết nối database");
//         }
//
//         [Test]
//         public void CreateBook_Should_Success_With_ValidData()
//         {
//             var service = TestHelper.GetBookService();
//             var book = new CreateBookRequestDto { Name = "Sách test", Author = "Tác giả test", Category = bookstore_Management.Core.Enums.BookCategory.Literature };
//             var result = service.CreateBook(book);
//             Assert.IsTrue(result.IsSuccess);
//             Console.WriteLine("Thêm sách mới thành công");
//         }
//
//         [Test]
//         public void CreateBook_Should_Fail_With_InvalidData()
//         {
//             var service = TestHelper.GetBookService();
//             var book = new CreateBookRequestDto { Name = "", Author = "", Category = bookstore_Management.Core.Enums.BookCategory.Literature };
//             var result = service.CreateBook(book);
//             Assert.IsFalse(result.IsSuccess);
//             Console.WriteLine("Thêm sách mới thất bại với dữ liệu thiếu.");
//         }
//
//         [Test]
//         public void GetBookById_Should_Return_Success_With_ExistingId()
//         {
//             var service = TestHelper.GetBookService();
//             var result = service.GetBookById("S00001");
//             Assert.IsTrue(result.IsSuccess);
//             Console.WriteLine("Lấy chi tiết sách thành công");
//         }
//
//         [Test]
//         public void GetBookById_Should_Fail_With_InvalidId()
//         {
//             var service = TestHelper.GetBookService();
//             var result = service.GetBookById("ID_Null");
//             Assert.IsFalse(result.IsSuccess);
//             Console.WriteLine("Lấy chi tiết sách thất bại do ID không tồn tại.");
//         }
//
//         [Test]
//         public void SearchByName_Should_ReturnMatch_For_Keyword()
//         {
//             var service = TestHelper.GetBookService();
//             var result = service.SearchByName("nhan tam");
//             Assert.IsTrue(result.IsSuccess);
//             Console.WriteLine($"Số lượng sách tìm thấy: {result.Data.Count()}");
//         }
//     }
// }
