// using NUnit.Framework;
// using bookstore_Management.Services.Interfaces;
// using bookstore_Management.DTOs.Order.Requests;
// using System;
//
// namespace bookstore_Management.Tests
// {
//     [TestFixture]
//     public class OrderServiceTests
//     {
//         [SetUp]
//         public void Setup()
//         {
//             Console.WriteLine("[OrderServiceTest] Đã kết nối database");
//         }
//
//         [Test]
//         public void CreateOrder_Should_Success_With_ValidData()
//         {
//             var service = TestHelper.GetOrderService();
//             var dto = new CreateOrderRequestDto { /*...*/ };
//             var result = service.CreateOrder(dto);
//             Assert.That(result.IsSuccess, Is.True);
//             Console.WriteLine("Tạo đơn hàng thành công");
//         }
//
//         [Test]
//         public void CreateOrder_Should_Fail_With_InvalidCustomer()
//         {
//             var service = TestHelper.GetOrderService();
//             var dto = new CreateOrderRequestDto { CustomerId = "NOTEXIST", /*...*/ };
//             var result = service.CreateOrder(dto);
//             Assert.That(result.IsSuccess, Is.False);
//             Console.WriteLine("Tạo đơn thất bại vì CustomerId không tồn tại");
//         }
//
//         [Test]
//         public void GetOrderById_Should_Success_With_ValidId()
//         {
//             var service = TestHelper.GetOrderService();
//             var result = service.GetOrderById("ORD001");
//             Assert.That(result.IsSuccess, Is.True);
//             Console.WriteLine($"Lấy chi tiết đơn hàng thành công");
//         }
//
//         [Test]
//         public void GetOrderById_Should_Fail_With_InvalidId()
//         {
//             var service = TestHelper.GetOrderService();
//             var result = service.GetOrderById("fakeid");
//             Assert.That(result.IsSuccess, Is.False);
//             Console.WriteLine($"Lấy chi tiết đơn hàng thất bại với id không đúng");
//         }
//     }
// }
