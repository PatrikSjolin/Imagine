namespace Imagine.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingLocked : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScheduledTasks", "Locked", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScheduledTasks", "Locked");
        }
    }
}
