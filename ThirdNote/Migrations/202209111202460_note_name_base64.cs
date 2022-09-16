namespace ThirdNote.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class note_name_base64 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notes", "Name", c => c.String(maxLength: 8));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Notes", "Name");
        }
    }
}
