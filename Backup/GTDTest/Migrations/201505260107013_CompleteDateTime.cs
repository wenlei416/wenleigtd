namespace GTDTest.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompleteDateTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Task", "CompleteDateTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Task", "CompleteDateTime");
        }
    }
}
