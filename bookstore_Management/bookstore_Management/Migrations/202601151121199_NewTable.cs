namespace bookstore_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewTable : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Suppliers", newName: "Publishers");
            RenameColumn(table: "dbo.ImportBills", name: "supplier_id", newName: "publisher_id");
            RenameColumn(table: "dbo.Books", name: "supplier_id", newName: "publisher_id");
            RenameIndex(table: "dbo.Books", name: "IX_supplier_id", newName: "IX_publisher_id");
            RenameIndex(table: "dbo.ImportBills", name: "IX_supplier_id", newName: "IX_publisher_id");
            AddColumn("dbo.Customers", "email", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.Customers", "address", c => c.String(nullable: false, maxLength: 250));
            AddColumn("dbo.Staffs", "phone", c => c.String(nullable: false, maxLength: 10));
            AddColumn("dbo.Staffs", "citizen_id", c => c.String(nullable: false, maxLength: 10));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Staffs", "citizen_id");
            DropColumn("dbo.Staffs", "phone");
            DropColumn("dbo.Customers", "address");
            DropColumn("dbo.Customers", "email");
            RenameIndex(table: "dbo.ImportBills", name: "IX_publisher_id", newName: "IX_supplier_id");
            RenameIndex(table: "dbo.Books", name: "IX_publisher_id", newName: "IX_supplier_id");
            RenameColumn(table: "dbo.Books", name: "publisher_id", newName: "supplier_id");
            RenameColumn(table: "dbo.ImportBills", name: "publisher_id", newName: "supplier_id");
            RenameTable(name: "dbo.Publishers", newName: "Suppliers");
        }
    }
}
