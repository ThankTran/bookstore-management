using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
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
            modelBuilder.Entity<User>().HasKey(u => u.UserId);
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

            // User → Staff (1 User là của 1 Staff)
            modelBuilder.Entity<User>()
                .HasRequired(u => u.staff)
                .WithMany()  // Book không có navigation property đến ImportBillDetail
                .HasForeignKey(s => s.StaffId)
                .WillCascadeOnDelete(false);



            // ============================================
            // INDEXES (Tối ưu tìm kiếm)
            // ============================================

            // Book
            modelBuilder.Entity<Book>()
                .Property(b => b.DeletedDate)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_Book_DeletedDate")
                    )
                );

            modelBuilder.Entity<Book>()
                .Property(b => b.Name)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_Book_Name")
                    )
                );
            
            // Customer
            modelBuilder.Entity<Customer>()
                .Property(c => c.Name)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_Customer_Name")
                    )
                );
            
            modelBuilder.Entity<Customer>()
                .Property(c => c.DeletedDate)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_Customer_DeletedDate")
                    )
                );
            
            // Staff
            modelBuilder.Entity<Staff>()
                .Property(c => c.Name)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_Staff_Name")
                    )
                );
            modelBuilder.Entity<Staff>()
                .Property(c => c.DeletedDate)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_Staff_DeletedDate")
                    )
                );
            
            // Publisher
            modelBuilder.Entity<Publisher>()
                .Property(p => p.Name)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_Publishers_Name")
                    )
                );
            
            modelBuilder.Entity<Publisher>()
                .Property(p => p.DeletedDate)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_Publishers_DeletedDate")
                    )
                );
            
            // ImportBill
            modelBuilder.Entity<ImportBill>()
                .Property(p => p.DeletedDate)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_ImportBill_DeletedDate")
                    )
                );
            
            // ImportBillDetail
            modelBuilder.Entity<ImportBillDetail>()
                .Property(p => p.DeletedDate)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_ImportBillDetail_DeletedDate")
                    )
                );
            modelBuilder.Entity<ImportBillDetail>()
                .Property(p => p.ImportId)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_ImportBillDetail_ImportId")
                    )
                );
            
            // Order
            modelBuilder.Entity<Order>()
                .Property(p => p.DeletedDate)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_Order_DeletedDate")
                    )
                );
                // OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .Property(p => p.OrderId)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_OrderDetail_OrderId")
                    )
                );
            
            // User
            modelBuilder.Entity<User>()
                .Property(p => p.Username)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_User_Username")
                    )
                );
            modelBuilder.Entity<User>()
                .Property(p => p.DeletedDate)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_User_DeletedDate")
                    )
                );



        }
    }
}