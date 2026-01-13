//using bookstore_Management.Data.Context;
//using bookstore_Management.Data.Repositories.Implementations;
//using bookstore_Management.DTOs;
//using bookstore_Management.Services.Implementations;
//using NUnit.Framework;

//namespace bookstore_Management.Tests
//{
//    [TestFixture]
//    public class AuthServiceTests
//    {
//        private BookstoreDbContext _context;
//        private AuthService _service;

//        [SetUp]
//        public void SetUp()
//        {
//            _context = new BookstoreDbContext();
//            var userRepo = new UserRepository(_context);
//            _service = new AuthService(userRepo);
//        }

//        [Test]
//        public void Login_ShouldSucceed_WithSeedAdmin()
//        {
//            var result = _service.Login(new LoginDto
//            {
//                Username = "admin",
//                Password = "Admin@123"
//            });

//            Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual("NV0001", result.Data.UserId);
//        }

//        [Test]
//        public void Login_ShouldFail_WithWrongPassword()
//        {
//            var result = _service.Login(new LoginDto
//            {
//                Username = "admin",
//                Password = "wrongpass"
//            });

//            Assert.IsFalse(result.IsSuccess);
//        }

//        [Test]
//        public void ChangePassword_ShouldFail_WhenOldPasswordIncorrect()
//        {
//            var result = _service.ChangePassword("NV0001", new ChangePasswordDto
//            {
//                OldPassword = "wrong",
//                NewPassword = "NewPass@123",
//                ConfirmPassword = "NewPass@123"
//            });

//            Assert.IsFalse(result.IsSuccess);
//        }

//        [Test]
//        public void ChangePassword_ShouldSucceed_AndRevert()
//        {
//            var tempPwd = "Temp@1234";

//            var change = _service.ChangePassword("NV0001", new ChangePasswordDto
//            {
//                OldPassword = "Admin@123",
//                NewPassword = tempPwd,
//                ConfirmPassword = tempPwd
//            });
//            Assert.IsTrue(change.IsSuccess, change.ErrorMessage);

//            // Đăng nhập với mật khẩu mới
//            var loginNew = _service.Login(new LoginDto { Username = "admin", Password = tempPwd });
//            Assert.IsTrue(loginNew.IsSuccess, loginNew.ErrorMessage);

//            // Đổi lại mật khẩu cũ để không phá dữ liệu seed
//            var revert = _service.ChangePassword("NV0001", new ChangePasswordDto
//            {
//                OldPassword = tempPwd,
//                NewPassword = "Admin@123",
//                ConfirmPassword = "Admin@123"
//            });
//            Assert.IsTrue(revert.IsSuccess, revert.ErrorMessage);
//        }
//    }
//}

