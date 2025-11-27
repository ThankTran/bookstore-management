using System.Data.Entity;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Context
{
    /// <summary>
    /// sử dụng Entity Framework để có thể kết nối DB dễ dàng hơn
    /// các DbSet là những bảng đã được tạo dựa tron model
    /// OnModelCreating(...) giống như một bản đồ để cho EF có thể tạo một DB trên các máy khác nhau
    /// </summary>
    public class BookstoreDbContext : DbContext
    {
        // Constructor - sử dụng base để kết nối với connection trong app.config
        public BookstoreDbContext() : base("name=BookstoreConnection")
        {
        }

        // Các DbSet
        public DbSet<Book> Books { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<StaffDailyRevenue> StaffDailyRevenues { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
            // config khóa chính cho từng bảng
            modelBuilder.Entity<Book>().HasKey(b => b.BookId);
            modelBuilder.Entity<Order>().HasKey(o => o.OrderId);
            modelBuilder.Entity<Customer>().HasKey(c => c.CustomerId);
            modelBuilder.Entity<Staff>().HasKey(s => s.Id);
            modelBuilder.Entity<Supplier>().HasKey(s => s.Id);
            modelBuilder.Entity<User>().HasKey(u => u.UserId); // cần phải xem xét lại
            modelBuilder.Entity<OrderDetail>().HasKey(od => new {od.BookId, od.OrderId});
            modelBuilder.Entity<StaffDailyRevenue>().HasKey(s => new { s.EmployeeId, s.Day });

            // config các khóa ngoại
            // HasReq --> quan hệ 1 
            // WithMany --> quan hệ nhiều
            // HasOptional --> có thể là null
            // HasForeignKey --> khóa ngoại
            
            // Order → Staff 
            modelBuilder.Entity<Order>()
                .HasRequired(o => o.Staff) // mỗi hóa đơn chỉ có 1 staff
                .WithMany(s => s.Orders) // mỗi staff có thể có nhiều hóa đơn
                .HasForeignKey(o => o.StaffId); // khóa ngoại kết nối qua cột staff
            
            // Order → Customer
            modelBuilder.Entity<Order>()
                .HasOptional(o => o.Customer)  // Customer có thể null
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId);

            // OrderDetail → Order
            modelBuilder.Entity<OrderDetail>()
                .HasRequired(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId);

            // OrderDetail → Book
            modelBuilder.Entity<OrderDetail>()
                .HasRequired(od => od.Book)
                .WithMany(b => b.OrderDetails)
                .HasForeignKey(od => od.BookId);

            // Book → Supplier
            modelBuilder.Entity<Book>()
                .HasRequired(b => b.Supplier)
                .WithMany(s => s.Books)
                .HasForeignKey(b => b.SupplierId);

            // StaffDailyRevenue → Staff
            modelBuilder.Entity<StaffDailyRevenue>()
                .HasRequired(s => s.Staff)
                .WithMany(s => s.DailyRevenues)
                .HasForeignKey(s => s.EmployeeId);

            // config các kiểu dữ liệu cho các cột cần thiết
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

            modelBuilder.Entity<StaffDailyRevenue>()
                .Property(s => s.Revenue)
                .HasPrecision(12, 2);
            

            base.OnModelCreating(modelBuilder);
        }
    }
}