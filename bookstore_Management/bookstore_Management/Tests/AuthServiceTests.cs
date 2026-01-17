// using NUnit.Framework;
// using bookstore_Management.Services.Interfaces;
// using bookstore_Management.DTOs.Auth;
// using System;
//
// namespace bookstore_Management.Tests
// {
//     [TestFixture]
//     public class AuthServiceTests
//     {
//         [SetUp]
//         public void Setup()
//         {
//             Console.WriteLine("[AuthServiceTest] Đã kết nối database");
//         }
//
//         [Test]
//         public void Login_Should_ReturnSuccess_When_ValidCredentials()
//         {
//             var service = TestHelper.GetAuthService();
//             var dto = new LoginRequestDto { Username = "admin", Password = "Admin@123" };
//             var result = service.Login(dto);
//             Assert.IsTrue(result.IsSuccess);
//             Console.WriteLine("Đăng nhập thành công với tài khoản admin.");
//         }
//
//         [Test]
//         public void Login_Should_Fail_When_WrongPassword()
//         {
//             var service = TestHelper.GetAuthService();
//             var dto = new LoginRequestDto { Username = "admin", Password = "wrong" };
//             var result = service.Login(dto);
//             Assert.IsFalse(result.IsSuccess);
//             Console.WriteLine("Đăng nhập thất bại với sai password.");
//         }
//
//         [Test]
//         public void Login_Should_Fail_When_WrongUsername()
//         {
//             var service = TestHelper.GetAuthService();
//             var dto = new LoginRequestDto { Username = "notexist", Password = "Admin@123" };
//             var result = service.Login(dto);
//             Assert.IsFalse(result.IsSuccess);
//             Console.WriteLine("Đăng nhập thất bại với username không tồn tại.");
//         }
//     }
// }
