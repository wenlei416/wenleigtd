namespace GTD.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTaskPropertyRepeatJson : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Task", "RepeatJson", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Task", "RepeatJson");
        }
    }
}
