namespace Imagine.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingDuration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TaskEntities", "Duration", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TaskEntities", "Duration");
        }
    }
}
