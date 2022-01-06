namespace ThirdNote.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WrittenDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notes", "WrittenDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Notes", "WrittenDate");
        }
    }
}
