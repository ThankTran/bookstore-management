namespace bookstore_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createDb : DbMigration
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
                        publisher_id = c.String(maxLength: 6),
                        category = c.Int(nullable: false),
                        sale_price = c.Decimal(precision: 18, scale: 2),
                        stock = c.Int(nullable: false),
                        created_date = c.DateTime(nullable: false),
                        updated_date = c.DateTime(),
                        deleted_date = c.DateTime(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Publishers", t => t.publisher_id)
                .Index(t => t.name, name: "IX_Book_Name")
                .Index(t => t.publisher_id)
                .Index(t => t.deleted_date, name: "IX_Book_DeletedDate");
            
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
                .Index(t => t.order_id, name: "IX_OrderDetail_OrderId");
            
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
                .Index(t => t.customer_id)
                .Index(t => t.deleted_date, name: "IX_Order_DeletedDate");
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 6),
                        name = c.String(nullable: false, maxLength: 50),
                        phone = c.String(nullable: false, maxLength: 20),
                        email = c.String(nullable: false, maxLength: 50),
                        address = c.String(nullable: false, maxLength: 250),
                        member_level = c.Int(nullable: false),
                        loyalty_points = c.Decimal(nullable: false, precision: 18, scale: 2),
                        created_date = c.DateTime(nullable: false),
                        updated_date = c.DateTime(),
                        deleted_date = c.DateTime(),
                    })
                .PrimaryKey(t => t.id)
                .Index(t => t.name, name: "IX_Customer_Name")
                .Index(t => t.deleted_date, name: "IX_Customer_DeletedDate");
            
            CreateTable(
                "dbo.Staffs",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 6),
                        name = c.String(nullable: false, maxLength: 50),
                        phone = c.String(nullable: false, maxLength: 10),
                        citizen_id = c.String(nullable: false, maxLength: 12),
                        role = c.Int(nullable: false),
                        created_date = c.DateTime(nullable: false),
                        updated_date = c.DateTime(),
                        deleted_date = c.DateTime(),
                    })
                .PrimaryKey(t => t.id)
                .Index(t => t.name, name: "IX_Staff_Name")
                .Index(t => t.deleted_date, name: "IX_Staff_DeletedDate");
            
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
                .PrimaryKey(t => t.id)
                .Index(t => t.name, name: "IX_Publishers_Name")
                .Index(t => t.deleted_date, name: "IX_Publishers_DeletedDate");
            
            CreateTable(
                "dbo.ImportBills",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 6),
                        publisher_id = c.String(nullable: false, maxLength: 6),
                        total_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        notes = c.String(maxLength: 500),
                        created_by = c.String(nullable: false, maxLength: 6),
                        created_date = c.DateTime(nullable: false),
                        updated_date = c.DateTime(),
                        deleted_date = c.DateTime(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Publishers", t => t.publisher_id)
                .Index(t => t.publisher_id)
                .Index(t => t.deleted_date, name: "IX_ImportBill_DeletedDate");
            
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
                .Index(t => t.import_id, name: "IX_ImportBillDetail_ImportId")
                .Index(t => t.deleted_date, name: "IX_ImportBillDetail_DeletedDate");
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        user_id = c.String(nullable: false, maxLength: 10),
                        username = c.String(nullable: false, maxLength: 50),
                        password_hash = c.String(nullable: false, maxLength: 255),
                        staff_id = c.String(nullable: false, maxLength: 6),
                        role = c.Int(nullable: false),
                        created_date = c.DateTime(nullable: false),
                        updated_date = c.DateTime(),
                        deleted_date = c.DateTime(),
                    })
                .PrimaryKey(t => t.user_id)
                .ForeignKey("dbo.Staffs", t => t.staff_id)
                .Index(t => t.username, name: "IX_User_Username")
                .Index(t => t.staff_id)
                .Index(t => t.deleted_date, name: "IX_User_DeletedDate");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "staff_id", "dbo.Staffs");
            DropForeignKey("dbo.Books", "publisher_id", "dbo.Publishers");
            DropForeignKey("dbo.ImportBills", "publisher_id", "dbo.Publishers");
            DropForeignKey("dbo.ImportBillDetails", "import_id", "dbo.ImportBills");
            DropForeignKey("dbo.ImportBillDetails", "book_id", "dbo.Books");
            DropForeignKey("dbo.OrderDetails", "order_id", "dbo.Orders");
            DropForeignKey("dbo.Orders", "staff_id", "dbo.Staffs");
            DropForeignKey("dbo.Orders", "customer_id", "dbo.Customers");
            DropForeignKey("dbo.OrderDetails", "book_id", "dbo.Books");
            DropIndex("dbo.Users", "IX_User_DeletedDate");
            DropIndex("dbo.Users", new[] { "staff_id" });
            DropIndex("dbo.Users", "IX_User_Username");
            DropIndex("dbo.ImportBillDetails", "IX_ImportBillDetail_DeletedDate");
            DropIndex("dbo.ImportBillDetails", "IX_ImportBillDetail_ImportId");
            DropIndex("dbo.ImportBillDetails", new[] { "book_id" });
            DropIndex("dbo.ImportBills", "IX_ImportBill_DeletedDate");
            DropIndex("dbo.ImportBills", new[] { "publisher_id" });
            DropIndex("dbo.Publishers", "IX_Publishers_DeletedDate");
            DropIndex("dbo.Publishers", "IX_Publishers_Name");
            DropIndex("dbo.Staffs", "IX_Staff_DeletedDate");
            DropIndex("dbo.Staffs", "IX_Staff_Name");
            DropIndex("dbo.Customers", "IX_Customer_DeletedDate");
            DropIndex("dbo.Customers", "IX_Customer_Name");
            DropIndex("dbo.Orders", "IX_Order_DeletedDate");
            DropIndex("dbo.Orders", new[] { "customer_id" });
            DropIndex("dbo.Orders", new[] { "staff_id" });
            DropIndex("dbo.OrderDetails", "IX_OrderDetail_OrderId");
            DropIndex("dbo.OrderDetails", new[] { "book_id" });
            DropIndex("dbo.Books", "IX_Book_DeletedDate");
            DropIndex("dbo.Books", new[] { "publisher_id" });
            DropIndex("dbo.Books", "IX_Book_Name");
            DropTable("dbo.Users");
            DropTable("dbo.ImportBillDetails");
            DropTable("dbo.ImportBills");
            DropTable("dbo.Publishers");
            DropTable("dbo.Staffs");
            DropTable("dbo.Customers");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderDetails");
            DropTable("dbo.Books");
            DropTable("dbo.AuditLogs");
        }
    }
}
