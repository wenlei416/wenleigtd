namespace GTDTest.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnexttask : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Task", "NextTask_TaskId", c => c.Int());
            AddForeignKey("dbo.Task", "NextTask_TaskId", "dbo.Task", "TaskId");
            CreateIndex("dbo.Task", "NextTask_TaskId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Task", new[] { "NextTask_TaskId" });
            DropForeignKey("dbo.Task", "NextTask_TaskId", "dbo.Task");
            DropColumn("dbo.Task", "NextTask_TaskId");
        }
    }
}
