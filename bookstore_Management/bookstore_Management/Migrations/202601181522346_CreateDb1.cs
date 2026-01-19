namespace bookstore_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateDb1 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Books", "deleted_date", name: "IX_Book_DeletedDate");
            CreateIndex("dbo.Publishers", "name", name: "IX_Publishers_Name");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Publishers", "IX_Publishers_Name");
            DropIndex("dbo.Books", "IX_Book_DeletedDate");
        }
    }
}
