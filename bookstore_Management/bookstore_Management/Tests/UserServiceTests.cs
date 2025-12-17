using System;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.implementations;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.DTOs;
using bookstore_Management.Services.Implementations;
using NUnit.Framework;

namespace bookstore_Management.Tests
{
    [TestFixture]
    public class UserServiceTests
    {
        private BookstoreDbContext _context;
        private UserService _service;

        [SetUp]
        public void SetUp()
        {
            _context = new BookstoreDbContext();
            var userRepo = new UserRepository(_context);
            var staffRepo = new StaffRepository(_context);
            _service = new UserService(userRepo, staffRepo);
        }

        [Test]
        public void CreateUser_ShouldFail_WhenMissingStaff()
        {
            var result = _service.CreateUser(new CreateUserDto
            {
                Username = "newuser",
                Password = "Password@123",
                StaffId = "NV9999"
            });
            Assert.IsFalse(result.IsSuccess);
        }

        [Test]
        public void CreateUser_ShouldSucceed_WithValidStaff()
        {
            var result = _service.CreateUser(new CreateUserDto
            {
                Username = "tester",
                Password = "Password@123",
                StaffId = "NV0001"
            });
            Assert.IsTrue(result.IsSuccess);
        }
    }
}

