using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using bookstore_Management.Core.Enums;
using bookstore_Management.Models;

namespace bookstore_Management.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<bookstore_Management.Data.Context.BookstoreDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(bookstore_Management.Data.Context.BookstoreDbContext context)
        {
            // ============================================
            // 1. SUPPLIERS (Nhà cung cấp)
            // ============================================
            Console.WriteLine("Inserting Suppliers...");
            var suppliers = new List<Supplier>
            {
                new Supplier 
                { 
                    Id = "NXB001", 
                    Name = "NXB Kim Đồng", 
                    Phone = "0243854354", 
                    Address = "55 Quang Trung, Hà Nội", 
                    Email = "contact@nxbkimdong.vn",
                    CreatedDate = DateTime.Now
                },
                new Supplier 
                { 
                    Id = "NXB002", 
                    Name = "NXB Trẻ", 
                    Phone = "0283931628", 
                    Address = "161B Lý Chính Thắng, Q.3, TP.HCM", 
                    Email = "info@nxbtre.com.vn",
                    CreatedDate = DateTime.Now
                },
                new Supplier 
                { 
                    Id = "NXB003", 
                    Name = "NXB Phụ Nữ", 
                    Phone = "0243822539", 
                    Address = "39 Hàng Chuối, Hà Nội", 
                    Email = "nxbphunu@hn.vnn.vn",
                    CreatedDate = DateTime.Now
                }
            };
            context.Suppliers.AddOrUpdate(s => s.Id, suppliers.ToArray());
            context.SaveChanges();
            Console.WriteLine("✓ Suppliers inserted");

            // ============================================
            // 2. BOOKS (Sách)
            // ============================================
            Console.WriteLine("Inserting Books...");
            var books = new List<Book>
            {
                new Book 
                { 
                    BookId = "S00001", 
                    Name = "Đắc Nhân Tâm", 
                    SupplierId = "NXB001", 
                    Category = BookCategory.Psychology, 
                    SalePrice = 80000, 
                    ImportPrice = 70000,
                    CreatedDate = DateTime.Now
                },
                new Book 
                { 
                    BookId = "S00002", 
                    Name = "Nhà Giả Kim", 
                    SupplierId = "NXB001", 
                    Category = BookCategory.Novel, 
                    SalePrice = 75000, 
                    ImportPrice = 65000,
                    CreatedDate = DateTime.Now
                },
                new Book 
                { 
                    BookId = "S00003", 
                    Name = "Tuổi Trẻ Đáng Giá Bao Nhiêu", 
                    SupplierId = "NXB002", 
                    Category = BookCategory.Literature, 
                    SalePrice = 50000,
                    ImportPrice = 40000,
                    CreatedDate = DateTime.Now
                },
                new Book 
                { 
                    BookId = "S00004", 
                    Name = "Tôi Tài Giỏi Bạn Cũng Thế", 
                    SupplierId = "NXB002", 
                    Category = BookCategory.Economics, 
                    SalePrice = 65000,
                    ImportPrice = 55000,
                    CreatedDate = DateTime.Now
                },
                new Book 
                { 
                    BookId = "S00005", 
                    Name = "Bố Già", 
                    SupplierId = "NXB003", 
                    Category = BookCategory.Literature, 
                    SalePrice = 95000,
                    ImportPrice = 85000,
                    CreatedDate = DateTime.Now
                },
                new Book 
                { 
                    BookId = "S00006", 
                    Name = "Harry Potter and the Sorcerer's Stone", 
                    SupplierId = "NXB001", 
                    Category = BookCategory.Novel, 
                    SalePrice = 120000,
                    ImportPrice = 100000,
                    CreatedDate = DateTime.Now
                },
                new Book 
                { 
                    BookId = "S00007", 
                    Name = "Lập Trình C# Cơ Bản", 
                    SupplierId = "NXB002", 
                    Category = BookCategory.Textbook, 
                    SalePrice = 150000,
                    ImportPrice = 120000,
                    CreatedDate = DateTime.Now
                },
                new Book 
                { 
                    BookId = "S00008", 
                    Name = "Steve Jobs - Cuộc Đời Một Thiên Tài", 
                    SupplierId = "NXB003", 
                    Category = BookCategory.Biography, 
                    SalePrice = 110000,
                    ImportPrice = 90000,
                    CreatedDate = DateTime.Now
                }
            };
            context.Books.AddOrUpdate(b => b.BookId, books.ToArray());
            context.SaveChanges();
            Console.WriteLine("✓ Books inserted");

            // ============================================
            // 3. STOCK (Tồn kho)
            // ============================================
            Console.WriteLine("Inserting Stocks...");
            var stocks = new List<Stock>
            {
                new Stock { BookId = "S00001", StockQuantity = 50 },
                new Stock { BookId = "S00002", StockQuantity = 30 },
                new Stock { BookId = "S00003", StockQuantity = 100 },
                new Stock { BookId = "S00004", StockQuantity = 75 },
                new Stock { BookId = "S00005", StockQuantity = 20 },
                new Stock { BookId = "S00006", StockQuantity = 15 },
                new Stock { BookId = "S00007", StockQuantity = 40 },
                new Stock { BookId = "S00008", StockQuantity = 25 }
            };
            context.Stocks.AddOrUpdate(s => s.BookId, stocks.ToArray());
            context.SaveChanges();
            Console.WriteLine("✓ Stocks inserted");

            // ============================================
            // 4. STAFF (Nhân viên)
            // ============================================
            Console.WriteLine("Inserting Staff...");
            var staffs = new List<Staff>
            {
                new Staff 
                { 
                    Id = "NV0001", 
                    Name = "Nguyễn Văn A", 
                    BaseSalary = 8000000, 
                    CitizenIdCard = "001234567", 
                    Phone = "0912345678", 
                    Status = StaffStatus.Working, 
                    Role = Role.SalesManager, 
                    SalaryRate = 1.2m,
                    CreatedDate = DateTime.Now
                },
                new Staff 
                { 
                    Id = "NV0002", 
                    Name = "Trần Thị B", 
                    BaseSalary = 7500000, 
                    CitizenIdCard = "001234567", 
                    Phone = "0923456789", 
                    Status = StaffStatus.Rest, 
                    Role = Role.CustomerManager, 
                    SalaryRate = 1.0m,
                    CreatedDate = DateTime.Now
                },
                new Staff 
                { 
                    Id = "NV0003", 
                    Name = "Phạm Quang C", 
                    BaseSalary = 6000000, 
                    CitizenIdCard = "011119876", 
                    Phone = "0923456789", 
                    Status = StaffStatus.Working, 
                    Role = Role.SalesStaff, 
                    SalaryRate = 0.8m,
                    CreatedDate = DateTime.Now
                },
                new Staff 
                { 
                    Id = "NV0004", 
                    Name = "Lê Hoàng D", 
                    BaseSalary = 6500000, 
                    CitizenIdCard = "011119877", 
                    Phone = "0934567890", 
                    Status = StaffStatus.Working, 
                    Role = Role.InventoryManager, 
                    SalaryRate = 1.1m,
                    CreatedDate = DateTime.Now
                },
                new Staff 
                { 
                    Id = "NV0005", 
                    Name = "Võ Thị E", 
                    BaseSalary = 7000000, 
                    CitizenIdCard = "011119878", 
                    Phone = "0945678901", 
                    Status = StaffStatus.Working, 
                    Role = Role.SalesStaff, 
                    SalaryRate = 0.9m,
                    CreatedDate = DateTime.Now
                }
            };
            context.Staff.AddOrUpdate(s => s.Id, staffs.ToArray());
            context.SaveChanges();
            Console.WriteLine("✓ Staff inserted");

            // ============================================
            // 5. CUSTOMERS (Khách hàng)
            // ============================================
            Console.WriteLine("Inserting Customers...");
            var customers = new List<Customer>
            {
                new Customer 
                { 
                    CustomerId = "KH0001", 
                    Name = "Lê Văn C", 
                    Phone = "0934567890", 
                    Email = "levanc@gmail.com", 
                    Address = "123 Nguyễn Trãi, Hà Nội",
                    MemberLevel = MemberTier.Bronze,
                    LoyaltyPoints = 0,
                    CreatedDate = DateTime.Now
                },
                new Customer 
                { 
                    CustomerId = "KH0002", 
                    Name = "Phạm Thị D", 
                    Phone = "0945678901", 
                    Email = "phamthid@gmail.com", 
                    Address = "456 Lê Lợi, TP.HCM",
                    MemberLevel = MemberTier.Silver,
                    LoyaltyPoints = 500,
                    CreatedDate = DateTime.Now
                },
                new Customer 
                { 
                    CustomerId = "KH0003", 
                    Name = "Vũ Minh E", 
                    Phone = "0956789012", 
                    Email = "vuminh@gmail.com", 
                    Address = "789 Trần Hưng Đạo, Đà Nẵng",
                    MemberLevel = MemberTier.Gold,
                    LoyaltyPoints = 1500,
                    CreatedDate = DateTime.Now
                },
                new Customer 
                { 
                    CustomerId = "KH0004", 
                    Name = "Hoàng Văn F", 
                    Phone = "0967890123", 
                    Email = "hoangvanf@gmail.com", 
                    Address = "321 Đinh Tiên Hoàng, Huế",
                    MemberLevel = MemberTier.Diamond,
                    LoyaltyPoints = 3000,
                    CreatedDate = DateTime.Now
                }
            };
            context.Customers.AddOrUpdate(c => c.CustomerId, customers.ToArray());
            context.SaveChanges();
            Console.WriteLine("✓ Customers inserted");

            // ============================================
            // 6. IMPORT BILLS (Hóa đơn nhập)
            // ============================================
            Console.WriteLine("Inserting ImportBills...");
            var importBills = new List<ImportBill>
            {
                new ImportBill 
                { 
                    ImportBillCode = "IMP001", 
                    ImportDate = DateTime.Now.AddDays(-30), 
                    SupplierId = "NXB001", 
                    TotalAmount = 3500000,
                    Notes = "Nhập sách tâm lý",
                    CreatedBy = "NV0004",
                    CreatedDate = DateTime.Now.AddDays(-30)
                },
                new ImportBill 
                { 
                    ImportBillCode = "IMP002", 
                    ImportDate = DateTime.Now.AddDays(-15), 
                    SupplierId = "NXB002", 
                    TotalAmount = 4000000,
                    Notes = "Nhập sách giáo khoa",
                    CreatedBy = "NV0004",
                    CreatedDate = DateTime.Now.AddDays(-15)
                },
                new ImportBill 
                { 
                    ImportBillCode = "IMP003", 
                    ImportDate = DateTime.Now.AddDays(-7), 
                    SupplierId = "NXB003", 
                    TotalAmount = 2250000,
                    Notes = "Nhập sách tiểu thuyết",
                    CreatedBy = "NV0004",
                    CreatedDate = DateTime.Now.AddDays(-7)
                }
            };
            context.ImportBills.AddOrUpdate(ib => ib.ImportBillCode, importBills.ToArray());
            context.SaveChanges();
            Console.WriteLine("✓ ImportBills inserted");

            // ============================================
            // 7. IMPORT BILL DETAILS (Chi tiết hóa đơn nhập)
            // ============================================
            Console.WriteLine("Inserting ImportBillDetails...");
            var importBillDetails = new List<ImportBillDetail>
            {
                new ImportBillDetail 
                { 
                    BookId = "S00001", 
                    ImportId = 1, 
                    Quantity = 50, 
                    ImportPrice = 70000,
                    TotalPrice = 3500000
                },
                new ImportBillDetail 
                { 
                    BookId = "S00003", 
                    ImportId = 2, 
                    Quantity = 100, 
                    ImportPrice = 40000,
                    TotalPrice = 4000000
                },
                new ImportBillDetail 
                { 
                    BookId = "S00005", 
                    ImportId = 3, 
                    Quantity = 25, 
                    ImportPrice = 85000,
                    TotalPrice = 2125000
                },
                new ImportBillDetail 
                { 
                    BookId = "S00006", 
                    ImportId = 3, 
                    Quantity = 2, 
                    ImportPrice = 100000,
                    TotalPrice = 200000
                }
            };
            context.ImportBillDetails.AddOrUpdate(
                ibd => new { ibd.BookId, ibd.ImportId }, 
                importBillDetails.ToArray());
            context.SaveChanges();
            Console.WriteLine("✓ ImportBillDetails inserted");

            // ============================================
            // 8. ORDERS (Hóa đơn bán)
            // ============================================
            Console.WriteLine("Inserting Orders...");
            var orders = new List<Order>
            {
                new Order 
                { 
                    OrderId = "ORD001", 
                    StaffId = "NV0001", 
                    CustomerId = "KH0001", 
                    PaymentMethod = PaymentType.Cash, 
                    Discount = 0,
                    TotalPrice = 155000,
                    Notes = "Khách hàng thường xuyên",
                    CreatedDate = DateTime.Now.AddDays(-5)
                },
                new Order 
                { 
                    OrderId = "ORD002", 
                    StaffId = "NV0003", 
                    CustomerId = "KH0002", 
                    PaymentMethod = PaymentType.Card, 
                    Discount = 10000,
                    TotalPrice = 210000,
                    Notes = "Mua quà tặng",
                    CreatedDate = DateTime.Now.AddDays(-3)
                },
                new Order 
                { 
                    OrderId = "ORD003", 
                    StaffId = "NV0001", 
                    CustomerId = null,  // Khách vãng lai
                    PaymentMethod = PaymentType.BankTransfer, 
                    Discount = 0,
                    TotalPrice = 120000,
                    CreatedDate = DateTime.Now.AddDays(-2)
                },
                new Order 
                { 
                    OrderId = "ORD004", 
                    StaffId = "NV0005", 
                    CustomerId = "KH0003", 
                    PaymentMethod = PaymentType.Card, 
                    Discount = 5000,
                    TotalPrice = 245000,
                    CreatedDate = DateTime.Now.AddDays(-1)
                },
                new Order 
                { 
                    OrderId = "ORD005", 
                    StaffId = "NV0003", 
                    CustomerId = "KH0004", 
                    PaymentMethod = PaymentType.Cash, 
                    Discount = 20000,
                    TotalPrice = 280000,
                    Notes = "VIP Customer",
                    CreatedDate = DateTime.Now
                }
            };
            context.Orders.AddOrUpdate(o => o.OrderId, orders.ToArray());
            context.SaveChanges();
            Console.WriteLine("✓ Orders inserted");

            // ============================================
            // 9. ORDER DETAILS (Chi tiết hóa đơn bán)
            // ============================================
            Console.WriteLine("Inserting OrderDetails...");
            var orderDetails = new List<OrderDetail>
            {
                new OrderDetail 
                { 
                    OrderId = "ORD001", 
                    BookId = "S00001", 
                    SalePrice = 80000, 
                    Quantity = 1,
                    Subtotal = 80000
                },
                new OrderDetail 
                { 
                    OrderId = "ORD001", 
                    BookId = "S00003", 
                    SalePrice = 50000, 
                    Quantity = 1,
                    Subtotal = 50000
                },
                new OrderDetail 
                { 
                    OrderId = "ORD001", 
                    BookId = "S00004", 
                    SalePrice = 65000, 
                    Quantity = 1,
                    Subtotal = 65000
                },
                new OrderDetail 
                { 
                    OrderId = "ORD002", 
                    BookId = "S00002", 
                    SalePrice = 75000, 
                    Quantity = 2,
                    Subtotal = 150000
                },
                new OrderDetail 
                { 
                    OrderId = "ORD002", 
                    BookId = "S00005", 
                    SalePrice = 95000, 
                    Quantity = 1,
                    Subtotal = 95000
                },
                new OrderDetail 
                { 
                    OrderId = "ORD003", 
                    BookId = "S00006", 
                    SalePrice = 120000, 
                    Quantity = 1,
                    Subtotal = 120000
                },
                new OrderDetail 
                { 
                    OrderId = "ORD004", 
                    BookId = "S00001", 
                    SalePrice = 80000, 
                    Quantity = 1,
                    Subtotal = 80000
                },
                new OrderDetail 
                { 
                    OrderId = "ORD004", 
                    BookId = "S00002", 
                    SalePrice = 75000, 
                    Quantity = 2,
                    Subtotal = 150000
                },
                new OrderDetail 
                { 
                    OrderId = "ORD005", 
                    BookId = "S00007", 
                    SalePrice = 150000, 
                    Quantity = 1,
                    Subtotal = 150000
                },
                new OrderDetail 
                { 
                    OrderId = "ORD005", 
                    BookId = "S00008", 
                    SalePrice = 110000, 
                    Quantity = 1,
                    Subtotal = 110000
                }
            };
            context.OrderDetails.AddOrUpdate(
                od => new { od.OrderId, od.BookId }, 
                orderDetails.ToArray());
            context.SaveChanges();
            Console.WriteLine("✓ OrderDetails inserted");

            // ============================================
            // 10. STAFF DAILY REVENUE (Doanh thu nhân viên theo ngày)
            // ============================================
            Console.WriteLine("Inserting StaffDailyRevenues...");
            var staffDailyRevenues = new List<StaffDailyRevenue>
            {
                new StaffDailyRevenue 
                { 
                    EmployeeId = "NV0001", 
                    Day = DateTime.Now.AddDays(-5), 
                    Revenue = 155000
                },
                new StaffDailyRevenue 
                { 
                    EmployeeId = "NV0001", 
                    Day = DateTime.Now.AddDays(-1), 
                    Revenue = 245000
                },
                new StaffDailyRevenue 
                { 
                    EmployeeId = "NV0003", 
                    Day = DateTime.Now.AddDays(-3), 
                    Revenue = 210000
                },
                new StaffDailyRevenue 
                { 
                    EmployeeId = "NV0003", 
                    Day = DateTime.Now, 
                    Revenue = 280000
                },
                new StaffDailyRevenue 
                { 
                    EmployeeId = "NV0005", 
                    Day = DateTime.Now.AddDays(-1), 
                    Revenue = 245000
                }
            };
            context.StaffDailyRevenues.AddOrUpdate(
                sdr => new { sdr.EmployeeId, sdr.Day }, 
                staffDailyRevenues.ToArray());
            context.SaveChanges();
            Console.WriteLine("✓ StaffDailyRevenues inserted");

            Console.WriteLine("\n✅ All seed data inserted successfully!");
            base.Seed(context);
        }
    }
}