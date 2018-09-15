namespace EDDNBodyDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRegionAddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Regions", "RegionAddress", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Regions", "RegionAddress");
        }
    }
}
