namespace bookstore_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserPrimeKeyToUserName : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Users");
            AddPrimaryKey("dbo.Users", "username");
            DropColumn("dbo.Users", "user_id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "user_id", c => c.String(nullable: false, maxLength: 10));
            DropPrimaryKey("dbo.Users");
            AddPrimaryKey("dbo.Users", "user_id");
        }
    }
}
