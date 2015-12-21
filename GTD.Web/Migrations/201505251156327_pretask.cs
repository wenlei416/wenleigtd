namespace GTDTest.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pretask : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Task", "PreviousTask_TaskId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Task", "PreviousTask_TaskId");
        }
    }
}
