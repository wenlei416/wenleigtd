using System.Data.Entity.Migrations;

namespace GTD.Migrations
{
    public partial class taskdeleted : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Task", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Task", "IsDeleted");
        }
    }
}
