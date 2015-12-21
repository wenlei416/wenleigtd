using System.Data.Entity.Migrations;

namespace GTD.Migrations
{
    public partial class addComment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comment",
                c => new
                    {
                        CommentId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        TaskId = c.Int(),
                    })
                .PrimaryKey(t => t.CommentId)
                .ForeignKey("dbo.Task", t => t.TaskId)
                .Index(t => t.TaskId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Comment", new[] { "TaskId" });
            DropForeignKey("dbo.Comment", "TaskId", "dbo.Task");
            DropTable("dbo.Comment");
        }
    }
}
