namespace MeetingSignIn.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Meetings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Theme = c.String(),
                        Owner = c.String(),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        Completed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Members",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Alias = c.String(),
                        Signed = c.Boolean(nullable: false),
                        MeetingId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Meetings", t => t.MeetingId, cascadeDelete: true)
                .Index(t => t.MeetingId);
            
            CreateTable(
                "dbo.People",
                c => new
                    {
                        Alias = c.String(nullable: false, maxLength: 128),
                        DepartMent = c.String(),
                        Photo = c.String(),
                        Infomation = c.String(),
                    })
                .PrimaryKey(t => t.Alias);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Members", "MeetingId", "dbo.Meetings");
            DropIndex("dbo.Members", new[] { "MeetingId" });
            DropTable("dbo.People");
            DropTable("dbo.Members");
            DropTable("dbo.Meetings");
        }
    }
}
