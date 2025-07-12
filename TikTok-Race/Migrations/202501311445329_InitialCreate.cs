namespace TikTok_Race.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PlayerWins",
                c => new
                    {
                        PlayerWinID = c.Int(nullable: false, identity: true),
                        RaceID = c.Int(nullable: false),
                        PlayerName = c.String(),
                        Placement = c.Int(nullable: false),
                        Points = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PlayerWinID)
                .ForeignKey("dbo.Races", t => t.RaceID, cascadeDelete: true)
                .Index(t => t.RaceID);
            
            CreateTable(
                "dbo.Races",
                c => new
                    {
                        RaceID = c.Int(nullable: false, identity: true),
                        RaceDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RaceID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PlayerWins", "RaceID", "dbo.Races");
            DropIndex("dbo.PlayerWins", new[] { "RaceID" });
            DropTable("dbo.Races");
            DropTable("dbo.PlayerWins");
        }
    }
}
