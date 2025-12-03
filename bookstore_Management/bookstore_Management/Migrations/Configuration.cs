using System.Collections.Generic;
using bookstore_Management.Core.Enums;
using bookstore_Management.Models;

namespace bookstore_Management.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<bookstore_Management.Data.Context.BookstoreDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        // khởi tạo giá trị mẫu cho database
        protected override void Seed(bookstore_Management.Data.Context.BookstoreDbContext context)
        {
            Console.WriteLine("Insert supplier");
            // 1. Suppliers
            var suppliers = new List<Supplier>
            {
                new Supplier { Id = "NXB001", Name = "NXB Kim Đồng", Phone = "02438543543", Address = "55 Quang Trung, Hà Nội", Email = "contact@nxbkimdong.vn" },
                new Supplier { Id = "NXB002", Name = "NXB Trẻ", Phone = "02839316289", Address = "161B Lý Chính Thắng, Q.3, TP.HCM", Email = "info@nxbtre.com.vn" },
                new Supplier { Id = "NXB003", Name = "NXB Phụ Nữ", Phone = "02438225391", Address = "39 Hàng Chuối, Hà Nội", Email = "nxbphunu@hn.vnn.vn" }
            };
            context.Suppliers.AddOrUpdate(s => s.Id, suppliers.ToArray());
            context.SaveChanges();
            Console.WriteLine("Finish");

            Console.WriteLine("Insert Book");
            // 2. Books
            var books = new List<Book>
            {
                new Book { BookId = "S00001",Name = "Đắc Nhân Tâm", SupplierId = "NXB001", Category = BookCategory.Psychology, SalePrice = 80000, ImportPrice = 70000 },
                new Book { BookId = "S00002",Name = "Nhà Giả Kim", SupplierId = "NXB001", Category = BookCategory.Novel, SalePrice = 75000, ImportPrice = 65000 },
                new Book { BookId = "S00003",Name = "Tuổi Trẻ Đáng Giá Bao Nhiêu", SupplierId = "NXB002", Category = BookCategory.Literature, ImportPrice = 6000 },
                new Book { BookId = "S00004",Name = "Tôi Tài Giỏi Bạn Cũng Thế", SupplierId = "NXB002", Category = BookCategory.Economics, ImportPrice = 5000 },
                new Book { BookId = "S00005",Name = "Bố Già", SupplierId = "NXB003", Category = BookCategory.Literature, ImportPrice = 85000 }
            };
            context.Books.AddOrUpdate(b => b.BookId, books.ToArray());
            context.SaveChanges();
            Console.WriteLine("Finish");

            Console.WriteLine("Insert Staff");
            // 3. Staff
            var staffs = new List<Staff>
            {
                new Staff { Id = "NV0001",Name = "Nguyễn Văn A", BaseSalary = 8000000, CitizenIdCard = "0012345678", Phone = "0912345678", Address = "Hà Nội", Status = (StaffStatus)1, Role = (Role)2, SalaryRate = 2 },
                new Staff { Id = "NV0002",Name = "Trần Thị B", BaseSalary = 7500000, CitizenIdCard = "0012345671", Phone = "0923456789", Address = "Hà Nội", Status = (StaffStatus)3, Role = (Role)5, SalaryRate = 3 },
                new Staff { Id = "NV0003",Name = "Phạm Quang C", BaseSalary = 6000000, CitizenIdCard = "0111198765", Phone = "0923456789", Address = "TPHCM", Status = (StaffStatus)2, Role = (Role)3 ,SalaryRate = 1 }

            };
            context.Staff.AddOrUpdate(s => s.Id, staffs.ToArray());
            context.SaveChanges();
            Console.WriteLine("Finish");


            // 4. Staff Daily Revenue

            Console.WriteLine("Insert Customer");
            // 5. Customers
            var customers = new List<Customer>
            {
                new Customer { CustomerId = "KH0001", Name = "Lê Văn C", Phone = "0934567890", Email = "levanc@gmail.com", Address = "Hà Nội" },
                new Customer { CustomerId = "KH0002", Name = "Phạm Thị D", Phone = "0945678901", Email = "phamthid@gmail.com", Address = "Hà Nội" }
            };
            context.Customers.AddOrUpdate(c => c.CustomerId, customers.ToArray());
            context.SaveChanges();
            Console.WriteLine("Finish");

            
        }
    }
}
