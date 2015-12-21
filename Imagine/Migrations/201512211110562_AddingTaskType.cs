namespace Imagine.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingTaskType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TaskEntities", "TaskType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TaskEntities", "TaskType");
        }
    }
}
