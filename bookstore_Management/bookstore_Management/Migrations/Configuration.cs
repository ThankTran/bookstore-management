using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using bookstore_Management.Core.Enums;
using bookstore_Management.Models;
using bookstore_Management.Utils;

namespace bookstore_Management.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<bookstore_Management.Data.Context.BookstoreDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            // Tuyệt đối tránh data loss khi update schema.
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(bookstore_Management.Data.Context.BookstoreDbContext context)
        {
            // ============================================
            // Publishers
            // ============================================
            var publishers = new List<Publisher>
            {
                new Publisher { Id = "NXB001", Name = "NXB Kim Đồng", Phone = "0243854354", CreatedDate = DateTime.Now },
                new Publisher { Id = "NXB002", Name = "NXB Trẻ", Phone = "0283931628", CreatedDate = DateTime.Now },
                new Publisher { Id = "NXB003", Name = "NXB Phụ Nữ", Phone = "0243822539", CreatedDate = DateTime.Now }
            };
            context.Publishers.AddOrUpdate(s => s.Id, publishers.ToArray());
            context.SaveChanges();
            
            // ============================================
            // Books
            // ============================================
            var books = new List<Book>
            {
                new Book { BookId = "S00001", Name = "Đắc Nhân Tâm", Author = "Dale Carnegie", PublisherId = "NXB001", Category = BookCategory.Psychology, SalePrice = 80000, Stock = 30, CreatedDate = DateTime.Now },
                new Book { BookId = "S00002", Name = "Nhà Giả Kim", Author = "Paulo Coelho", PublisherId = "NXB001", Category = BookCategory.Novel, SalePrice = 75000, Stock = 50, CreatedDate = DateTime.Now },
                new Book { BookId = "S00003", Name = "Tuổi Trẻ Đáng Giá Bao Nhiêu", Author = "Rosie Nguyễn", PublisherId = "NXB002", Category = BookCategory.Literature, SalePrice = 50000, Stock = 10 , CreatedDate = DateTime.Now },
                new Book { BookId = "S00004", Name = "Tôi Tài Giỏi Bạn Cũng Thế", Author = "Adam Khoo", PublisherId = "NXB002", Category = BookCategory.Economics, SalePrice = 65000, Stock = 35, CreatedDate = DateTime.Now },
                new Book { BookId = "S00005", Name = "Bố Già", Author = "Mario Puzo", PublisherId = "NXB003", Category = BookCategory.Literature, SalePrice = 95000, Stock = 100 , CreatedDate = DateTime.Now }
            };
            context.Books.AddOrUpdate(b => b.BookId, books.ToArray());
            context.SaveChanges();
            

            // ============================================
            // Staff
            // ============================================
            var staffs = new List<Staff>
            {
                new Staff { Id = "NV0001", Name = "Nguyễn Văn A", Phone = "123456789", CitizenId = "123456789",UserRole = UserRole.SalesManager, CreatedDate = DateTime.Now },
                new Staff { Id = "NV0002", Name = "Trần Thị B", Phone = "222222222", CitizenId = "111111111", UserRole = UserRole.CustomerManager, CreatedDate = DateTime.Now },
                new Staff { Id = "NV0003", Name = "Phạm Quang C", Phone = "33333333", CitizenId = "4444444444", UserRole = UserRole.SalesStaff, CreatedDate = DateTime.Now }
            };
            context.Staff.AddOrUpdate(s => s.Id, staffs.ToArray());
            context.SaveChanges();

            // ============================================
            // Customers
            // ============================================
            var customers = new List<Customer>
            {
                new Customer { CustomerId = "KH0001", Name = "Lê Văn C", Phone = "0934567890", Email = "abc@gmail.com", Address = "Quận 7",MemberLevel = MemberTier.Bronze, LoyaltyPoints = 0, CreatedDate = DateTime.Now },
                new Customer { CustomerId = "KH0002", Name = "Phạm Thị D", Phone = "0945678901" , Email = "def@gmail.com", Address = "Quận 8", MemberLevel = MemberTier.Silver, LoyaltyPoints = 500, CreatedDate = DateTime.Now }
            };
            context.Customers.AddOrUpdate(c => c.CustomerId, customers.ToArray());
            context.SaveChanges();

            // ============================================
            // Import Bills
            // ============================================
            var importBills = new List<ImportBill>
            {
                new ImportBill { Id = "PN0001", PublisherId = "NXB001",  TotalAmount = 3_500_000, Notes = "Nhập sách tâm lý", CreatedBy = "NV0001", CreatedDate = DateTime.Now.AddDays(-30) },
                new ImportBill { Id = "PN0002", PublisherId = "NXB002", TotalAmount = 4_000_000, Notes = "Nhập sách giáo khoa", CreatedBy = "NV0002", CreatedDate = DateTime.Now.AddDays(-15) }
            };
            context.ImportBills.AddOrUpdate(ib => ib.Id, importBills.ToArray());
            context.SaveChanges();

            // ============================================
            // Import Bill Details
            // ============================================
            var importBillDetails = new List<ImportBillDetail>
            {
                new ImportBillDetail { BookId = "S00001", ImportId = "PN0001", Quantity = 50, ImportPrice = 70000 },
                new ImportBillDetail { BookId = "S00002", ImportId = "PN0001", Quantity = 30, ImportPrice = 65000 },
                new ImportBillDetail { BookId = "S00003", ImportId = "PN0002", Quantity = 60, ImportPrice = 40000 },
                new ImportBillDetail { BookId = "S00004", ImportId = "PN0002", Quantity = 40, ImportPrice = 55000 }
            };
            context.ImportBillDetails.AddOrUpdate(
                ibd => new { ibd.BookId, ibd.ImportId },
                importBillDetails.ToArray());
            context.SaveChanges();

            // ============================================
            // Orders
            // ============================================
            var orders = new List<Order>
            {
                new Order { OrderId = "ORD001", StaffId = "NV0001", CustomerId = "KH0001", PaymentMethod = PaymentType.Cash, Discount = 0, TotalPrice = 155000, CreatedDate = DateTime.Now.AddDays(-5) },
                new Order { OrderId = "ORD002", StaffId = "NV0003", CustomerId = "KH0002", PaymentMethod = PaymentType.Card, Discount = 10000, TotalPrice = 210000, CreatedDate = DateTime.Now.AddDays(-3) }
            };
            context.Orders.AddOrUpdate(o => o.OrderId, orders.ToArray());
            context.SaveChanges();

            // ============================================
            // Order Details
            // ============================================
            var orderDetails = new List<OrderDetail>
            {
                new OrderDetail { OrderId = "ORD001", BookId = "S00001", SalePrice = 80000, Quantity = 1, Subtotal = 80000 },
                new OrderDetail { OrderId = "ORD001", BookId = "S00003", SalePrice = 50000, Quantity = 1, Subtotal = 50000 },
                new OrderDetail { OrderId = "ORD002", BookId = "S00002", SalePrice = 75000, Quantity = 2, Subtotal = 150000 }
            };
            context.OrderDetails.AddOrUpdate(
                od => new { od.OrderId, od.BookId },
                orderDetails.ToArray());
            context.SaveChanges();

            // ============================================
            // Users (gắn Staff)
            // ============================================
            var users = new List<User>
            {
                new User { Username = "admin", PasswordHash = Encryptor.Hash("Admin@123"), StaffId = "NV0001", CreatedDate = DateTime.Now },
                new User { Username = "manager", PasswordHash = Encryptor.Hash("Manager@123"), StaffId = "NV0002", CreatedDate = DateTime.Now },
                new User { Username = "staff", PasswordHash = Encryptor.Hash("Staff@123"), StaffId = "NV0003", CreatedDate = DateTime.Now }
            };
            context.Users.AddOrUpdate(u => u.Username, users.ToArray());
            context.SaveChanges();
            

            // AuditLogs mẫu 
            // context.AuditLogs.AddOrUpdate(al => al.Id, new AuditLog { ... });
            // context.SaveChanges();

            Console.WriteLine("\n✅ All seed data inserted successfully!");
            base.Seed(context);
        }
    }
}