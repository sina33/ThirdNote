namespace ThirdNote.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Notes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Text = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        Markdown = c.Boolean(nullable: false),
                        Hidden = c.Boolean(nullable: false),
                        Pin = c.Boolean(nullable: false),
                        Actionable = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                        ViewCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.NoteTags",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        NoteID = c.Int(nullable: false),
                        TagID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Notes", t => t.NoteID, cascadeDelete: true)
                .ForeignKey("dbo.Tags", t => t.TagID, cascadeDelete: true)
                .Index(t => t.NoteID)
                .Index(t => t.TagID);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Label = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NoteTags", "TagID", "dbo.Tags");
            DropForeignKey("dbo.NoteTags", "NoteID", "dbo.Notes");
            DropIndex("dbo.NoteTags", new[] { "TagID" });
            DropIndex("dbo.NoteTags", new[] { "NoteID" });
            DropTable("dbo.Tags");
            DropTable("dbo.NoteTags");
            DropTable("dbo.Notes");
        }
    }
}
