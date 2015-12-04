namespace Imagine.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingSchedule : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TaskEntities", "DueDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TaskEntities", "DueDate");
        }
    }
}
