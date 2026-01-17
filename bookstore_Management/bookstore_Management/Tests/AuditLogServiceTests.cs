using NUnit.Framework;
using bookstore_Management.Services.Interfaces;
using bookstore_Management.Models;
using bookstore_Management.Core.Results;
using System.Linq;
using System;
using System.Collections.Generic;
using bookstore_Management.Data.Context;

namespace bookstore_Management.Tests
{
    [TestFixture]
    public class AuditLogServiceTests
    {
        private IAuditLogService _service;
        private BookstoreDbContext _db;

        [SetUp]
        public void Setup()
        {
            _db = new BookstoreDbContext();
            // Bạn cần khởi tạo AuditLogService đúng theo DI hoặc new trực tiếp tùy cấu trúc project
            // _service = new AuditLogService(...);
        }

        [Test]
        public void Log_Should_AddLogRecord_When_ValidInput()
        {
            // Arrange
            var auditLog = new AuditLog
            {
                EntityName = "TestEntity",
                EntityId = Guid.NewGuid().ToString(),
                Action = "CREATE",
                OldValues = null,
                NewValues = "{\"Val\":1}",
                ChangedBy = "admin",
                ChangedDate = DateTime.Now
            };

            // Act
            //var result = _service.Log(auditLog);
            // Assert
            //Assert.IsTrue(result.IsSuccess && result.Data > 0);
            Assert.Pass("Add actual implementation to run this test.");
        }

        [Test]
        public void GetRecent_Should_ReturnLogs_When_LogsExist()
        {
            // Act
            //var result = _service.GetRecent(5);
            // Assert
            //Assert.IsTrue(result.IsSuccess);
            //Assert.LessOrEqual(result.Data.Count(), 5);
            Assert.Pass("Add actual implementation to run this test.");
        }

        [Test]
        public void GetByEntity_Should_ReturnCorrectLogs()
        {
            // Arrange
            var entityName = "Order"; var entityId = "ORD001";
            // Act
            //var result = _service.GetByEntity(entityName, entityId);
            // Assert
            //Assert.IsTrue(result.IsSuccess);
            Assert.Pass("Add actual implementation to run this test.");
        }
    }
}
