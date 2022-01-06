namespace ThirdNote.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WrittenDate1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Notes", "WrittenDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Notes", "WrittenDate", c => c.DateTime(nullable: false));
        }
    }
}
