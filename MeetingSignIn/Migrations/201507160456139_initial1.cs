namespace MeetingSignIn.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Meetings", "Newest", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Meetings", "Newest");
        }
    }
}
