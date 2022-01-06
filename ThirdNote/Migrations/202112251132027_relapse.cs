namespace ThirdNote.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class relapse : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Relapses",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Subject = c.String(),
                        Date = c.DateTime(nullable: false),
                        Message = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Relapses");
        }
    }
}
