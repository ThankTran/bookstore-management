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
        public DbSet<Publisher> Publishers { get; set; }
        
        public DbSet<User> Users { get; set; }
        public DbSet<ImportBill> ImportBills { get; set; }
        public DbSet<ImportBillDetail> ImportBillDetails { get; set; }
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
            modelBuilder.Entity<Publisher>().HasKey(s => s.Id);
            modelBuilder.Entity<User>().HasKey(u => u.Username);
            modelBuilder.Entity<ImportBill>().HasKey(ib => ib.Id);
            modelBuilder.Entity<ImportBillDetail>().HasKey(ibd => new { ibd.BookId, ibd.ImportId });
            modelBuilder.Entity<AuditLog>().HasKey(al => al.Id);

            // ============================================
            // FOREIGN KEYS & RELATIONSHIPS
            // ============================================

            // Order → Staff (1 Order có 1 Staff)
            modelBuilder.Entity<Order>()
                .HasRequired(o => o.Staff)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.StaffId)
                .WillCascadeOnDelete(false);

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
                .WillCascadeOnDelete(false);

            // OrderDetail → Book (1 OrderDetail có 1 Book)
            modelBuilder.Entity<OrderDetail>()
                .HasRequired(od => od.Book)
                .WithMany(b => b.OrderDetails)
                .HasForeignKey(od => od.BookId)
                .WillCascadeOnDelete(false);
            

            // ImportBill → Publisher (1 ImportBill có 1 Supplier)
            modelBuilder.Entity<ImportBill>()
                .HasRequired(ib => ib.Publisher)
                .WithMany(s => s.ImportBills)
                .HasForeignKey(ib => ib.PublisherId)
                .WillCascadeOnDelete(false);

            // Book -> Publisher (optional)
            modelBuilder.Entity<Book>()
                .HasOptional(b => b.Publisher)
                .WithMany()
                .HasForeignKey(b => b.PublisherId)
                .WillCascadeOnDelete(false);
            

            // ImportBillDetail → ImportBill (1 ImportBillDetail thuộc 1 ImportBill)
            modelBuilder.Entity<ImportBillDetail>()
                .HasRequired(ibd => ibd.Import)
                .WithMany(ib => ib.ImportBillDetails)
                .HasForeignKey(ibd => ibd.ImportId)
                .WillCascadeOnDelete(false);

            // ImportBillDetail → Book (1 ImportBillDetail có 1 Book)
            modelBuilder.Entity<ImportBillDetail>()
                .HasRequired(ibd => ibd.Book)
                .WithMany()  // Book không có navigation property đến ImportBillDetail
                .HasForeignKey(ibd => ibd.BookId)
                .WillCascadeOnDelete(false);
            
            

            // ============================================
            // INDEXES (Tối ưu tìm kiếm)
            // ============================================

            
        }
    }
}