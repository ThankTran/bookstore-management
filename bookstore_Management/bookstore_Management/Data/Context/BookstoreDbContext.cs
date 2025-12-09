using System.Data.Entity;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Context
{
/// <summary>
    /// Entity Framework DbContext - Kết nối với Database
    /// OnModelCreating(...) config tất cả relationships, keys, constraints
    /// </summary>
    public class BookstoreDbContext : DbContext
    {
        // Constructor - kết nối với connection string trong app.config
        public BookstoreDbContext() : base("name=BookstoreConnection")
        {
        }

        // Các DbSet - tương ứng với các bảng trong Database
        public DbSet<Book> Books { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<StaffDailyRevenue> StaffDailyRevenues { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ImportBill> ImportBills { get; set; }
        public DbSet<ImportBillDetail> ImportBillDetails { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // ============================================
            // PRIMARY KEYS
            // ============================================
            
            modelBuilder.Entity<Book>().HasKey(b => b.BookId);
            modelBuilder.Entity<Order>().HasKey(o => o.OrderId);
            modelBuilder.Entity<OrderDetail>().HasKey(od => new { od.BookId, od.OrderId });
            modelBuilder.Entity<Customer>().HasKey(c => c.CustomerId);
            modelBuilder.Entity<Staff>().HasKey(s => s.Id);
            modelBuilder.Entity<Supplier>().HasKey(s => s.Id);
            modelBuilder.Entity<StaffDailyRevenue>().HasKey(s => new { s.EmployeeId, s.Day });
            modelBuilder.Entity<User>().HasKey(u => u.UserId);
            modelBuilder.Entity<ImportBill>().HasKey(ib => ib.ImportBillId);
            modelBuilder.Entity<ImportBillDetail>().HasKey(ibd => new { ibd.BookId, ibd.ImportId });
            modelBuilder.Entity<Stock>().HasKey(st => st.BookId);
            modelBuilder.Entity<AuditLog>().HasKey(al => al.Id);

            // ============================================
            // FOREIGN KEYS & RELATIONSHIPS
            // ============================================

            // Order → Staff (1 Order có 1 Staff)
            modelBuilder.Entity<Order>()
                .HasRequired(o => o.Staff)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.StaffId)
                .WillCascadeOnDelete(true);

            // Order → Customer (1 Order có 0 hoặc 1 Customer - khách vãng lai)
            modelBuilder.Entity<Order>()
                .HasOptional(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .WillCascadeOnDelete(false);

            // OrderDetail → Order (1 OrderDetail thuộc 1 Order)
            modelBuilder.Entity<OrderDetail>()
                .HasRequired(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .WillCascadeOnDelete(true);

            // OrderDetail → Book (1 OrderDetail có 1 Book)
            modelBuilder.Entity<OrderDetail>()
                .HasRequired(od => od.Book)
                .WithMany(b => b.OrderDetails)
                .HasForeignKey(od => od.BookId)
                .WillCascadeOnDelete(false);

            // Book → Supplier (1 Book có 1 Supplier)
            modelBuilder.Entity<Book>()
                .HasRequired(b => b.Supplier)
                .WithMany(s => s.Books)
                .HasForeignKey(b => b.SupplierId)
                .WillCascadeOnDelete(false);

            // StaffDailyRevenue → Staff (1 DailyRevenue có 1 Staff)
            modelBuilder.Entity<StaffDailyRevenue>()
                .HasRequired(s => s.Staff)
                .WithMany(s => s.DailyRevenues)
                .HasForeignKey(s => s.EmployeeId)
                .WillCascadeOnDelete(true);

            // ImportBill → Supplier (1 ImportBill có 1 Supplier)
            modelBuilder.Entity<ImportBill>()
                .HasRequired(ib => ib.Supplier)
                .WithMany(s => s.ImportBills)
                .HasForeignKey(ib => ib.SupplierId)
                .WillCascadeOnDelete(false);

            // ImportBillDetail → ImportBill (1 ImportBillDetail thuộc 1 ImportBill)
            modelBuilder.Entity<ImportBillDetail>()
                .HasRequired(ibd => ibd.Import)
                .WithMany(ib => ib.ImportBillDetails)
                .HasForeignKey(ibd => ibd.ImportId)
                .WillCascadeOnDelete(true);

            // ImportBillDetail → Book (1 ImportBillDetail có 1 Book)
            modelBuilder.Entity<ImportBillDetail>()
                .HasRequired(ibd => ibd.Book)
                .WithMany()  // Book không có navigation property đến ImportBillDetail
                .HasForeignKey(ibd => ibd.BookId)
                .WillCascadeOnDelete(false);

            // Stock → Book (1 Stock - 1 Book, quan hệ 1-1)
            modelBuilder.Entity<Book>()
                .HasOptional(b => new Stock())
                .WithRequired(st => st.Book);

            // ============================================
            // DECIMAL PRECISION (12,2 = 10 chữ số + 2 số thập phân)
            // ============================================

            modelBuilder.Entity<Book>()
                .Property(b => b.ImportPrice)
                .HasPrecision(12, 2);

            modelBuilder.Entity<Book>()
                .Property(b => b.SalePrice)
                .HasPrecision(12, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalPrice)
                .HasPrecision(12, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.Discount)
                .HasPrecision(12, 2);

            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.SalePrice)
                .HasPrecision(12, 2);

            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.Subtotal)
                .HasPrecision(12, 2);

            modelBuilder.Entity<StaffDailyRevenue>()
                .Property(s => s.Revenue)
                .HasPrecision(12, 2);

            modelBuilder.Entity<Staff>()
                .Property(s => s.BaseSalary)
                .HasPrecision(12, 2);

            modelBuilder.Entity<Staff>()
                .Property(s => s.SalaryRate)
                .HasPrecision(12, 2);

            modelBuilder.Entity<Customer>()
                .Property(c => c.LoyaltyPoints)
                .HasPrecision(12, 2);

            modelBuilder.Entity<ImportBill>()
                .Property(ib => ib.TotalAmount)
                .HasPrecision(12, 2);

            modelBuilder.Entity<ImportBillDetail>()
                .Property(ibd => ibd.ImportPrice)
                .HasPrecision(12, 2);

            modelBuilder.Entity<ImportBillDetail>()
                .Property(ibd => ibd.TotalPrice)
                .HasPrecision(12, 2);

            // ============================================
            // INDEXES (Tối ưu tìm kiếm)
            // ============================================

            // Book indexes
            modelBuilder.Entity<Book>()
                .HasIndex(b => b.SupplierId);

            modelBuilder.Entity<Book>()
                .HasIndex(b => b.DeletedDate);

            // Order indexes
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.StaffId);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.CustomerId);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.CreatedDate);

            // Stock indexes
            modelBuilder.Entity<Stock>()
                .HasIndex(st => st.BookId).IsUnique();

            // AuditLog indexes
            modelBuilder.Entity<AuditLog>()
                .HasIndex(al => al.EntityName);

            modelBuilder.Entity<AuditLog>()
                .HasIndex(al => al.EntityId);

            modelBuilder.Entity<AuditLog>()
                .HasIndex(al => al.ChangedDate);

            modelBuilder.Entity<AuditLog>()
                .HasIndex(al => new { al.EntityName, al.EntityId });

            base.OnModelCreating(modelBuilder);
        }
    }
}