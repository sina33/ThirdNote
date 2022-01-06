namespace ThirdNote.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeActionables : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Notes", "Actionable");
            DropColumn("dbo.Notes", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Notes", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.Notes", "Actionable", c => c.Boolean(nullable: false));
        }
    }
}
