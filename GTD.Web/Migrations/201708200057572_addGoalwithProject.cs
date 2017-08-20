namespace GTD.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addGoalwithProject : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Project", name: "Goal_GoalId", newName: "GoalId");
            RenameIndex(table: "dbo.Project", name: "IX_Goal_GoalId", newName: "IX_GoalId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Project", name: "IX_GoalId", newName: "IX_Goal_GoalId");
            RenameColumn(table: "dbo.Project", name: "GoalId", newName: "Goal_GoalId");
        }
    }
}
