namespace ThirdNote.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class note_handle : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Notes", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Notes", "Name", c => c.String(maxLength: 8));
        }
    }
}
