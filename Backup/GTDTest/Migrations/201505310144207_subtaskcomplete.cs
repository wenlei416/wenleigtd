namespace GTDTest.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class subtaskcomplete : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SubTask", "IsComplete", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SubTask", "IsComplete");
        }
    }
}
