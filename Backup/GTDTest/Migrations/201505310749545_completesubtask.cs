namespace GTDTest.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class completesubtask : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SubTask", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SubTask", "IsDeleted");
        }
    }
}
