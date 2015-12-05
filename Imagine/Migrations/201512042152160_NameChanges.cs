namespace Imagine.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NameChanges : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Schedules", newName: "ScheduledTasks");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.ScheduledTasks", newName: "Schedules");
        }
    }
}
