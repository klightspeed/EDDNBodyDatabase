namespace EDDNBodyDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SplitSectors : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Systems", "RegionId", "dbo.Regions");
            CreateTable(
                "dbo.SystemSectorNames",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        RegionId = c.Short(nullable: false),
                        Mid1a = c.Byte(nullable: false),
                        Mid1b = c.Byte(nullable: false),
                        Mid2 = c.Byte(nullable: false),
                        SizeClass = c.Byte(nullable: false),
                        Mid3 = c.Byte(nullable: false),
                        Sequence = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Regions", t => t.RegionId)
                .ForeignKey("dbo.Systems", t => t.Id);

            Sql("INSERT INTO SystemSectorNames (Id, RegionId, Mid1a, Mid1b, Mid2, SizeClass, Mid3, Sequence) " +
                "SELECT s.Id, s.RegionId, s.Mid1a, s.Mid1b, s.Mid2, s.SizeClass, s.Mid3, s.Sequence " +
                "FROM Systems s " +
                "JOIN Regions r ON r.Id = s.RegionId " +
                "WHERE r.RegionAddress IS NULL");

            DropColumn("dbo.Systems", "RegionId");
            DropColumn("dbo.Systems", "Mid1a");
            DropColumn("dbo.Systems", "Mid1b");
            DropColumn("dbo.Systems", "Mid2");
            DropColumn("dbo.Systems", "SizeClass");
            DropColumn("dbo.Systems", "Mid3");
            DropColumn("dbo.Systems", "Sequence");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Systems", "Sequence", c => c.Short(nullable: false));
            AddColumn("dbo.Systems", "Mid3", c => c.Byte(nullable: false));
            AddColumn("dbo.Systems", "SizeClass", c => c.Byte(nullable: false));
            AddColumn("dbo.Systems", "Mid2", c => c.Byte(nullable: false));
            AddColumn("dbo.Systems", "Mid1b", c => c.Byte(nullable: false));
            AddColumn("dbo.Systems", "Mid1a", c => c.Byte(nullable: false));
            AddColumn("dbo.Systems", "RegionId", c => c.Short(nullable: false));
            DropForeignKey("dbo.SystemSectorNames", "Id", "dbo.Systems");
            DropForeignKey("dbo.SystemSectorNames", "RegionId", "dbo.Regions");
            DropTable("dbo.SystemSectorNames");
            AddForeignKey("dbo.Systems", "RegionId", "dbo.Regions", "Id");
        }
    }
}
