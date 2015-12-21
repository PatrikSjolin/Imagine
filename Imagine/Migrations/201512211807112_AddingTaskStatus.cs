namespace Imagine.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingTaskStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScheduledTasks", "TaskStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScheduledTasks", "TaskStatus");
        }
    }
}
