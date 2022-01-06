namespace ThirdNote.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WrittenDate5 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Notes", "WrittenDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Notes", "WrittenDate", c => c.DateTime());
        }
    }
}
