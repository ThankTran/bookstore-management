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
                #region Publishers
      // ============================================
      // Publishers
      // ============================================
      var publishers = new List<Publisher>
      {
          new Publisher { Id = "NXB001", Name = "NXB Kim Đồng", Phone = "0243854354", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB002", Name = "NXB Trẻ", Phone = "0283931628", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB003", Name = "NXB Phụ Nữ", Phone = "0243822539", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB004", Name = "NXB Giáo Dục Việt Nam", Phone = "02438220801", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB005", Name = "NXB Lao Động", Phone = "0243854687", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB006", Name = "NXB Văn Học", Phone = "0243943485", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB007", Name = "NXB Tổng Hợp TP.HCM", Phone = "02838222840", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB008", Name = "NXB Chính Trị Quốc Gia", Phone = "02437472007", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB009", Name = "NXB Khoa Học & Kỹ Thuật", Phone = "0243971632", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB010", Name = "NXB Đại Học Quốc Gia Hà Nội", Phone = "02437547963", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB011", Name = "NXB Đại Học Quốc Gia TP.HCM", Phone = "02837242181", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB012", Name = "NXB Thanh Niên", Phone = "02838293801", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB013", Name = "NXB Văn Hóa – Văn Nghệ", Phone = "02838368907", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB014", Name = "NXB Hồng Đức", Phone = "02437732295", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB015", Name = "NXB Thế Giới", Phone = "02439347968", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB016", Name = "NXB Công Thương", Phone = "02422205555", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB017", Name = "NXB Y Học", Phone = "02438225505", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB018", Name = "NXB Tài Chính", Phone = "02439423365", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB019", Name = "NXB Giao Thông Vận Tải", Phone = "02439422055", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB020", Name = "NXB Xây Dựng", Phone = "02439430578", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB021", Name = "NXB Mỹ Thuật", Phone = "02438223657", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB022", Name = "NXB Âm Nhạc", Phone = "02439336572", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB023", Name = "NXB Sự Thật", Phone = "02437472222", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB024", Name = "NXB Thông Tin & Truyền Thông", Phone = "02439444567", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB025", Name = "NXB Kinh Tế Quốc Dân", Phone = "02438696699", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB026", Name = "NXB Đại Học Sư Phạm", Phone = "02437547890", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB027", Name = "NXB Tư Pháp", Phone = "02438222888", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB028", Name = "NXB Khoa Học Xã Hội", Phone = "02439332211", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB029", Name = "NXB Dân Trí", Phone = "02439441122", CreatedDate = DateTime.Now },
          new Publisher { Id = "NXB030", Name = "NXB Tri Thức", Phone = "02439335544", CreatedDate = DateTime.Now }
      };

      context.Publishers.AddOrUpdate(s => s.Id, publishers.ToArray());
      context.SaveChanges();

      #endregion  
                
                #region Books
                // ============================================
                // Books
                // ============================================
                var books = new List<Book>
                {
                    new Book { BookId = "S00001", Name = "Đắc Nhân Tâm", Author = "Dale Carnegie", PublisherId = "NXB001", Category = BookCategory.Psychology, SalePrice = 80000, Stock = 30, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00002", Name = "Nhà Giả Kim", Author = "Paulo Coelho", PublisherId = "NXB001", Category = BookCategory.Novel, SalePrice = 75000, Stock = 50, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00003", Name = "Tuổi Trẻ Đáng Giá Bao Nhiêu", Author = "Rosie Nguyễn", PublisherId = "NXB002", Category = BookCategory.Literature, SalePrice = 50000, Stock = 10, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00004", Name = "Tôi Tài Giỏi Bạn Cũng Thế", Author = "Adam Khoo", PublisherId = "NXB002", Category = BookCategory.Economics, SalePrice = 65000, Stock = 35, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00005", Name = "Bố Già", Author = "Mario Puzo", PublisherId = "NXB003", Category = BookCategory.Literature, SalePrice = 95000, Stock = 100, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00006", Name = "7 Thói Quen Hiệu Quả", Author = "Stephen R. Covey", PublisherId = "NXB001", Category = BookCategory.Psychology, SalePrice = 120000, Stock = 40, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00007", Name = "Muôn Kiếp Nhân Sinh", Author = "Nguyên Phong", PublisherId = "NXB004", Category = BookCategory.Literature, SalePrice = 135000, Stock = 60, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00008", Name = "Dám Nghĩ Lớn", Author = "David J. Schwartz", PublisherId = "NXB001", Category = BookCategory.Psychology, SalePrice = 90000, Stock = 25, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00009", Name = "Hạt Giống Tâm Hồn", Author = "Nhiều Tác Giả", PublisherId = "NXB001", Category = BookCategory.Literature, SalePrice = 60000, Stock = 80, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00010", Name = "Không Gia Đình", Author = "Hector Malot", PublisherId = "NXB001", Category = BookCategory.Novel, SalePrice = 70000, Stock = 45, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00011", Name = "Cha Giàu Cha Nghèo", Author = "Robert Kiyosaki", PublisherId = "NXB005", Category = BookCategory.Economics, SalePrice = 110000, Stock = 55, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00012", Name = "Tư Duy Nhanh Và Chậm", Author = "Daniel Kahneman", PublisherId = "NXB006", Category = BookCategory.Psychology, SalePrice = 150000, Stock = 20, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00013", Name = "Clean Code", Author = "Robert C. Martin", PublisherId = "NXB009", Category = BookCategory.ForeignLanguage, SalePrice = 180000, Stock = 15, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00014", Name = "Lập Trình C# Căn Bản", Author = "Phạm Hữu Khang", PublisherId = "NXB009", Category = BookCategory.Literature, SalePrice = 95000, Stock = 70, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00015", Name = "Dế Mèn Phiêu Lưu Ký", Author = "Tô Hoài", PublisherId = "NXB001", Category = BookCategory.Literature, SalePrice = 55000, Stock = 90, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00016", Name = "Harry Potter Và Hòn Đá Phù Thủy", Author = "J.K. Rowling", PublisherId = "NXB007", Category = BookCategory.Novel, SalePrice = 125000, Stock = 65, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00017", Name = "Sherlock Holmes Toàn Tập", Author = "Arthur Conan Doyle", PublisherId = "NXB006", Category = BookCategory.Novel, SalePrice = 160000, Stock = 30, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00018", Name = "Tắt Đèn", Author = "Ngô Tất Tố", PublisherId = "NXB006", Category = BookCategory.Literature, SalePrice = 50000, Stock = 40, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00019", Name = "Sapiens – Lược Sử Loài Người", Author = "Yuval Noah Harari", PublisherId = "NXB008", Category = BookCategory.Children, SalePrice = 170000, Stock = 22, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00020", Name = "Lược Sử Thời Gian", Author = "Stephen Hawking", PublisherId = "NXB008", Category = BookCategory.ForeignLanguage, SalePrice = 145000, Stock = 18, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00021", Name = "Marketing Căn Bản", Author = "Philip Kotler", PublisherId = "NXB005", Category = BookCategory.Economics, SalePrice = 130000, Stock = 35, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00022", Name = "Khởi Nghiệp Tinh Gọn", Author = "Eric Ries", PublisherId = "NXB005", Category = BookCategory.Economics, SalePrice = 115000, Stock = 28, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00023", Name = "Design Patterns", Author = "Erich Gamma", PublisherId = "NXB009", Category = BookCategory.Biography, SalePrice = 190000, Stock = 12, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00024", Name = "Tâm Lý Học Hành Vi", Author = "Dan Ariely", PublisherId = "NXB006", Category = BookCategory.Psychology, SalePrice = 140000, Stock = 20, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00025", Name = "Lão Hạc", Author = "Nam Cao", PublisherId = "NXB006", Category = BookCategory.Literature, SalePrice = 45000, Stock = 60, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00026", Name = "Đừng Làm Việc Chăm Chỉ Hãy Làm Việc Thông Minh", Author = "Tony Schwartz", PublisherId = "NXB001", Category = BookCategory.Psychology, SalePrice = 88000, Stock = 33, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00027", Name = "SQL Cơ Bản Đến Nâng Cao", Author = "Nguyễn Văn Minh", PublisherId = "NXB009", Category = BookCategory.Textbook, SalePrice = 105000, Stock = 48, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00028", Name = "Cây Cam Ngọt Của Tôi", Author = "José Mauro de Vasconcelos", PublisherId = "NXB001", Category = BookCategory.Novel, SalePrice = 68000, Stock = 52, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00029", Name = "Atomic Habits", Author = "James Clear", PublisherId = "NXB001", Category = BookCategory.Psychology, SalePrice = 125000, Stock = 47, CreatedDate = DateTime.Now },
                    new Book { BookId = "S00030", Name = "Tư Duy Phản Biện", Author = "Richard Paul", PublisherId = "NXB006", Category = BookCategory.Psychology, SalePrice = 99000, Stock = 26, CreatedDate = DateTime.Now }
                };

                context.Books.AddOrUpdate(b => b.BookId, books.ToArray());
                context.SaveChanges();
                #endregion

                #region Staffs
                // ============================================
                // Staff
                // ============================================
                var staffs = new List<Staff>
                {
                    new Staff { Id = "NV0001", Name = "Nguyễn Minh Tuấn", Phone = "0901234567", CitizenId = "079203001234", UserRole = UserRole.SalesManager, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0002", Name = "Trần Thị Thanh Nga", Phone = "0912345678", CitizenId = "079203004567", UserRole = UserRole.CustomerManager, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0003", Name = "Phạm Quốc Huy", Phone = "0923456789", CitizenId = "079203007890", UserRole = UserRole.SalesStaff, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0004", Name = "Lê Hoàng Anh", Phone = "0934567890", CitizenId = "079203009876", UserRole = UserRole.SalesStaff, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0005", Name = "Nguyễn Thị Mai", Phone = "0945678901", CitizenId = "079203002345", UserRole = UserRole.InventoryManager, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0006", Name = "Võ Thành Long", Phone = "0966789012", CitizenId = "079203006543", UserRole = UserRole.SalesStaff, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0007", Name = "Bùi Thị Hồng Nhung", Phone = "0977890123", CitizenId = "079203008901", UserRole = UserRole.SalesStaff, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0008", Name = "Đặng Quốc Bảo", Phone = "0988901234", CitizenId = "079203005678", UserRole = UserRole.InventoryManager, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0009", Name = "Phan Thị Thu Hà", Phone = "0399012345", CitizenId = "079203003210", UserRole = UserRole.SalesStaff, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0010", Name = "Ngô Minh Khang", Phone = "0388123456", CitizenId = "079203000999", UserRole = UserRole.SalesStaff, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0011", Name = "Nguyễn Hoàng Phúc", Phone = "0902345678", CitizenId = "079203011111", UserRole = UserRole.SalesStaff, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0012", Name = "Trần Thị Bích Ngọc", Phone = "0913456789", CitizenId = "079203012222", UserRole = UserRole.CustomerManager, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0013", Name = "Lê Quốc Khánh", Phone = "0924567890", CitizenId = "079203013333", UserRole = UserRole.SalesStaff, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0014", Name = "Phạm Thị Thu Uyên", Phone = "0935678901", CitizenId = "079203014444", UserRole = UserRole.InventoryManager, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0015", Name = "Vũ Minh Đức", Phone = "0946789012", CitizenId = "079203015555", UserRole = UserRole.SalesStaff, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0016", Name = "Nguyễn Thị Kim Anh", Phone = "0967890123", CitizenId = "079203016666", UserRole = UserRole.SalesStaff, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0017", Name = "Đỗ Thành Trung", Phone = "0978901234", CitizenId = "079203017777", UserRole = UserRole.InventoryManager, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0018", Name = "Bùi Thị Ngọc Trâm", Phone = "0989012345", CitizenId = "079203018888", UserRole = UserRole.SalesStaff, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0019", Name = "Ngô Quốc Việt", Phone = "0390123456", CitizenId = "079203019999", UserRole = UserRole.SalesStaff, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0020", Name = "Phan Thị Mỹ Linh", Phone = "0381234567", CitizenId = "079203020000", UserRole = UserRole.InventoryManager, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0021", Name = "Huỳnh Minh Quân", Phone = "0903456789", CitizenId = "079203021111", UserRole = UserRole.SalesStaff, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0022", Name = "Trịnh Thị Lan Hương", Phone = "0914567890", CitizenId = "079203022222", UserRole = UserRole.SalesStaff, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0023", Name = "Nguyễn Quốc Bảo", Phone = "0925678901", CitizenId = "079203023333", UserRole = UserRole.SalesStaff, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0024", Name = "Lê Thị Thanh Thảo", Phone = "0936789012", CitizenId = "079203024444", UserRole = UserRole.CustomerManager, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0025", Name = "Phạm Minh Nhật", Phone = "0947890123", CitizenId = "079203025555", UserRole = UserRole.SalesStaff, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0026", Name = "Nguyễn Thị Yến Nhi", Phone = "0968901234", CitizenId = "079203026666", UserRole = UserRole.SalesStaff, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0027", Name = "Đặng Quốc Thịnh", Phone = "0979012345", CitizenId = "079203027777", UserRole = UserRole.InventoryManager, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0028", Name = "Bùi Thị Thu Phương", Phone = "0980123456", CitizenId = "079203028888", UserRole = UserRole.SalesStaff, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0029", Name = "Võ Minh Tâm", Phone = "0391234567", CitizenId = "079203029999", UserRole = UserRole.SalesStaff, CreatedDate = DateTime.Now },
                    new Staff { Id = "NV0030", Name = "Nguyễn Thị Ánh Tuyết", Phone = "0382345678", CitizenId = "079203030000", UserRole = UserRole.Administrator, CreatedDate = DateTime.Now }
                };

                context.Staff.AddOrUpdate(s => s.Id, staffs.ToArray());
                context.SaveChanges();

                #endregion

                #region Customers
                // ============================================
                // Customers
                // ============================================
                var customers = new List<Customer>
                {
                    new Customer { CustomerId = "KH0001", Name = "Trần Minh Hoàng", Phone = "0904567891", Email = "minhhoang@gmail.com", Address = "Quận Bình Thạnh", MemberLevel = MemberTier.Gold, LoyaltyPoints = 4800, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0002", Name = "Nguyễn Thị Diệu Linh", Phone = "0915678902", Email = "dieulinh@gmail.com", Address = "Quận Phú Nhuận", MemberLevel = MemberTier.Bronze, LoyaltyPoints = 90, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0003", Name = "Nguyễn Minh Anh", Phone = "0901234567", Email = "minhanh@gmail.com", Address = "Quận 1", MemberLevel = MemberTier.Bronze, LoyaltyPoints = 120, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0004", Name = "Trần Thị Thu Hà", Phone = "0912345678", Email = "thuhat@gmail.com", Address = "Quận 3", MemberLevel = MemberTier.Silver, LoyaltyPoints = 850, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0005", Name = "Phạm Quốc Bảo", Phone = "0923456789", Email = "quocbao@gmail.com", Address = "Quận 5", MemberLevel = MemberTier.Gold, LoyaltyPoints = 2200, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0006", Name = "Lê Thị Mai", Phone = "0934567801", Email = "lethimai@gmail.com", Address = "Quận 10", MemberLevel = MemberTier.Bronze, LoyaltyPoints = 60, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0007", Name = "Võ Minh Tuấn", Phone = "0945678012", Email = "minhtuan@gmail.com", Address = "Quận 11", MemberLevel = MemberTier.Silver, LoyaltyPoints = 1100, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0008", Name = "Nguyễn Thị Ngọc", Phone = "0966789012", Email = "ngocnt@gmail.com", Address = "Quận Bình Thạnh", MemberLevel = MemberTier.Gold, LoyaltyPoints = 3400, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0009", Name = "Trần Quốc Khánh", Phone = "0977890123", Email = "khanhtr@gmail.com", Address = "Quận Tân Bình", MemberLevel = MemberTier.Bronze, LoyaltyPoints = 200, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0010", Name = "Bùi Thị Hồng", Phone = "0988901234", Email = "hongbui@gmail.com", Address = "Quận Gò Vấp", MemberLevel = MemberTier.Silver, LoyaltyPoints = 950, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0011", Name = "Ngô Minh Quân", Phone = "0399012345", Email = "minhquan@gmail.com", Address = "Quận 6", MemberLevel = MemberTier.Gold, LoyaltyPoints = 4100, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0012", Name = "Phan Thị Mỹ Linh", Phone = "0388123456", Email = "mylinh@gmail.com", Address = "Quận 4", MemberLevel = MemberTier.Bronze, LoyaltyPoints = 90, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0013", Name = "Huỳnh Quốc Việt", Phone = "0902345678", Email = "viethq@gmail.com", Address = "Quận 12", MemberLevel = MemberTier.Silver, LoyaltyPoints = 1300, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0014", Name = "Lê Thị Thanh Thảo", Phone = "0913456789", Email = "thanhthao@gmail.com", Address = "Quận 9", MemberLevel = MemberTier.Gold, LoyaltyPoints = 2800, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0015", Name = "Nguyễn Minh Nhật", Phone = "0924567890", Email = "minhnhat@gmail.com", Address = "Quận Thủ Đức", MemberLevel = MemberTier.Diamond, LoyaltyPoints = 8200, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0016", Name = "Trịnh Thị Lan Anh", Phone = "0935678901", Email = "lananh@gmail.com", Address = "Quận Phú Nhuận", MemberLevel = MemberTier.Bronze, LoyaltyPoints = 150, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0017", Name = "Đặng Minh Khoa", Phone = "0946789012", Email = "minhkhoa@gmail.com", Address = "Quận 2", MemberLevel = MemberTier.Silver, LoyaltyPoints = 1750, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0018", Name = "Bùi Thị Ngọc Trâm", Phone = "0967890123", Email = "ngoctram@gmail.com", Address = "Quận 7", MemberLevel = MemberTier.Gold, LoyaltyPoints = 3600, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0019", Name = "Võ Thành Long", Phone = "0978901234", Email = "thanhlong@gmail.com", Address = "Quận 8", MemberLevel = MemberTier.Bronze, LoyaltyPoints = 40, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0020", Name = "Nguyễn Thị Kim Oanh", Phone = "0989012345", Email = "kimoanh@gmail.com", Address = "Quận 10", MemberLevel = MemberTier.Silver, LoyaltyPoints = 980, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0021", Name = "Phạm Quốc Thịnh", Phone = "0390123456", Email = "quocthinh@gmail.com", Address = "Quận 11", MemberLevel = MemberTier.Gold, LoyaltyPoints = 4500, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0022", Name = "Nguyễn Thị Yến Nhi", Phone = "0381234567", Email = "yennhi@gmail.com", Address = "Quận Tân Phú", MemberLevel = MemberTier.Bronze, LoyaltyPoints = 70, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0023", Name = "Lê Minh Phúc", Phone = "0903456789", Email = "minhphuc@gmail.com", Address = "Quận Bình Tân", MemberLevel = MemberTier.Silver, LoyaltyPoints = 1450, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0024", Name = "Trần Thị Thu Uyên", Phone = "0914567890", Email = "thuuyen@gmail.com", Address = "Quận 5", MemberLevel = MemberTier.Gold, LoyaltyPoints = 3900, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0025", Name = "Ngô Quốc Bảo", Phone = "0925678901", Email = "quocbao.ng@gmail.com", Address = "Quận 6", MemberLevel = MemberTier.Diamond, LoyaltyPoints = 9100, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0026", Name = "Phan Thị Ánh Tuyết", Phone = "0936789012", Email = "anhtuyet@gmail.com", Address = "Quận 1", MemberLevel = MemberTier.Bronze, LoyaltyPoints = 110, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0027", Name = "Đỗ Minh Trí", Phone = "0947890123", Email = "minhtri@gmail.com", Address = "Quận 3", MemberLevel = MemberTier.Silver, LoyaltyPoints = 1600, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0028", Name = "Bùi Thị Thu Phương", Phone = "0968901234", Email = "thuphuong@gmail.com", Address = "Quận 4", MemberLevel = MemberTier.Gold, LoyaltyPoints = 5200, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0029", Name = "Võ Minh Tâm", Phone = "0979012345", Email = "minhtam@gmail.com", Address = "Quận 9", MemberLevel = MemberTier.Silver, LoyaltyPoints = 1800, CreatedDate = DateTime.Now },
                    new Customer { CustomerId = "KH0030", Name = "Nguyễn Thị Hạnh", Phone = "0980123456", Email = "thihanh@gmail.com", Address = "Quận 7", MemberLevel = MemberTier.Diamond, LoyaltyPoints = 12000, CreatedDate = DateTime.Now }
                };

                context.Customers.AddOrUpdate(c => c.CustomerId, customers.ToArray());
                context.SaveChanges();

                #endregion

                #region Import Bills
                // ============================================
                // Import Bills
                // ============================================
                var importBills = new List<ImportBill>
                {
                    new ImportBill { Id = "PN0001", PublisherId = "NXB001", TotalAmount = 3_500_000, Notes = "Nhập sách tâm lý", CreatedBy = "NV0001", CreatedDate = DateTime.Now.AddDays(-60) },
                    new ImportBill { Id = "PN0002", PublisherId = "NXB002", TotalAmount = 4_000_000, Notes = "Nhập sách giáo khoa", CreatedBy = "NV0002", CreatedDate = DateTime.Now.AddDays(-58) },
                    new ImportBill { Id = "PN0003", PublisherId = "NXB003", TotalAmount = 5_200_000, Notes = "Nhập sách văn học", CreatedBy = "NV0003", CreatedDate = DateTime.Now.AddDays(-56) },
                    new ImportBill { Id = "PN0004", PublisherId = "NXB004", TotalAmount = 3_800_000, Notes = "Nhập sách thiếu nhi", CreatedBy = "NV0004", CreatedDate = DateTime.Now.AddDays(-54) },
                    new ImportBill { Id = "PN0005", PublisherId = "NXB005", TotalAmount = 6_500_000, Notes = "Nhập sách kinh tế", CreatedBy = "NV0005", CreatedDate = DateTime.Now.AddDays(-52) },
                    new ImportBill { Id = "PN0006", PublisherId = "NXB006", TotalAmount = 4_300_000, Notes = "Nhập sách tiểu thuyết", CreatedBy = "NV0001", CreatedDate = DateTime.Now.AddDays(-50) },
                    new ImportBill { Id = "PN0007", PublisherId = "NXB007", TotalAmount = 5_900_000, Notes = "Nhập sách ngoại ngữ", CreatedBy = "NV0002", CreatedDate = DateTime.Now.AddDays(-48) },
                    new ImportBill { Id = "PN0008", PublisherId = "NXB008", TotalAmount = 3_600_000, Notes = "Nhập sách thiếu nhi", CreatedBy = "NV0003", CreatedDate = DateTime.Now.AddDays(-46) },
                    new ImportBill { Id = "PN0009", PublisherId = "NXB009", TotalAmount = 7_200_000, Notes = "Nhập sách kinh tế", CreatedBy = "NV0004", CreatedDate = DateTime.Now.AddDays(-44) },
                    new ImportBill { Id = "PN0010", PublisherId = "NXB010", TotalAmount = 4_800_000, Notes = "Nhập sách tâm lý", CreatedBy = "NV0005", CreatedDate = DateTime.Now.AddDays(-42) },
                    new ImportBill { Id = "PN0011", PublisherId = "NXB001", TotalAmount = 6_000_000, Notes = "Nhập sách văn học", CreatedBy = "NV0001", CreatedDate = DateTime.Now.AddDays(-40) },
                    new ImportBill { Id = "PN0012", PublisherId = "NXB002", TotalAmount = 3_900_000, Notes = "Nhập sách giáo khoa", CreatedBy = "NV0002", CreatedDate = DateTime.Now.AddDays(-38) },
                    new ImportBill { Id = "PN0013", PublisherId = "NXB003", TotalAmount = 8_100_000, Notes = "Nhập sách tiểu sử", CreatedBy = "NV0003", CreatedDate = DateTime.Now.AddDays(-36) },
                    new ImportBill { Id = "PN0014", PublisherId = "NXB004", TotalAmount = 4_200_000, Notes = "Nhập sách thiếu nhi", CreatedBy = "NV0004", CreatedDate = DateTime.Now.AddDays(-34) },
                    new ImportBill { Id = "PN0015", PublisherId = "NXB005", TotalAmount = 6_800_000, Notes = "Nhập sách kinh tế", CreatedBy = "NV0005", CreatedDate = DateTime.Now.AddDays(-32) },
                    new ImportBill { Id = "PN0016", PublisherId = "NXB006", TotalAmount = 5_400_000, Notes = "Nhập sách tâm lý", CreatedBy = "NV0001", CreatedDate = DateTime.Now.AddDays(-30) },
                    new ImportBill { Id = "PN0017", PublisherId = "NXB007", TotalAmount = 4_700_000, Notes = "Nhập sách ngoại ngữ", CreatedBy = "NV0002", CreatedDate = DateTime.Now.AddDays(-28) },
                    new ImportBill { Id = "PN0018", PublisherId = "NXB008", TotalAmount = 3_600_000, Notes = "Nhập sách văn học", CreatedBy = "NV0003", CreatedDate = DateTime.Now.AddDays(-26) },
                    new ImportBill { Id = "PN0019", PublisherId = "NXB009", TotalAmount = 7_500_000, Notes = "Nhập sách tiểu thuyết", CreatedBy = "NV0004", CreatedDate = DateTime.Now.AddDays(-24) },
                    new ImportBill { Id = "PN0020", PublisherId = "NXB010", TotalAmount = 5_100_000, Notes = "Nhập sách giáo khoa", CreatedBy = "NV0005", CreatedDate = DateTime.Now.AddDays(-22) },
                    new ImportBill { Id = "PN0021", PublisherId = "NXB001", TotalAmount = 6_300_000, Notes = "Nhập sách tiểu sử", CreatedBy = "NV0001", CreatedDate = DateTime.Now.AddDays(-20) },
                    new ImportBill { Id = "PN0022", PublisherId = "NXB002", TotalAmount = 4_000_000, Notes = "Nhập sách thiếu nhi", CreatedBy = "NV0002", CreatedDate = DateTime.Now.AddDays(-18) },
                    new ImportBill { Id = "PN0023", PublisherId = "NXB003", TotalAmount = 8_700_000, Notes = "Nhập sách kinh tế", CreatedBy = "NV0003", CreatedDate = DateTime.Now.AddDays(-16) },
                    new ImportBill { Id = "PN0024", PublisherId = "NXB004", TotalAmount = 5_600_000, Notes = "Nhập sách tâm lý", CreatedBy = "NV0004", CreatedDate = DateTime.Now.AddDays(-14) },
                    new ImportBill { Id = "PN0025", PublisherId = "NXB005", TotalAmount = 4_900_000, Notes = "Nhập sách ngoại ngữ", CreatedBy = "NV0005", CreatedDate = DateTime.Now.AddDays(-12) },
                    new ImportBill { Id = "PN0026", PublisherId = "NXB006", TotalAmount = 6_100_000, Notes = "Nhập sách văn học", CreatedBy = "NV0001", CreatedDate = DateTime.Now.AddDays(-10) },
                    new ImportBill { Id = "PN0027", PublisherId = "NXB007", TotalAmount = 7_800_000, Notes = "Nhập sách tiểu thuyết", CreatedBy = "NV0002", CreatedDate = DateTime.Now.AddDays(-8) },
                    new ImportBill { Id = "PN0028", PublisherId = "NXB008", TotalAmount = 3_700_000, Notes = "Nhập sách thiếu nhi", CreatedBy = "NV0003", CreatedDate = DateTime.Now.AddDays(-6) },
                    new ImportBill { Id = "PN0029", PublisherId = "NXB009", TotalAmount = 5_300_000, Notes = "Nhập sách giáo khoa", CreatedBy = "NV0004", CreatedDate = DateTime.Now.AddDays(-4) },
                    new ImportBill { Id = "PN0030", PublisherId = "NXB010", TotalAmount = 9_200_000, Notes = "Nhập sách kinh tế", CreatedBy = "NV0005", CreatedDate = DateTime.Now.AddDays(-2) }
                };

                context.ImportBills.AddOrUpdate(ib => ib.Id, importBills.ToArray());
                context.SaveChanges();
                #endregion

                #region Import Bill Details
                // ============================================
                // Import Bill Details
                // ============================================
                var importBillDetails = new List<ImportBillDetail>
                {
                    new ImportBillDetail { BookId = "S00001", ImportId = "PN0001", Quantity = 50, ImportPrice = 70000 },
                    new ImportBillDetail { BookId = "S00003", ImportId = "PN0002", Quantity = 60, ImportPrice = 40000 },
                    new ImportBillDetail { BookId = "S00005", ImportId = "PN0003", Quantity = 45, ImportPrice = 82000 },
                    new ImportBillDetail { BookId = "S00015", ImportId = "PN0004", Quantity = 70, ImportPrice = 42000 },
                    new ImportBillDetail { BookId = "S00011", ImportId = "PN0005", Quantity = 55, ImportPrice = 93000 },

                    new ImportBillDetail { BookId = "S00002", ImportId = "PN0006", Quantity = 40, ImportPrice = 62000 },
                    new ImportBillDetail { BookId = "S00020", ImportId = "PN0007", Quantity = 25, ImportPrice = 120000 },
                    new ImportBillDetail { BookId = "S00010", ImportId = "PN0008", Quantity = 35, ImportPrice = 56000 },
                    new ImportBillDetail { BookId = "S00021", ImportId = "PN0009", Quantity = 30, ImportPrice = 102000 },
                    new ImportBillDetail { BookId = "S00006", ImportId = "PN0010", Quantity = 28, ImportPrice = 95000 },

                    new ImportBillDetail { BookId = "S00018", ImportId = "PN0011", Quantity = 65, ImportPrice = 36000 },
                    new ImportBillDetail { BookId = "S00014", ImportId = "PN0012", Quantity = 50, ImportPrice = 72000 },
                    new ImportBillDetail { BookId = "S00019", ImportId = "PN0013", Quantity = 22, ImportPrice = 135000 },
                    new ImportBillDetail { BookId = "S00004", ImportId = "PN0014", Quantity = 40, ImportPrice = 52000 },
                    new ImportBillDetail { BookId = "S00022", ImportId = "PN0015", Quantity = 26, ImportPrice = 88000 },

                    new ImportBillDetail { BookId = "S00012", ImportId = "PN0016", Quantity = 18, ImportPrice = 125000 },
                    new ImportBillDetail { BookId = "S00007", ImportId = "PN0017", Quantity = 32, ImportPrice = 110000 },
                    new ImportBillDetail { BookId = "S00008", ImportId = "PN0018", Quantity = 24, ImportPrice = 72000 },
                    new ImportBillDetail { BookId = "S00016", ImportId = "PN0019", Quantity = 20, ImportPrice = 98000 },
                    new ImportBillDetail { BookId = "S00009", ImportId = "PN0020", Quantity = 60, ImportPrice = 45000 },

                    new ImportBillDetail { BookId = "S00013", ImportId = "PN0021", Quantity = 15, ImportPrice = 140000 },
                    new ImportBillDetail { BookId = "S00017", ImportId = "PN0022", Quantity = 18, ImportPrice = 120000 },
                    new ImportBillDetail { BookId = "S00023", ImportId = "PN0023", Quantity = 12, ImportPrice = 150000 },
                    new ImportBillDetail { BookId = "S00024", ImportId = "PN0024", Quantity = 16, ImportPrice = 112000 },
                    new ImportBillDetail { BookId = "S00027", ImportId = "PN0025", Quantity = 34, ImportPrice = 82000 },

                    new ImportBillDetail { BookId = "S00025", ImportId = "PN0026", Quantity = 80, ImportPrice = 32000 },
                    new ImportBillDetail { BookId = "S00028", ImportId = "PN0027", Quantity = 46, ImportPrice = 52000 },
                    new ImportBillDetail { BookId = "S00030", ImportId = "PN0028", Quantity = 20, ImportPrice = 78000 },
                    new ImportBillDetail { BookId = "S00029", ImportId = "PN0029", Quantity = 22, ImportPrice = 98000 },
                    new ImportBillDetail { BookId = "S00026", ImportId = "PN0030", Quantity = 30, ImportPrice = 65000 }
                };

                context.ImportBillDetails.AddOrUpdate(
                    ibd => new { ibd.BookId, ibd.ImportId },
                    importBillDetails.ToArray());
                context.SaveChanges();

                #endregion

                #region Orders
                // ============================================
                // Orders
                // ============================================
                var orders = new List<Order>
                {
                    new Order { OrderId = "ORD001", StaffId = "NV0001", CustomerId = "KH0001", PaymentMethod = PaymentType.Cash, Discount = 0, TotalPrice = 155000, CreatedDate = DateTime.Now.AddDays(-30) },
                    new Order { OrderId = "ORD002", StaffId = "NV0003", CustomerId = "KH0002", PaymentMethod = PaymentType.Card, Discount = 10000, TotalPrice = 210000, CreatedDate = DateTime.Now.AddDays(-29) },
                    new Order { OrderId = "ORD003", StaffId = "NV0002", CustomerId = "KH0003", PaymentMethod = PaymentType.BankTransfer, Discount = 0, TotalPrice = 98000, CreatedDate = DateTime.Now.AddDays(-28) },
                    new Order { OrderId = "ORD004", StaffId = "NV0004", CustomerId = "KH0004", PaymentMethod = PaymentType.Cash, Discount = 20000, TotalPrice = 175000, CreatedDate = DateTime.Now.AddDays(-27) },
                    new Order { OrderId = "ORD005", StaffId = "NV0005", CustomerId = "KH0005", PaymentMethod = PaymentType.DebitCard, Discount = 30000, TotalPrice = 420000, CreatedDate = DateTime.Now.AddDays(-26) },
                    new Order { OrderId = "ORD006", StaffId = "NV0001", CustomerId = "KH0006", PaymentMethod = PaymentType.Card, Discount = 0, TotalPrice = 125000, CreatedDate = DateTime.Now.AddDays(-25) },
                    new Order { OrderId = "ORD007", StaffId = "NV0003", CustomerId = "KH0007", PaymentMethod = PaymentType.BankTransfer, Discount = 10000, TotalPrice = 265000, CreatedDate = DateTime.Now.AddDays(-24) },
                    new Order { OrderId = "ORD008", StaffId = "NV0002", CustomerId = "KH0008", PaymentMethod = PaymentType.DebitCard, Discount = 20000, TotalPrice = 360000, CreatedDate = DateTime.Now.AddDays(-23) },
                    new Order { OrderId = "ORD009", StaffId = "NV0004", CustomerId = "KH0009", PaymentMethod = PaymentType.Cash, Discount = 0, TotalPrice = 88000, CreatedDate = DateTime.Now.AddDays(-22) },
                    new Order { OrderId = "ORD010", StaffId = "NV0005", CustomerId = "KH0010", PaymentMethod = PaymentType.Card, Discount = 10000, TotalPrice = 145000, CreatedDate = DateTime.Now.AddDays(-21) },
                    new Order { OrderId = "ORD011", StaffId = "NV0001", CustomerId = "KH0011", PaymentMethod = PaymentType.BankTransfer, Discount = 30000, TotalPrice = 510000, CreatedDate = DateTime.Now.AddDays(-20) },
                    new Order { OrderId = "ORD012", StaffId = "NV0003", CustomerId = "KH0012", PaymentMethod = PaymentType.Cash, Discount = 0, TotalPrice = 99000, CreatedDate = DateTime.Now.AddDays(-19) },
                    new Order { OrderId = "ORD013", StaffId = "NV0002", CustomerId = "KH0013", PaymentMethod = PaymentType.Card, Discount = 20000, TotalPrice = 285000, CreatedDate = DateTime.Now.AddDays(-18) },
                    new Order { OrderId = "ORD014", StaffId = "NV0004", CustomerId = "KH0014", PaymentMethod = PaymentType.DebitCard, Discount = 10000, TotalPrice = 330000, CreatedDate = DateTime.Now.AddDays(-17) },
                    new Order { OrderId = "ORD015", StaffId = "NV0005", CustomerId = "KH0015", PaymentMethod = PaymentType.BankTransfer, Discount = 30000, TotalPrice = 620000, CreatedDate = DateTime.Now.AddDays(-16) },
                    new Order { OrderId = "ORD016", StaffId = "NV0001", CustomerId = "KH0016", PaymentMethod = PaymentType.Cash, Discount = 0, TotalPrice = 115000, CreatedDate = DateTime.Now.AddDays(-15) },
                    new Order { OrderId = "ORD017", StaffId = "NV0003", CustomerId = "KH0017", PaymentMethod = PaymentType.Card, Discount = 20000, TotalPrice = 275000, CreatedDate = DateTime.Now.AddDays(-14) },
                    new Order { OrderId = "ORD018", StaffId = "NV0002", CustomerId = "KH0018", PaymentMethod = PaymentType.DebitCard, Discount = 30000, TotalPrice = 410000, CreatedDate = DateTime.Now.AddDays(-13) },
                    new Order { OrderId = "ORD019", StaffId = "NV0004", CustomerId = "KH0019", PaymentMethod = PaymentType.Cash, Discount = 0, TotalPrice = 89000, CreatedDate = DateTime.Now.AddDays(-12) },
                    new Order { OrderId = "ORD020", StaffId = "NV0005", CustomerId = "KH0020", PaymentMethod = PaymentType.BankTransfer, Discount = 10000, TotalPrice = 165000, CreatedDate = DateTime.Now.AddDays(-11) },
                    new Order { OrderId = "ORD021", StaffId = "NV0001", CustomerId = "KH0021", PaymentMethod = PaymentType.Card, Discount = 30000, TotalPrice = 540000, CreatedDate = DateTime.Now.AddDays(-10) },
                    new Order { OrderId = "ORD022", StaffId = "NV0003", CustomerId = "KH0022", PaymentMethod = PaymentType.Cash, Discount = 0, TotalPrice = 102000, CreatedDate = DateTime.Now.AddDays(-9) },
                    new Order { OrderId = "ORD023", StaffId = "NV0002", CustomerId = "KH0023", PaymentMethod = PaymentType.BankTransfer, Discount = 20000, TotalPrice = 245000, CreatedDate = DateTime.Now.AddDays(-8) },
                    new Order { OrderId = "ORD024", StaffId = "NV0004", CustomerId = "KH0024", PaymentMethod = PaymentType.DebitCard, Discount = 10000, TotalPrice = 320000, CreatedDate = DateTime.Now.AddDays(-7) },
                    new Order { OrderId = "ORD025", StaffId = "NV0005", CustomerId = "KH0025", PaymentMethod = PaymentType.Card, Discount = 30000, TotalPrice = 610000, CreatedDate = DateTime.Now.AddDays(-6) },
                    new Order { OrderId = "ORD026", StaffId = "NV0001", CustomerId = "KH0026", PaymentMethod = PaymentType.Cash, Discount = 0, TotalPrice = 118000, CreatedDate = DateTime.Now.AddDays(-5) },
                    new Order { OrderId = "ORD027", StaffId = "NV0003", CustomerId = "KH0027", PaymentMethod = PaymentType.BankTransfer, Discount = 20000, TotalPrice = 255000, CreatedDate = DateTime.Now.AddDays(-4) },
                    new Order { OrderId = "ORD028", StaffId = "NV0002", CustomerId = "KH0028", PaymentMethod = PaymentType.DebitCard, Discount = 30000, TotalPrice = 460000, CreatedDate = DateTime.Now.AddDays(-3) },
                    new Order { OrderId = "ORD029", StaffId = "NV0004", CustomerId = "KH0029", PaymentMethod = PaymentType.Card, Discount = 10000, TotalPrice = 230000, CreatedDate = DateTime.Now.AddDays(-2) },
                    new Order { OrderId = "ORD030", StaffId = "NV0005", CustomerId = "KH0030", PaymentMethod = PaymentType.Cash, Discount = 0, TotalPrice = 145000, CreatedDate = DateTime.Now.AddDays(-1) }
                };

                context.Orders.AddOrUpdate(o => o.OrderId, orders.ToArray());
                context.SaveChanges();
                #endregion

                #region Order Details
                // ============================================
                // Order Details
                // ============================================
                var orderDetails = new List<OrderDetail>
                {
                    new OrderDetail { OrderId = "ORD001", BookId = "S00001", SalePrice = 80000, Quantity = 2, Subtotal = 160000 },
                    new OrderDetail { OrderId = "ORD002", BookId = "S00002", SalePrice = 75000, Quantity = 3, Subtotal = 225000 },
                    new OrderDetail { OrderId = "ORD003", BookId = "S00003", SalePrice = 50000, Quantity = 2, Subtotal = 100000 },
                    new OrderDetail { OrderId = "ORD004", BookId = "S00004", SalePrice = 65000, Quantity = 3, Subtotal = 195000 },
                    new OrderDetail { OrderId = "ORD005", BookId = "S00005", SalePrice = 95000, Quantity = 5, Subtotal = 475000 },
                    new OrderDetail { OrderId = "ORD006", BookId = "S00006", SalePrice = 120000, Quantity = 1, Subtotal = 120000 },
                    new OrderDetail { OrderId = "ORD007", BookId = "S00007", SalePrice = 135000, Quantity = 2, Subtotal = 270000 },
                    new OrderDetail { OrderId = "ORD008", BookId = "S00008", SalePrice = 90000, Quantity = 4, Subtotal = 360000 },
                    new OrderDetail { OrderId = "ORD009", BookId = "S00009", SalePrice = 60000, Quantity = 1, Subtotal = 60000 },
                    new OrderDetail { OrderId = "ORD010", BookId = "S00010", SalePrice = 70000, Quantity = 2, Subtotal = 140000 },
                    new OrderDetail { OrderId = "ORD011", BookId = "S00011", SalePrice = 110000, Quantity = 5, Subtotal = 550000 },
                    new OrderDetail { OrderId = "ORD012", BookId = "S00012", SalePrice = 150000, Quantity = 1, Subtotal = 150000 },
                    new OrderDetail { OrderId = "ORD013", BookId = "S00013", SalePrice = 180000, Quantity = 2, Subtotal = 360000 },
                    new OrderDetail { OrderId = "ORD014", BookId = "S00014", SalePrice = 95000, Quantity = 3, Subtotal = 285000 },
                    new OrderDetail { OrderId = "ORD015", BookId = "S00015", SalePrice = 55000, Quantity = 4, Subtotal = 220000 },
                    new OrderDetail { OrderId = "ORD016", BookId = "S00016", SalePrice = 125000, Quantity = 1, Subtotal = 125000 },
                    new OrderDetail { OrderId = "ORD017", BookId = "S00017", SalePrice = 160000, Quantity = 2, Subtotal = 320000 },
                    new OrderDetail { OrderId = "ORD018", BookId = "S00018", SalePrice = 50000, Quantity = 3, Subtotal = 150000 },
                    new OrderDetail { OrderId = "ORD019", BookId = "S00019", SalePrice = 170000, Quantity = 1, Subtotal = 170000 },
                    new OrderDetail { OrderId = "ORD020", BookId = "S00020", SalePrice = 145000, Quantity = 2, Subtotal = 290000 },
                    new OrderDetail { OrderId = "ORD021", BookId = "S00021", SalePrice = 130000, Quantity = 4, Subtotal = 520000 },
                    new OrderDetail { OrderId = "ORD022", BookId = "S00022", SalePrice = 115000, Quantity = 1, Subtotal = 115000 },
                    new OrderDetail { OrderId = "ORD023", BookId = "S00023", SalePrice = 190000, Quantity = 2, Subtotal = 380000 },
                    new OrderDetail { OrderId = "ORD024", BookId = "S00024", SalePrice = 140000, Quantity = 2, Subtotal = 280000 },
                    new OrderDetail { OrderId = "ORD025", BookId = "S00025", SalePrice = 45000, Quantity = 3, Subtotal = 135000 },
                    new OrderDetail { OrderId = "ORD026", BookId = "S00026", SalePrice = 88000, Quantity = 2, Subtotal = 176000 },
                    new OrderDetail { OrderId = "ORD027", BookId = "S00027", SalePrice = 105000, Quantity = 2, Subtotal = 210000 },
                    new OrderDetail { OrderId = "ORD028", BookId = "S00028", SalePrice = 68000, Quantity = 4, Subtotal = 272000 },
                    new OrderDetail { OrderId = "ORD029", BookId = "S00029", SalePrice = 125000, Quantity = 2, Subtotal = 250000 },
                    new OrderDetail { OrderId = "ORD030", BookId = "S00030", SalePrice = 99000, Quantity = 1, Subtotal = 99000 }
                };

                context.OrderDetails.AddOrUpdate(
                    od => new { od.OrderId, od.BookId },
                    orderDetails.ToArray());
                context.SaveChanges();
                #endregion

                #region Users
                // ============================================
                // Users (gắn Staff)
                // ============================================
                var users = new List<User>
                {
                    new User {  Username = "admin", PasswordHash = Encryptor.Hash("Admin@123"), StaffId = "NV0001", CreatedDate = DateTime.Now },
                    new User {  Username = "cust.manager", PasswordHash = Encryptor.Hash("CustManager@123"), StaffId = "NV0002", CreatedDate = DateTime.Now },
                    new User { Username = "sales.staff01", PasswordHash = Encryptor.Hash("SalesStaff@123"), StaffId = "NV0003", CreatedDate = DateTime.Now },
                    new User {  Username = "sales.staff02", PasswordHash = Encryptor.Hash("SalesStaff@123"), StaffId = "NV0004", CreatedDate = DateTime.Now },
                    new User {  Username = "inventory.manager01", PasswordHash = Encryptor.Hash("Inventory@123"), StaffId = "NV0005", CreatedDate = DateTime.Now },
                    new User {  Username = "sales.staff03", PasswordHash = Encryptor.Hash("SalesStaff@123"), StaffId = "NV0006", CreatedDate = DateTime.Now },
                    new User {  Username = "sales.staff04", PasswordHash = Encryptor.Hash("SalesStaff@123"), StaffId = "NV0007", CreatedDate = DateTime.Now },
                    new User {  Username = "inventory.manager02", PasswordHash = Encryptor.Hash("Inventory@123"), StaffId = "NV0008", CreatedDate = DateTime.Now },
                    new User {  Username = "sales.staff05", PasswordHash = Encryptor.Hash("SalesStaff@123"), StaffId = "NV0009", CreatedDate = DateTime.Now },
                    new User {  Username = "sales.staff06", PasswordHash = Encryptor.Hash("SalesStaff@123"), StaffId = "NV0010", CreatedDate = DateTime.Now },
                    new User {  Username = "cust.manager02", PasswordHash = Encryptor.Hash("CustManager@123"), StaffId = "NV0012", CreatedDate = DateTime.Now },
                    new User {  Username = "sales.staff08", PasswordHash = Encryptor.Hash("SalesStaff@123"), StaffId = "NV0013", CreatedDate = DateTime.Now },
                    new User { Username = "inventory.manager03", PasswordHash = Encryptor.Hash("Inventory@123"), StaffId = "NV0014", CreatedDate = DateTime.Now },
                    new User { Username = "sales.staff09", PasswordHash = Encryptor.Hash("SalesStaff@123"), StaffId = "NV0015", CreatedDate = DateTime.Now },
                    new User {  Username = "sales.staff10", PasswordHash = Encryptor.Hash("SalesStaff@123"), StaffId = "NV0016", CreatedDate = DateTime.Now },
                    new User { Username = "inventory.manager04", PasswordHash = Encryptor.Hash("Inventory@123"), StaffId = "NV0017", CreatedDate = DateTime.Now },
                    new User {  Username = "sales.staff11", PasswordHash = Encryptor.Hash("SalesStaff@123"), StaffId = "NV0018", CreatedDate = DateTime.Now },
                    new User {  Username = "sales.staff12", PasswordHash = Encryptor.Hash("SalesStaff@123"), StaffId = "NV0019", CreatedDate = DateTime.Now },
                    new User {  Username = "inventory.manager05", PasswordHash = Encryptor.Hash("Inventory@123"), StaffId = "NV0020", CreatedDate = DateTime.Now },
                    new User {  Username = "sales.staff13", PasswordHash = Encryptor.Hash("SalesStaff@123"), StaffId = "NV0021", CreatedDate = DateTime.Now },
                    new User {  Username = "sales.staff14", PasswordHash = Encryptor.Hash("SalesStaff@123"), StaffId = "NV0022", CreatedDate = DateTime.Now },
                    new User {  Username = "sales.staff15", PasswordHash = Encryptor.Hash("SalesStaff@123"), StaffId = "NV0023", CreatedDate = DateTime.Now },
                    new User {  Username = "cust.manager03", PasswordHash = Encryptor.Hash("CustManager@123"), StaffId = "NV0024", CreatedDate = DateTime.Now },
                    new User {  Username = "sales.staff16", PasswordHash = Encryptor.Hash("SalesStaff@123"), StaffId = "NV0025", CreatedDate = DateTime.Now },
                    new User {  Username = "sales.staff17", PasswordHash = Encryptor.Hash("SalesStaff@123"), StaffId = "NV0026", CreatedDate = DateTime.Now },
                    new User {  Username = "inventory.manager06", PasswordHash = Encryptor.Hash("Inventory@123"), StaffId = "NV0027", CreatedDate = DateTime.Now },
                    new User {  Username = "sales.staff18", PasswordHash = Encryptor.Hash("SalesStaff@123"), StaffId = "NV0028", CreatedDate = DateTime.Now },
                    new User {  Username = "sales.staff19", PasswordHash = Encryptor.Hash("SalesStaff@123"), StaffId = "NV0029", CreatedDate = DateTime.Now },
                    new User {  Username = "admin2", PasswordHash = Encryptor.Hash("Admin@123"), StaffId = "NV0030", CreatedDate = DateTime.Now }
                };

                context.Users.AddOrUpdate(u => u.Username, users.ToArray());
                context.SaveChanges();

                #endregion

                // AuditLogs mẫu 
                // context.AuditLogs.AddOrUpdate(al => al.Id, new AuditLog { ... });
                // context.SaveChanges();

                Console.WriteLine("\n✅ All seed data inserted successfully!");
                base.Seed(context);
            }
        }
    }