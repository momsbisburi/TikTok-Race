namespace TikTok_Race.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class player : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        PlayerID = c.Int(nullable: false, identity: true),
                        uniqueId = c.String(),
                        nickname = c.String(),
                        ProfilePicture = c.String(),
                    })
                .PrimaryKey(t => t.PlayerID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Players");
        }
    }
}
