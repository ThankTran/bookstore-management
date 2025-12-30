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
        
        public DbSet<User> Users { get; set; }
        public DbSet<ImportBill> ImportBills { get; set; }
        public DbSet<ImportBillDetail> ImportBillDetails { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }

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
            modelBuilder.Entity<User>().HasKey(u => u.UserId);
            modelBuilder.Entity<ImportBill>().HasKey(ib => ib.Id);
            modelBuilder.Entity<ImportBillDetail>().HasKey(ibd => new { ibd.BookId, ibd.ImportId });
            modelBuilder.Entity<Stock>().HasKey(st => new { st.WarehouseId, st.BookId });
            modelBuilder.Entity<AuditLog>().HasKey(al => al.Id);
            modelBuilder.Entity<Warehouse>().HasKey(w => w.WarehouseId);
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
            

            // ImportBill → Supplier (1 ImportBill có 1 Supplier)
            modelBuilder.Entity<ImportBill>()
                .HasRequired(ib => ib.Supplier)
                .WithMany(s => s.ImportBills)
                .HasForeignKey(ib => ib.SupplierId)
                .WillCascadeOnDelete(false);

            // Book -> Supplier (optional)
            modelBuilder.Entity<Book>()
                .HasOptional(b => b.Supplier)
                .WithMany()
                .HasForeignKey(b => b.SupplierId)
                .WillCascadeOnDelete(false);

            // ImportBill → Warehouse (1 ImportBill có 1 Warehouse)
            modelBuilder.Entity<ImportBill>()
                .HasRequired(ib => ib.Warehouse)
                .WithMany(w => w.ImportBills)
                .HasForeignKey(ib => ib.WarehouseId)
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

            // Stock → Book/Warehouse (mỗi kho có tồn kho cho từng sách)
            modelBuilder.Entity<Stock>()
                .HasRequired(st => st.Book)
                .WithMany(b => b.Stocks)
                .HasForeignKey(st => st.BookId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Stock>()
                .HasRequired(st => st.Warehouse)
                .WithMany(w => w.Stocks)
                .HasForeignKey(st => st.WarehouseId)
                .WillCascadeOnDelete(false);
            
            

            // ============================================
            // INDEXES (Tối ưu tìm kiếm)
            // ============================================

            
        }
    }
}