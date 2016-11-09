namespace GTD.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPomodoro : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Pomodoro",
                c => new
                    {
                        PomodoroId = c.Int(nullable: false, identity: true),
                        IsCompletedPomodoro = c.Boolean(nullable: false),
                        StarDateTime = c.DateTime(nullable: false),
                        EnDateTime = c.DateTime(),
                        IsWorkingTime = c.Boolean(nullable: false),
                        TaskId = c.Int(),
                    })
                .PrimaryKey(t => t.PomodoroId)
                .ForeignKey("dbo.Task", t => t.TaskId)
                .Index(t => t.TaskId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Pomodoro", "TaskId", "dbo.Task");
            DropIndex("dbo.Pomodoro", new[] { "TaskId" });
            DropTable("dbo.Pomodoro");
        }
    }
}
