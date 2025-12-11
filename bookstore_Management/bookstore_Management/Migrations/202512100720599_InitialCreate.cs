namespace bookstore_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuditLogs",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        entity_name = c.String(nullable: false, maxLength: 50),
                        entity_id = c.String(nullable: false, maxLength: 50),
                        action = c.String(nullable: false, maxLength: 20),
                        old_values = c.String(),
                        new_values = c.String(),
                        changed_by = c.String(nullable: false, maxLength: 6),
                        changed_date = c.DateTime(nullable: false),
                        description = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.id)
                .Index(t => t.entity_name)
                .Index(t => new { t.entity_name, t.entity_id })
                .Index(t => t.entity_id)
                .Index(t => t.changed_date);
            
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 6),
                        name = c.String(nullable: false, maxLength: 50),
                        supplier_id = c.String(nullable: false, maxLength: 6),
                        category = c.Int(nullable: false),
                        sale_price = c.Decimal(precision: 12, scale: 2),
                        import_price = c.Decimal(nullable: false, precision: 12, scale: 2),
                        created_date = c.DateTime(nullable: false),
                        updated_date = c.DateTime(),
                        deleted_date = c.DateTime(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Suppliers", t => t.supplier_id)
                .Index(t => t.supplier_id)
                .Index(t => t.deleted_date);
            
            CreateTable(
                "dbo.OrderDetails",
                c => new
                    {
                        book_id = c.String(nullable: false, maxLength: 6),
                        order_id = c.String(nullable: false, maxLength: 6),
                        sale_price = c.Decimal(nullable: false, precision: 12, scale: 2),
                        quantity = c.Int(nullable: false),
                        subtotal = c.Decimal(nullable: false, precision: 12, scale: 2),
                        note = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => new { t.book_id, t.order_id })
                .ForeignKey("dbo.Books", t => t.book_id)
                .ForeignKey("dbo.Orders", t => t.order_id, cascadeDelete: true)
                .Index(t => t.book_id)
                .Index(t => t.order_id);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 6),
                        staff_id = c.String(nullable: false, maxLength: 6),
                        customer_id = c.String(maxLength: 6),
                        payment_method = c.Int(nullable: false),
                        discount = c.Decimal(nullable: false, precision: 12, scale: 2),
                        total_price = c.Decimal(nullable: false, precision: 12, scale: 2),
                        note = c.String(maxLength: 500),
                        created_date = c.DateTime(nullable: false),
                        updated_date = c.DateTime(),
                        deleted_date = c.DateTime(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Customers", t => t.customer_id)
                .ForeignKey("dbo.Staffs", t => t.staff_id, cascadeDelete: true)
                .Index(t => t.staff_id)
                .Index(t => t.customer_id)
                .Index(t => t.created_date);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 6),
                        name = c.String(nullable: false, maxLength: 50),
                        phone = c.String(nullable: false, maxLength: 20),
                        email = c.String(maxLength: 100),
                        address = c.String(maxLength: 200),
                        loyalty_points = c.Decimal(nullable: false, precision: 12, scale: 2),
                        member_level = c.Int(nullable: false),
                        created_date = c.DateTime(nullable: false),
                        updated_date = c.DateTime(),
                        deleted_date = c.DateTime(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Staffs",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 6),
                        name = c.String(nullable: false, maxLength: 50),
                        base_salary = c.Decimal(nullable: false, precision: 12, scale: 2),
                        citizen_id_card = c.String(nullable: false, maxLength: 12),
                        phone = c.String(nullable: false, maxLength: 20),
                        status = c.Int(nullable: false),
                        role = c.Int(nullable: false),
                        salary_rate = c.Decimal(nullable: false, precision: 12, scale: 2),
                        created_date = c.DateTime(nullable: false),
                        updated_date = c.DateTime(),
                        deleted_date = c.DateTime(),
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
                "dbo.Stocks",
                c => new
                    {
                        book_id = c.String(nullable: false, maxLength: 6),
                        quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.book_id)
                .ForeignKey("dbo.Books", t => t.book_id)
                .Index(t => t.book_id, unique: true);
            
            CreateTable(
                "dbo.Suppliers",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 6),
                        name = c.String(nullable: false, maxLength: 50),
                        phone = c.String(nullable: false, maxLength: 20),
                        address = c.String(maxLength: 200),
                        email = c.String(maxLength: 100),
                        created_date = c.DateTime(nullable: false),
                        updated_date = c.DateTime(),
                        deleted_date = c.DateTime(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.ImportBills",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        code = c.String(nullable: false, maxLength: 20),
                        import_date = c.DateTime(nullable: false),
                        supplier_id = c.String(nullable: false, maxLength: 6),
                        total_amount = c.Decimal(nullable: false, precision: 12, scale: 2),
                        notes = c.String(maxLength: 500),
                        created_by = c.String(nullable: false, maxLength: 6),
                        created_date = c.DateTime(nullable: false),
                        updated_date = c.DateTime(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Suppliers", t => t.supplier_id)
                .Index(t => t.supplier_id);
            
            CreateTable(
                "dbo.ImportBillDetails",
                c => new
                    {
                        book_id = c.String(nullable: false, maxLength: 6),
                        import_id = c.Int(nullable: false),
                        quantity = c.Int(nullable: false),
                        import_price = c.Decimal(nullable: false, precision: 12, scale: 2),
                        total_price = c.Decimal(nullable: false, precision: 12, scale: 2),
                    })
                .PrimaryKey(t => new { t.book_id, t.import_id })
                .ForeignKey("dbo.Books", t => t.book_id)
                .ForeignKey("dbo.ImportBills", t => t.import_id, cascadeDelete: true)
                .Index(t => t.book_id)
                .Index(t => t.import_id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        user_id = c.String(nullable: false, maxLength: 10),
                        username = c.String(nullable: false, maxLength: 50),
                        password_hash = c.String(nullable: false, maxLength: 255),
                        email = c.String(maxLength: 100),
                        role = c.Int(nullable: false),
                        created_date = c.DateTime(nullable: false),
                        updated_date = c.DateTime(),
                        deleted_date = c.DateTime(),
                    })
                .PrimaryKey(t => t.user_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Books", "supplier_id", "dbo.Suppliers");
            DropForeignKey("dbo.ImportBills", "supplier_id", "dbo.Suppliers");
            DropForeignKey("dbo.ImportBillDetails", "import_id", "dbo.ImportBills");
            DropForeignKey("dbo.ImportBillDetails", "book_id", "dbo.Books");
            DropForeignKey("dbo.Stocks", "book_id", "dbo.Books");
            DropForeignKey("dbo.OrderDetails", "order_id", "dbo.Orders");
            DropForeignKey("dbo.Orders", "staff_id", "dbo.Staffs");
            DropForeignKey("dbo.StaffDailyRevenues", "employee_id", "dbo.Staffs");
            DropForeignKey("dbo.Orders", "customer_id", "dbo.Customers");
            DropForeignKey("dbo.OrderDetails", "book_id", "dbo.Books");
            DropIndex("dbo.ImportBillDetails", new[] { "import_id" });
            DropIndex("dbo.ImportBillDetails", new[] { "book_id" });
            DropIndex("dbo.ImportBills", new[] { "supplier_id" });
            DropIndex("dbo.Stocks", new[] { "book_id" });
            DropIndex("dbo.StaffDailyRevenues", new[] { "employee_id" });
            DropIndex("dbo.Orders", new[] { "created_date" });
            DropIndex("dbo.Orders", new[] { "customer_id" });
            DropIndex("dbo.Orders", new[] { "staff_id" });
            DropIndex("dbo.OrderDetails", new[] { "order_id" });
            DropIndex("dbo.OrderDetails", new[] { "book_id" });
            DropIndex("dbo.Books", new[] { "deleted_date" });
            DropIndex("dbo.Books", new[] { "supplier_id" });
            DropIndex("dbo.AuditLogs", new[] { "changed_date" });
            DropIndex("dbo.AuditLogs", new[] { "entity_id" });
            DropIndex("dbo.AuditLogs", new[] { "entity_name", "entity_id" });
            DropIndex("dbo.AuditLogs", new[] { "entity_name" });
            DropTable("dbo.Users");
            DropTable("dbo.ImportBillDetails");
            DropTable("dbo.ImportBills");
            DropTable("dbo.Suppliers");
            DropTable("dbo.Stocks");
            DropTable("dbo.StaffDailyRevenues");
            DropTable("dbo.Staffs");
            DropTable("dbo.Customers");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderDetails");
            DropTable("dbo.Books");
            DropTable("dbo.AuditLogs");
        }
    }
}
