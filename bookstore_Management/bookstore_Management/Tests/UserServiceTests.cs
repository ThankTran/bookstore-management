// using NUnit.Framework;
// using bookstore_Management.Services.Interfaces;
// using bookstore_Management.DTOs.User.Requests;
// using System;
//
// namespace bookstore_Management.Tests
// {
//     [TestFixture]
//     public class UserServiceTests
//     {
//         [SetUp]
//         public void Setup()
//         {
//             Console.WriteLine("[UserServiceTest] Đã kết nối database");
//         }
//
//         [Test]
//         public void CreateUser_Should_Success_With_UniqueUsername()
//         {
//             var service = TestHelper.GetUserService();
//             var dto = new CreateUserRequestDto { Username = "testuser" + System.Guid.NewGuid(), Password = "123456Aa@", /*...*/ };
//             var result = service.CreateUser(dto);
//             Assert.IsTrue(result.IsSuccess);
//             Console.WriteLine("Tạo tài khoản mới thành công");
//         }
//
//         [Test]
//         public void CreateUser_Should_Fail_With_DuplicateUsername()
//         {
//             var service = TestHelper.GetUserService();
//             var dto = new CreateUserRequestDto { Username = "admin", Password = "123456Aa@" };
//             var result = service.CreateUser(dto);
//             Assert.IsFalse(result.IsSuccess);
//             Console.WriteLine("Tạo tài khoản thất bại do trùng username.");
//         }
//
//         [Test]
//         public void GetUserById_Should_Success_When_ValidId()
//         {
//             var service = TestHelper.GetUserService();
//             var result = service.GetById("admin");
//             Assert.IsTrue(result.IsSuccess);
//             Console.WriteLine("Lấy thông tin user thành công");
//         }
//
//         [Test]
//         public void GetUserById_Should_Fail_When_InvalidId()
//         {
//             var service = TestHelper.GetUserService();
//             var result = service.GetById("notfound");
//             Assert.IsFalse(result.IsSuccess);
//             Console.WriteLine("Lấy thông tin user thất bại với id không tồn tại");
//         }
//     }
// }
