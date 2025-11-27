namespace bookstore_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 6),
                        name = c.String(nullable: false, maxLength: 50),
                        Supplier_id = c.String(nullable: false, maxLength: 6),
                        category = c.Int(nullable: false),
                        sale_price = c.Decimal(precision: 12, scale: 2),
                        import_price = c.Decimal(nullable: false, precision: 12, scale: 2),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Suppliers", t => t.Supplier_id, cascadeDelete: true)
                .Index(t => t.Supplier_id);
            
            CreateTable(
                "dbo.OrderDetails",
                c => new
                    {
                        book_id = c.String(nullable: false, maxLength: 6),
                        Order_id = c.String(nullable: false, maxLength: 6),
                        sale_price = c.Decimal(nullable: false, precision: 12, scale: 2),
                        quantity = c.Int(nullable: false),
                        note = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => new { t.book_id, t.Order_id })
                .ForeignKey("dbo.Books", t => t.book_id, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.Order_id, cascadeDelete: true)
                .Index(t => t.book_id)
                .Index(t => t.Order_id);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        order_id = c.String(nullable: false, maxLength: 6),
                        staff_id = c.String(nullable: false, maxLength: 6),
                        customer_id = c.String(maxLength: 6),
                        payment_method = c.Int(nullable: false),
                        discount = c.Decimal(nullable: false, precision: 12, scale: 2),
                        total_price = c.Decimal(nullable: false, precision: 12, scale: 2),
                        note = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.order_id)
                .ForeignKey("dbo.Customers", t => t.customer_id)
                .ForeignKey("dbo.Staffs", t => t.staff_id, cascadeDelete: true)
                .Index(t => t.staff_id)
                .Index(t => t.customer_id);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 6),
                        name = c.String(nullable: false, maxLength: 50),
                        phone = c.String(nullable: false, maxLength: 30),
                        email = c.String(maxLength: 100),
                        address = c.String(maxLength: 200),
                        loyalty_points = c.Decimal(nullable: false, precision: 18, scale: 2),
                        member_level = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Staffs",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 6),
                        name = c.String(nullable: false, maxLength: 50),
                        base_salary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        citizen_id_card = c.String(nullable: false, maxLength: 10),
                        phone = c.String(nullable: false, maxLength: 10),
                        address = c.String(maxLength: 50),
                        status = c.Int(nullable: false),
                        role = c.Int(nullable: false),
                        salary_rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.StaffDailyRevenues",
                c => new
                    {
                        employee_id = c.String(nullable: false, maxLength: 6),
                        day = c.DateTime(nullable: false),
                        revenue = c.Decimal(nullable: false, precision: 12, scale: 2),
                    })
                .PrimaryKey(t => new { t.employee_id, t.day })
                .ForeignKey("dbo.Staffs", t => t.employee_id, cascadeDelete: true)
                .Index(t => t.employee_id);
            
            CreateTable(
                "dbo.Suppliers",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 6),
                        name = c.String(nullable: false, maxLength: 50),
                        phone = c.String(nullable: false, maxLength: 30),
                        address = c.String(),
                        email = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 6),
                        Username = c.String(nullable: false, maxLength: 50),
                        PasswordHash = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Books", "Supplier_id", "dbo.Suppliers");
            DropForeignKey("dbo.OrderDetails", "Order_id", "dbo.Orders");
            DropForeignKey("dbo.Orders", "staff_id", "dbo.Staffs");
            DropForeignKey("dbo.StaffDailyRevenues", "employee_id", "dbo.Staffs");
            DropForeignKey("dbo.Orders", "customer_id", "dbo.Customers");
            DropForeignKey("dbo.OrderDetails", "book_id", "dbo.Books");
            DropIndex("dbo.StaffDailyRevenues", new[] { "employee_id" });
            DropIndex("dbo.Orders", new[] { "customer_id" });
            DropIndex("dbo.Orders", new[] { "staff_id" });
            DropIndex("dbo.OrderDetails", new[] { "Order_id" });
            DropIndex("dbo.OrderDetails", new[] { "book_id" });
            DropIndex("dbo.Books", new[] { "Supplier_id" });
            DropTable("dbo.Users");
            DropTable("dbo.Suppliers");
            DropTable("dbo.StaffDailyRevenues");
            DropTable("dbo.Staffs");
            DropTable("dbo.Customers");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderDetails");
            DropTable("dbo.Books");
        }
    }
}
