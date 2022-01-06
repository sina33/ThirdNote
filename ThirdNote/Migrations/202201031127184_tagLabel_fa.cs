namespace ThirdNote.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tagLabel_fa : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tags", "Lable_fa", c => c.String());
            
            AddColumn("dbo.Tags", "Lable_en", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tags", "Lable_en");
            DropColumn("dbo.Tags", "Lable_fa");
        }
    }
}
