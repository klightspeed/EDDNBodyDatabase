namespace EDDNBodyDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Atmospheres",
                c => new
                    {
                        Id = c.Byte(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.AtmosphereComponents",
                c => new
                    {
                        Id = c.Byte(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.AtmosphereTypes",
                c => new
                    {
                        Id = c.Byte(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.BodyTypes",
                c => new
                    {
                        Id = c.Byte(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.Luminosities",
                c => new
                    {
                        Id = c.Byte(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.MaterialNames",
                c => new
                    {
                        Id = c.Byte(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.PlanetClasses",
                c => new
                    {
                        Id = c.Byte(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.ReserveLevels",
                c => new
                    {
                        Id = c.Byte(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.RingClasses",
                c => new
                    {
                        Id = c.Byte(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.ScanTypes",
                c => new
                    {
                        Id = c.Byte(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.Softwares",
                c => new
                    {
                        Id = c.Byte(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.StarTypes",
                c => new
                    {
                        Id = c.Byte(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.TerraformStates",
                c => new
                    {
                        Id = c.Byte(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.Volcanism",
                c => new
                    {
                        Id = c.Byte(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.Regions",
                c => new
                    {
                        Id = c.Short(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        X0 = c.Single(),
                        Y0 = c.Single(),
                        Z0 = c.Single(),
                        SizeX = c.Int(),
                        SizeY = c.Int(),
                        SizeZ = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.SoftwareVersions",
                c => new
                    {
                        Id = c.Short(nullable: false, identity: true),
                        SoftwareId = c.Byte(nullable: false),
                        Version = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Softwares", t => t.SoftwareId)
                .Index(t => new { t.SoftwareId, t.Version }, name: "Version");
            
            CreateTable(
                "dbo.Systems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ModSystemAddress = c.Long(nullable: false),
                        X = c.Int(nullable: false),
                        Y = c.Int(nullable: false),
                        Z = c.Int(nullable: false),
                        RegionId = c.Short(nullable: false),
                        Mid1a = c.Byte(nullable: false),
                        Mid1b = c.Byte(nullable: false),
                        Mid2 = c.Byte(nullable: false),
                        SizeClass = c.Byte(nullable: false),
                        Mid3 = c.Byte(nullable: false),
                        Sequence = c.Short(nullable: false),
                        EdsmId = c.Int(nullable: false),
                        EdsmLastModifiedSeconds = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Regions", t => t.RegionId)
                .Index(t => t.ModSystemAddress);
            
            CreateTable(
                "dbo.SystemCustomNames",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        SystemAddress = c.Long(nullable: false),
                        CustomName = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Systems", t => t.Id)
                .Index(t => t.SystemAddress)
                .Index(t => t.CustomName);
            
            CreateTable(
                "dbo.SystemBodies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SystemId = c.Int(nullable: false),
                        BodyID = c.Short(nullable: false),
                        Stars = c.Byte(nullable: false),
                        Planet = c.Byte(nullable: false),
                        Moon1 = c.Byte(nullable: false),
                        Moon2 = c.Byte(nullable: false),
                        Moon3 = c.Byte(nullable: false),
                        IsBelt = c.Boolean(nullable: false),
                        ScanBaseHash = c.Int(nullable: false),
                        CustomNameId = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Systems", t => t.SystemId)
                .Index(t => new { t.SystemId, t.Stars, t.IsBelt, t.Planet, t.Moon1, t.Moon2, t.Moon3, t.ScanBaseHash, t.BodyID, t.CustomNameId }, unique: true, name: "BodyPgName");
            
            CreateTable(
                "dbo.SystemBodyCustomNames",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        SystemId = c.Int(nullable: false),
                        BodyID = c.Short(),
                        CustomName = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SystemBodies", t => t.Id)
                .Index(t => new { t.SystemId, t.CustomName }, name: "CustomName")
                .Index(t => t.CustomName);
            
            CreateTable(
                "dbo.SystemBodyDuplicates",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        DuplicateOfBodyId = c.Int(nullable: false),
                        BodyID = c.Short(nullable: false),
                        Name = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        ScanBaseHash = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SystemBodies", t => t.DuplicateOfBodyId)
                .ForeignKey("dbo.SystemBodies", t => t.Id)
                .Index(t => new { t.Name, t.ScanBaseHash }, name: "NameHash");
            
            CreateTable(
                "dbo.BodyCustomNames",
                c => new
                    {
                        Id = c.Short(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.ParentSets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(),
                        Parent1BodyId = c.Short(nullable: false),
                        Parent2BodyId = c.Short(nullable: false),
                        Parent3BodyId = c.Short(nullable: false),
                        Parent4BodyId = c.Short(nullable: false),
                        Parent5BodyId = c.Short(nullable: false),
                        Parent6BodyId = c.Short(nullable: false),
                        Parent7BodyId = c.Short(nullable: false),
                        Parent0TypeId = c.Byte(nullable: false),
                        Parent1TypeId = c.Byte(),
                        Parent2TypeId = c.Byte(),
                        Parent3TypeId = c.Byte(),
                        Parent4TypeId = c.Byte(),
                        Parent5TypeId = c.Byte(),
                        Parent6TypeId = c.Byte(),
                        Parent7TypeId = c.Byte(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BodyTypes", t => t.Parent0TypeId)
                .ForeignKey("dbo.BodyTypes", t => t.Parent1TypeId)
                .ForeignKey("dbo.BodyTypes", t => t.Parent2TypeId)
                .ForeignKey("dbo.BodyTypes", t => t.Parent3TypeId)
                .ForeignKey("dbo.BodyTypes", t => t.Parent4TypeId)
                .ForeignKey("dbo.BodyTypes", t => t.Parent5TypeId)
                .ForeignKey("dbo.BodyTypes", t => t.Parent6TypeId)
                .ForeignKey("dbo.BodyTypes", t => t.Parent7TypeId)
                .ForeignKey("dbo.ParentSets", t => t.ParentId)
                .Index(t => new { t.Parent1BodyId, t.Parent2BodyId, t.Parent3BodyId, t.Parent4BodyId, t.Parent5BodyId, t.Parent6BodyId, t.Parent7BodyId }, name: "IX_ParentBody");
            
            CreateTable(
                "dbo.BodyScans",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SystemBodyId = c.Int(nullable: false),
                        ParentSetId = c.Int(),
                        ScanBaseHash = c.Int(nullable: false),
                        Eccentricity = c.Single(nullable: false),
                        OrbitalInclination = c.Single(nullable: false),
                        Periapsis = c.Single(nullable: false),
                        HasOrbit = c.Boolean(nullable: false),
                        AxialTilt = c.Single(),
                        Radius = c.Single(nullable: false),
                        SemiMajorAxis = c.Single(nullable: false),
                        OrbitalPeriod = c.Single(nullable: false),
                        RotationPeriod = c.Single(nullable: false),
                        SurfaceTemperature = c.Single(nullable: false),
                        TidalLock = c.Boolean(),
                        ReserveLevelId = c.Byte(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ParentSets", t => t.ParentSetId)
                .ForeignKey("dbo.ReserveLevels", t => t.ReserveLevelId)
                .ForeignKey("dbo.SystemBodies", t => t.SystemBodyId)
                .Index(t => t.SystemBodyId);
            
            CreateTable(
                "dbo.BodyScanRings",
                c => new
                    {
                        ScanId = c.Int(nullable: false),
                        RingNum = c.Byte(nullable: false),
                        ClassId = c.Byte(nullable: false),
                        InnerRad = c.Single(nullable: false),
                        OuterRad = c.Single(nullable: false),
                        MassMT = c.Single(nullable: false),
                        IsBelt = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.ScanId, t.RingNum })
                .ForeignKey("dbo.RingClasses", t => t.ClassId)
                .ForeignKey("dbo.BodyScans", t => t.ScanId);
            
            CreateTable(
                "dbo.BodyScanRingCustomNames",
                c => new
                    {
                        ScanId = c.Int(nullable: false),
                        RingNum = c.Byte(nullable: false),
                        Name = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.ScanId, t.RingNum })
                .ForeignKey("dbo.BodyScanRings", t => new { t.ScanId, t.RingNum });
            
            CreateTable(
                "dbo.BodyScanAtmospheres",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        SurfacePressure = c.Single(nullable: false),
                        AtmosphereComponent1Amt = c.Single(nullable: false),
                        AtmosphereComponent2Amt = c.Single(nullable: false),
                        AtmosphereComponent3Amt = c.Single(nullable: false),
                        AtmosphereComponent1Id = c.Byte(),
                        AtmosphereComponent2Id = c.Byte(),
                        AtmosphereComponent3Id = c.Byte(),
                        AtmosphereId = c.Byte(nullable: false),
                        AtmosphereTypeId = c.Byte(),
                        AtmosphereHot = c.Boolean(nullable: false),
                        AtmosphereThin = c.Boolean(nullable: false),
                        AtmosphereThick = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AtmosphereComponents", t => t.AtmosphereComponent1Id)
                .ForeignKey("dbo.AtmosphereComponents", t => t.AtmosphereComponent2Id)
                .ForeignKey("dbo.AtmosphereComponents", t => t.AtmosphereComponent3Id)
                .ForeignKey("dbo.Atmospheres", t => t.AtmosphereId)
                .ForeignKey("dbo.AtmosphereTypes", t => t.AtmosphereTypeId)
                .ForeignKey("dbo.BodyScanPlanets", t => t.Id);
            
            CreateTable(
                "dbo.BodyScanMaterials",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        MaterialCarbon = c.Single(nullable: false),
                        MaterialIron = c.Single(nullable: false),
                        MaterialNickel = c.Single(nullable: false),
                        MaterialPhosphorus = c.Single(nullable: false),
                        MaterialSulphur = c.Single(nullable: false),
                        Material1Amt = c.Single(nullable: false),
                        Material2Amt = c.Single(nullable: false),
                        Material3Amt = c.Single(nullable: false),
                        Material4Amt = c.Single(nullable: false),
                        Material5Amt = c.Single(nullable: false),
                        Material6Amt = c.Single(nullable: false),
                        Material1Id = c.Byte(nullable: false),
                        Material2Id = c.Byte(nullable: false),
                        Material3Id = c.Byte(nullable: false),
                        Material4Id = c.Byte(nullable: false),
                        Material5Id = c.Byte(nullable: false),
                        Material6Id = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MaterialNames", t => t.Material1Id)
                .ForeignKey("dbo.MaterialNames", t => t.Material2Id)
                .ForeignKey("dbo.MaterialNames", t => t.Material3Id)
                .ForeignKey("dbo.MaterialNames", t => t.Material4Id)
                .ForeignKey("dbo.MaterialNames", t => t.Material5Id)
                .ForeignKey("dbo.MaterialNames", t => t.Material6Id)
                .ForeignKey("dbo.BodyScanPlanets", t => t.Id);
            
            CreateTable(
                "dbo.EDDNJournalScans",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BodyScanId = c.Int(nullable: false),
                        GatewayTimestampTicks = c.Long(nullable: false),
                        ScanTimestampSeconds = c.Int(nullable: false),
                        SoftwareVersionId = c.Short(nullable: false),
                        ScanTypeId = c.Byte(),
                        DistanceFromArrivalLS = c.Single(nullable: false),
                        HasSystemAddress = c.Boolean(nullable: false),
                        HasBodyID = c.Boolean(nullable: false),
                        HasParents = c.Boolean(nullable: false),
                        HasComposition = c.Boolean(nullable: false),
                        HasAxialTilt = c.Boolean(nullable: false),
                        HasLuminosity = c.Boolean(nullable: false),
                        IsMaterialsDict = c.Boolean(nullable: false),
                        IsBasicScan = c.Boolean(nullable: false),
                        IsPos3SigFig = c.Boolean(nullable: false),
                        HasAtmosphereType = c.Boolean(nullable: false),
                        HasAtmosphereComposition = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BodyScans", t => t.BodyScanId)
                .ForeignKey("dbo.ScanTypes", t => t.ScanTypeId)
                .ForeignKey("dbo.SoftwareVersions", t => t.SoftwareVersionId)
                .Index(t => t.BodyScanId);
            
            CreateTable(
                "dbo.EDDNJournalScanJsonExtras",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        JsonExtra = c.String(nullable: false, maxLength: 1024, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EDDNJournalScans", t => t.Id);
            
            CreateTable(
                "dbo.BodyScanStars",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        AbsoluteMagnitude = c.Single(nullable: false),
                        StellarMass = c.Single(nullable: false),
                        Age_MY = c.Short(nullable: false),
                        StarTypeId = c.Byte(nullable: false),
                        LuminosityId = c.Byte(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BodyScans", t => t.Id)
                .ForeignKey("dbo.StarTypes", t => t.StarTypeId)
                .ForeignKey("dbo.Luminosities", t => t.LuminosityId);
            
            CreateTable(
                "dbo.BodyScanPlanets",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        PlanetClassId = c.Byte(nullable: false),
                        CompositionMetal = c.Single(nullable: false),
                        CompositionRock = c.Single(nullable: false),
                        CompositionIce = c.Single(nullable: false),
                        MassEM = c.Single(nullable: false),
                        SurfaceGravity = c.Single(nullable: false),
                        VolcanismId = c.Byte(),
                        VolcanismMinor = c.Boolean(nullable: false),
                        VolcanismMajor = c.Boolean(nullable: false),
                        IsLandable = c.Boolean(),
                        HasComposition = c.Boolean(nullable: false),
                        TerraformStateId = c.Byte(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BodyScans", t => t.Id)
                .ForeignKey("dbo.PlanetClasses", t => t.PlanetClassId)
                .ForeignKey("dbo.Volcanism", t => t.VolcanismId)
                .ForeignKey("dbo.TerraformStates", t => t.TerraformStateId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BodyScanPlanets", "TerraformStateId", "dbo.TerraformStates");
            DropForeignKey("dbo.BodyScanPlanets", "VolcanismId", "dbo.Volcanism");
            DropForeignKey("dbo.BodyScanPlanets", "PlanetClassId", "dbo.PlanetClasses");
            DropForeignKey("dbo.BodyScanPlanets", "Id", "dbo.BodyScans");
            DropForeignKey("dbo.BodyScanStars", "LuminosityId", "dbo.Luminosities");
            DropForeignKey("dbo.BodyScanStars", "StarTypeId", "dbo.StarTypes");
            DropForeignKey("dbo.BodyScanStars", "Id", "dbo.BodyScans");
            DropForeignKey("dbo.EDDNJournalScans", "SoftwareVersionId", "dbo.SoftwareVersions");
            DropForeignKey("dbo.EDDNJournalScans", "ScanTypeId", "dbo.ScanTypes");
            DropForeignKey("dbo.EDDNJournalScans", "BodyScanId", "dbo.BodyScans");
            DropForeignKey("dbo.EDDNJournalScanJsonExtras", "Id", "dbo.EDDNJournalScans");
            DropForeignKey("dbo.BodyScanMaterials", "Id", "dbo.BodyScanPlanets");
            DropForeignKey("dbo.BodyScanMaterials", "Material6Id", "dbo.MaterialNames");
            DropForeignKey("dbo.BodyScanMaterials", "Material5Id", "dbo.MaterialNames");
            DropForeignKey("dbo.BodyScanMaterials", "Material4Id", "dbo.MaterialNames");
            DropForeignKey("dbo.BodyScanMaterials", "Material3Id", "dbo.MaterialNames");
            DropForeignKey("dbo.BodyScanMaterials", "Material2Id", "dbo.MaterialNames");
            DropForeignKey("dbo.BodyScanMaterials", "Material1Id", "dbo.MaterialNames");
            DropForeignKey("dbo.BodyScanAtmospheres", "Id", "dbo.BodyScanPlanets");
            DropForeignKey("dbo.BodyScanAtmospheres", "AtmosphereTypeId", "dbo.AtmosphereTypes");
            DropForeignKey("dbo.BodyScanAtmospheres", "AtmosphereId", "dbo.Atmospheres");
            DropForeignKey("dbo.BodyScanAtmospheres", "AtmosphereComponent3Id", "dbo.AtmosphereComponents");
            DropForeignKey("dbo.BodyScanAtmospheres", "AtmosphereComponent2Id", "dbo.AtmosphereComponents");
            DropForeignKey("dbo.BodyScanAtmospheres", "AtmosphereComponent1Id", "dbo.AtmosphereComponents");
            DropForeignKey("dbo.BodyScans", "SystemBodyId", "dbo.SystemBodies");
            DropForeignKey("dbo.BodyScanRings", "ScanId", "dbo.BodyScans");
            DropForeignKey("dbo.BodyScanRingCustomNames", new[] { "ScanId", "RingNum" }, "dbo.BodyScanRings");
            DropForeignKey("dbo.BodyScanRings", "ClassId", "dbo.RingClasses");
            DropForeignKey("dbo.BodyScans", "ReserveLevelId", "dbo.ReserveLevels");
            DropForeignKey("dbo.BodyScans", "ParentSetId", "dbo.ParentSets");
            DropForeignKey("dbo.ParentSets", "ParentId", "dbo.ParentSets");
            DropForeignKey("dbo.ParentSets", "Parent7TypeId", "dbo.BodyTypes");
            DropForeignKey("dbo.ParentSets", "Parent6TypeId", "dbo.BodyTypes");
            DropForeignKey("dbo.ParentSets", "Parent5TypeId", "dbo.BodyTypes");
            DropForeignKey("dbo.ParentSets", "Parent4TypeId", "dbo.BodyTypes");
            DropForeignKey("dbo.ParentSets", "Parent3TypeId", "dbo.BodyTypes");
            DropForeignKey("dbo.ParentSets", "Parent2TypeId", "dbo.BodyTypes");
            DropForeignKey("dbo.ParentSets", "Parent1TypeId", "dbo.BodyTypes");
            DropForeignKey("dbo.ParentSets", "Parent0TypeId", "dbo.BodyTypes");
            DropForeignKey("dbo.SystemBodies", "SystemId", "dbo.Systems");
            DropForeignKey("dbo.SystemBodyDuplicates", "Id", "dbo.SystemBodies");
            DropForeignKey("dbo.SystemBodyDuplicates", "DuplicateOfBodyId", "dbo.SystemBodies");
            DropForeignKey("dbo.SystemBodyCustomNames", "Id", "dbo.SystemBodies");
            DropForeignKey("dbo.SystemCustomNames", "Id", "dbo.Systems");
            DropForeignKey("dbo.Systems", "RegionId", "dbo.Regions");
            DropForeignKey("dbo.SoftwareVersions", "SoftwareId", "dbo.Softwares");
            DropIndex("dbo.EDDNJournalScans", new[] { "BodyScanId" });
            DropIndex("dbo.BodyScans", new[] { "SystemBodyId" });
            DropIndex("dbo.ParentSets", "IX_ParentBody");
            DropIndex("dbo.BodyCustomNames", new[] { "Name" });
            DropIndex("dbo.SystemBodyDuplicates", "NameHash");
            DropIndex("dbo.SystemBodyCustomNames", new[] { "CustomName" });
            DropIndex("dbo.SystemBodyCustomNames", "CustomName");
            DropIndex("dbo.SystemBodies", "BodyPgName");
            DropIndex("dbo.SystemCustomNames", new[] { "CustomName" });
            DropIndex("dbo.SystemCustomNames", new[] { "SystemAddress" });
            DropIndex("dbo.Systems", new[] { "ModSystemAddress" });
            DropIndex("dbo.SoftwareVersions", "Version");
            DropIndex("dbo.Regions", new[] { "Name" });
            DropIndex("dbo.Volcanism", new[] { "Name" });
            DropIndex("dbo.TerraformStates", new[] { "Name" });
            DropIndex("dbo.StarTypes", new[] { "Name" });
            DropIndex("dbo.Softwares", new[] { "Name" });
            DropIndex("dbo.ScanTypes", new[] { "Name" });
            DropIndex("dbo.RingClasses", new[] { "Name" });
            DropIndex("dbo.ReserveLevels", new[] { "Name" });
            DropIndex("dbo.PlanetClasses", new[] { "Name" });
            DropIndex("dbo.MaterialNames", new[] { "Name" });
            DropIndex("dbo.Luminosities", new[] { "Name" });
            DropIndex("dbo.BodyTypes", new[] { "Name" });
            DropIndex("dbo.AtmosphereTypes", new[] { "Name" });
            DropIndex("dbo.AtmosphereComponents", new[] { "Name" });
            DropIndex("dbo.Atmospheres", new[] { "Name" });
            DropTable("dbo.BodyScanPlanets");
            DropTable("dbo.BodyScanStars");
            DropTable("dbo.EDDNJournalScanJsonExtras");
            DropTable("dbo.EDDNJournalScans");
            DropTable("dbo.BodyScanMaterials");
            DropTable("dbo.BodyScanAtmospheres");
            DropTable("dbo.BodyScanRingCustomNames");
            DropTable("dbo.BodyScanRings");
            DropTable("dbo.BodyScans");
            DropTable("dbo.ParentSets");
            DropTable("dbo.BodyCustomNames");
            DropTable("dbo.SystemBodyDuplicates");
            DropTable("dbo.SystemBodyCustomNames");
            DropTable("dbo.SystemBodies");
            DropTable("dbo.SystemCustomNames");
            DropTable("dbo.Systems");
            DropTable("dbo.SoftwareVersions");
            DropTable("dbo.Regions");
            DropTable("dbo.Volcanism");
            DropTable("dbo.TerraformStates");
            DropTable("dbo.StarTypes");
            DropTable("dbo.Softwares");
            DropTable("dbo.ScanTypes");
            DropTable("dbo.RingClasses");
            DropTable("dbo.ReserveLevels");
            DropTable("dbo.PlanetClasses");
            DropTable("dbo.MaterialNames");
            DropTable("dbo.Luminosities");
            DropTable("dbo.BodyTypes");
            DropTable("dbo.AtmosphereTypes");
            DropTable("dbo.AtmosphereComponents");
            DropTable("dbo.Atmospheres");
        }
    }
}
