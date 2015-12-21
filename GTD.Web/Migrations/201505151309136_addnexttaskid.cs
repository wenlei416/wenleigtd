using System.Data.Entity.Migrations;

namespace GTD.Migrations
{
    public partial class addnexttaskid : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Task", "NextTask_TaskId", "dbo.Task");
            DropIndex("dbo.Task", new[] { "NextTask_TaskId" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Task", "NextTask_TaskId");
            AddForeignKey("dbo.Task", "NextTask_TaskId", "dbo.Task", "TaskId");
        }
    }
}
