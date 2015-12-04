namespace Imagine.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifiedNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TaskEntities", "Modified", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TaskEntities", "Modified", c => c.DateTime(nullable: false));
        }
    }
}
