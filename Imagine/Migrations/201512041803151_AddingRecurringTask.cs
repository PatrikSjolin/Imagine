namespace Imagine.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingRecurringTask : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TaskEntities", "Period", c => c.Int());
            AddColumn("dbo.TaskEntities", "Frequency", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TaskEntities", "Frequency");
            DropColumn("dbo.TaskEntities", "Period");
        }
    }
}
