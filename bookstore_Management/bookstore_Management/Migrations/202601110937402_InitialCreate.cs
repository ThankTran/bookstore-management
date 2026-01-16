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
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 6),
                        name = c.String(nullable: false, maxLength: 50),
                        author = c.String(nullable: false, maxLength: 50),
                        supplier_id = c.String(maxLength: 6),
                        category = c.Int(nullable: false),
                        sale_price = c.Decimal(precision: 18, scale: 2),
                        created_date = c.DateTime(nullable: false),
                        updated_date = c.DateTime(),
                        deleted_date = c.DateTime(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Publishers", t => t.supplier_id)
                .Index(t => t.supplier_id);
            
            CreateTable(
                "dbo.OrderDetails",
                c => new
                    {
                        book_id = c.String(nullable: false, maxLength: 6),
                        order_id = c.String(nullable: false, maxLength: 6),
                        sale_price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        quantity = c.Int(nullable: false),
                        subtotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        note = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => new { t.book_id, t.order_id })
                .ForeignKey("dbo.Books", t => t.book_id)
                .ForeignKey("dbo.Orders", t => t.order_id)
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
                        discount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        total_price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        note = c.String(maxLength: 500),
                        created_date = c.DateTime(nullable: false),
                        updated_date = c.DateTime(),
                        deleted_date = c.DateTime(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Customers", t => t.customer_id)
                .ForeignKey("dbo.Staffs", t => t.staff_id)
                .Index(t => t.staff_id)
                .Index(t => t.customer_id);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 6),
                        name = c.String(nullable: false, maxLength: 50),
                        phone = c.String(nullable: false, maxLength: 20),
                        member_level = c.Int(nullable: false),
                        loyalty_points = c.Decimal(nullable: false, precision: 18, scale: 2),
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
                        role = c.Int(nullable: false),
                        created_date = c.DateTime(nullable: false),
                        updated_date = c.DateTime(),
                        deleted_date = c.DateTime(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Stocks",
                c => new
                    {
                        warehouse_id = c.String(nullable: false, maxLength: 6),
                        book_id = c.String(nullable: false, maxLength: 6),
                        quantity = c.Int(nullable: false),
                        updated_date = c.DateTime(),
                        deleted_date = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.warehouse_id, t.book_id })
                .ForeignKey("dbo.Books", t => t.book_id)
                .ForeignKey("dbo.Warehouses", t => t.warehouse_id)
                .Index(t => t.warehouse_id)
                .Index(t => t.book_id);
            
            CreateTable(
                "dbo.Warehouses",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 6),
                        name = c.String(nullable: false, maxLength: 50),
                        address = c.String(maxLength: 200),
                        created_date = c.DateTime(nullable: false),
                        updated_date = c.DateTime(),
                        deleted_date = c.DateTime(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.ImportBills",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 6),
                        supplier_id = c.String(nullable: false, maxLength: 6),
                        warehouse_id = c.String(nullable: false, maxLength: 6),
                        total_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        notes = c.String(maxLength: 500),
                        created_by = c.String(nullable: false, maxLength: 6),
                        created_date = c.DateTime(nullable: false),
                        updated_date = c.DateTime(),
                        deleted_date = c.DateTime(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Publishers", t => t.supplier_id)
                .ForeignKey("dbo.Warehouses", t => t.warehouse_id)
                .Index(t => t.supplier_id)
                .Index(t => t.warehouse_id);
            
            CreateTable(
                "dbo.ImportBillDetails",
                c => new
                    {
                        book_id = c.String(nullable: false, maxLength: 6),
                        import_id = c.String(nullable: false, maxLength: 6),
                        quantity = c.Int(nullable: false),
                        import_price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        deleted_date = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.book_id, t.import_id })
                .ForeignKey("dbo.Books", t => t.book_id)
                .ForeignKey("dbo.ImportBills", t => t.import_id)
                .Index(t => t.book_id)
                .Index(t => t.import_id);
            
            CreateTable(
                "dbo.Publishers",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 6),
                        name = c.String(nullable: false, maxLength: 50),
                        phone = c.String(nullable: false, maxLength: 20),
                        email = c.String(maxLength: 100),
                        created_date = c.DateTime(nullable: false),
                        updated_date = c.DateTime(),
                        deleted_date = c.DateTime(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        user_id = c.String(nullable: false, maxLength: 10),
                        username = c.String(nullable: false, maxLength: 50),
                        password_hash = c.String(nullable: false, maxLength: 255),
                        staff_id = c.String(maxLength: 100),
                        role = c.Int(nullable: false),
                        created_date = c.DateTime(nullable: false),
                        updated_date = c.DateTime(),
                        deleted_date = c.DateTime(),
                    })
                .PrimaryKey(t => t.user_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Books", "supplier_id", "dbo.Publishers");
            DropForeignKey("dbo.Stocks", "warehouse_id", "dbo.Warehouses");
            DropForeignKey("dbo.ImportBills", "warehouse_id", "dbo.Warehouses");
            DropForeignKey("dbo.ImportBills", "supplier_id", "dbo.Publishers");
            DropForeignKey("dbo.ImportBillDetails", "import_id", "dbo.ImportBills");
            DropForeignKey("dbo.ImportBillDetails", "book_id", "dbo.Books");
            DropForeignKey("dbo.Stocks", "book_id", "dbo.Books");
            DropForeignKey("dbo.OrderDetails", "order_id", "dbo.Orders");
            DropForeignKey("dbo.Orders", "staff_id", "dbo.Staffs");
            DropForeignKey("dbo.Orders", "customer_id", "dbo.Customers");
            DropForeignKey("dbo.OrderDetails", "book_id", "dbo.Books");
            DropIndex("dbo.ImportBillDetails", new[] { "import_id" });
            DropIndex("dbo.ImportBillDetails", new[] { "book_id" });
            DropIndex("dbo.ImportBills", new[] { "warehouse_id" });
            DropIndex("dbo.ImportBills", new[] { "supplier_id" });
            DropIndex("dbo.Stocks", new[] { "book_id" });
            DropIndex("dbo.Stocks", new[] { "warehouse_id" });
            DropIndex("dbo.Orders", new[] { "customer_id" });
            DropIndex("dbo.Orders", new[] { "staff_id" });
            DropIndex("dbo.OrderDetails", new[] { "order_id" });
            DropIndex("dbo.OrderDetails", new[] { "book_id" });
            DropIndex("dbo.Books", new[] { "supplier_id" });
            DropTable("dbo.Users");
            DropTable("dbo.Publishers");
            DropTable("dbo.ImportBillDetails");
            DropTable("dbo.ImportBills");
            DropTable("dbo.Warehouses");
            DropTable("dbo.Stocks");
            DropTable("dbo.Staffs");
            DropTable("dbo.Customers");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderDetails");
            DropTable("dbo.Books");
            DropTable("dbo.AuditLogs");
        }
    }
}
