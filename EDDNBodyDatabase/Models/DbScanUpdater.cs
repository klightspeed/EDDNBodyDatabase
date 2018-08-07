using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public class DbScanUpdater : IDisposable
    {
        private BodyDbContext Context;

        DbCommand cmdUpdateBody = null;
        DbParameter paramUpdateBodyId;
        DbParameter paramUpdateBodyBodyID;

        DbCommand cmdUpdateScan = null;
        DbParameter paramUpdateScanId;
        DbParameter paramUpdateScanTidalLock;
        DbParameter paramUpdateScanAxialTilt;
        DbParameter paramUpdateScanParents;
        DbParameter paramUpdateScanReserveLevel;

        DbCommand cmdUpdatePlanet = null;
        DbParameter paramUpdatePlanetScanId;
        DbParameter paramUpdatePlanetVolcanismId;
        DbParameter paramUpdatePlanetVolcanismMajor;
        DbParameter paramUpdatePlanetVolcanismMinor;
        DbParameter paramUpdatePlanetLandable;
        DbParameter paramUpdatePlanetComposMetal;
        DbParameter paramUpdatePlanetComposRock;
        DbParameter paramUpdatePlanetComposIce;
        DbParameter paramUpdatePlanetHasCompos;
        DbParameter paramUpdatePlanetTerraformState;

        DbCommand cmdUpdateStar = null;
        DbParameter paramUpdateStarScanId;
        DbParameter paramUpdateStarLuminosity;

        DbCommand cmdInsertRings = null;
        DbParameter paramRingScanId;
        DbParameter paramRingNum;
        DbParameter paramRingClass;
        DbParameter paramRingInnerRad;
        DbParameter paramRingOuterRad;
        DbParameter paramRingMassMT;
        DbParameter paramRingIsBelt;

        DbCommand cmdInsertRingName = null;
        DbParameter paramRingNameScanId;
        DbParameter paramRingNameNum;
        DbParameter paramRingName;

        DbCommand cmdInsertMats = null;
        DbParameter paramMatsId;
        DbParameter paramMatsCarbon;
        DbParameter paramMatsIron;
        DbParameter paramMatsNickel;
        DbParameter paramMatsPhosphorus;
        DbParameter paramMatsSulphur;
        DbParameter paramMat1Id;
        DbParameter paramMat1Amt;
        DbParameter paramMat2Id;
        DbParameter paramMat2Amt;
        DbParameter paramMat3Id;
        DbParameter paramMat3Amt;
        DbParameter paramMat4Id;
        DbParameter paramMat4Amt;
        DbParameter paramMat5Id;
        DbParameter paramMat5Amt;
        DbParameter paramMat6Id;
        DbParameter paramMat6Amt;

        DbCommand cmdInsertAtmos = null;
        DbParameter paramAtmosScanId;
        DbParameter paramSurfacePressure;
        DbParameter paramAtmosComp1Id;
        DbParameter paramAtmosComp1Amt;
        DbParameter paramAtmosComp2Id;
        DbParameter paramAtmosComp2Amt;
        DbParameter paramAtmosComp3Id;
        DbParameter paramAtmosComp3Amt;
        DbParameter paramAtmosNameId;
        DbParameter paramAtmosTypeId;
        DbParameter paramAtmosHot;
        DbParameter paramAtmosThin;
        DbParameter paramAtmosThick;

        DbCommand cmdUpdateAtmos = null;
        DbParameter paramAtmosUpdateScanId;
        DbParameter paramAtmosUpdateComp1Id;
        DbParameter paramAtmosUpdateComp1Amt;
        DbParameter paramAtmosUpdateComp2Id;
        DbParameter paramAtmosUpdateComp2Amt;
        DbParameter paramAtmosUpdateComp3Id;
        DbParameter paramAtmosUpdateComp3Amt;
        DbParameter paramAtmosUpdateTypeId;

        public DbScanUpdater(BodyDbContext ctx)
        {
            Context = ctx;

            if (ctx.Database.Connection.State != ConnectionState.Open)
            {
                ctx.Database.Connection.Open();
            }

            cmdUpdateBody = ctx.Database.Connection.CreateCommand();
            cmdUpdateBody.CommandText = "UPDATE SystemBodies SET BodyID = @BodyID WHERE Id = @Id";
            cmdUpdateBody.CommandType = CommandType.Text;

            paramUpdateBodyId = cmdUpdateBody.AddParameter("@Id", DbType.Int32);
            paramUpdateBodyBodyID = cmdUpdateBody.AddParameter("@BodyID", DbType.Int16);

            cmdUpdateScan = ctx.Database.Connection.CreateCommand();
            cmdUpdateScan.CommandText = 
                "UPDATE BodyScans SET " +
                "ParentSetId = @ParentSetId, TidalLock = @TidalLock, " +
                "AxialTilt = @AxialTilt, ReserveLevelId = @ReserveLevelId " +
                "WHERE Id = @Id";
            cmdUpdateScan.CommandType = CommandType.Text;

            paramUpdateScanId = cmdUpdateScan.AddParameter("@Id", DbType.Int32);
            paramUpdateScanTidalLock = cmdUpdateScan.AddParameter("@TidalLock", DbType.Boolean);
            paramUpdateScanAxialTilt = cmdUpdateScan.AddParameter("@AxialTilt", DbType.Single);
            paramUpdateScanParents = cmdUpdateScan.AddParameter("@ParentSetId", DbType.Int32);
            paramUpdateScanReserveLevel = cmdUpdateScan.AddParameter("@ReserveLevelId", DbType.Byte);

            cmdUpdatePlanet = ctx.Database.Connection.CreateCommand();
            cmdUpdatePlanet.CommandText =
                "UPDATE BodyScanPlanets SET " +
                "VolcanismId = @VolcanismId, VolcanismMajor = @VolcanismMajor, VolcanismMinor = @VolcanismMinor, " +
                "IsLandable = @IsLandable, TerraformStateId = @TerraformStateId, HasComposition = @HasComposition, " +
                "CompositionMetal = @CompositionMetal, CompositionRock = @CompositionRock, CompositionIce = @CompositionIce " +
                "WHERE Id = @Id";
            cmdUpdatePlanet.CommandType = CommandType.Text;

            paramUpdatePlanetScanId = cmdUpdatePlanet.AddParameter("@Id", DbType.Int32);
            paramUpdatePlanetVolcanismId = cmdUpdatePlanet.AddParameter("@VolcanismId", DbType.Byte);
            paramUpdatePlanetVolcanismMajor = cmdUpdatePlanet.AddParameter("@VolcanismMajor", DbType.Boolean);
            paramUpdatePlanetVolcanismMinor = cmdUpdatePlanet.AddParameter("@VolcanismMinor", DbType.Boolean);
            paramUpdatePlanetLandable = cmdUpdatePlanet.AddParameter("@IsLandable", DbType.Boolean);
            paramUpdatePlanetComposMetal = cmdUpdatePlanet.AddParameter("@CompositionMetal", DbType.Single);
            paramUpdatePlanetComposRock = cmdUpdatePlanet.AddParameter("@CompositionRock", DbType.Single);
            paramUpdatePlanetComposIce = cmdUpdatePlanet.AddParameter("@CompositionIce", DbType.Single);
            paramUpdatePlanetHasCompos = cmdUpdatePlanet.AddParameter("@HasComposition", DbType.Boolean);
            paramUpdatePlanetTerraformState = cmdUpdatePlanet.AddParameter("@TerraformStateId", DbType.Byte);

            cmdUpdateStar = ctx.Database.Connection.CreateCommand();
            cmdUpdateStar.CommandText = "UPDATE BodyScanStars SET LuminosityId = @LuminosityId WHERE Id = @Id";
            cmdUpdateStar.CommandType = CommandType.Text;

            paramUpdateStarScanId = cmdUpdateStar.AddParameter("@Id", DbType.Int32);
            paramUpdateStarLuminosity = cmdUpdateStar.AddParameter("@LuminosityId", DbType.Byte);

            cmdUpdateAtmos = ctx.Database.Connection.CreateCommand();
            cmdUpdateAtmos.CommandText =
                "UPDATE BodyScanAtmospheres SET " +
                "AtmosphereTypeId = @AtmosphereTypeId, " +
                "AtmosphereComponent1Id = @AtmosphereComponent1Id, AtmosphereComponent1Amt = @AtmosphereComponent1Amt, " +
                "AtmosphereComponent2Id = @AtmosphereComponent2Id, AtmosphereComponent2Amt = @AtmosphereComponent2Amt, " +
                "AtmosphereComponent3Id = @AtmosphereComponent3Id, AtmosphereComponent3Amt = @AtmosphereComponent3Amt " +
                "WHERE Id = @Id";
            cmdUpdateAtmos.CommandType = CommandType.Text;

            paramAtmosUpdateScanId = cmdUpdateAtmos.AddParameter("@Id", DbType.Int32);
            paramAtmosUpdateComp1Id = cmdUpdateAtmos.AddParameter("@AtmosphereComponent1Id", DbType.Byte);
            paramAtmosUpdateComp1Amt = cmdUpdateAtmos.AddParameter("@AtmosphereComponent1Amt", DbType.Single);
            paramAtmosUpdateComp2Id = cmdUpdateAtmos.AddParameter("@AtmosphereComponent2Id", DbType.Byte);
            paramAtmosUpdateComp2Amt = cmdUpdateAtmos.AddParameter("@AtmosphereComponent2Amt", DbType.Single);
            paramAtmosUpdateComp3Id = cmdUpdateAtmos.AddParameter("@AtmosphereComponent3Id", DbType.Byte);
            paramAtmosUpdateComp3Amt = cmdUpdateAtmos.AddParameter("@AtmosphereComponent3Amt", DbType.Single);
            paramAtmosUpdateTypeId = cmdUpdateAtmos.AddParameter("@AtmosphereTypeId", DbType.Byte);

            cmdInsertRings = ctx.Database.Connection.CreateCommand();
            cmdInsertRings.CommandText =
                "INSERT INTO BodyScanRings (ScanId, RingNum, ClassId, InnerRad, OuterRad, MassMT, IsBelt) " +
                "VALUES (@ScanId, @RingNum, @ClassId, @InnerRad, @OuterRad, @MassMT, @IsBelt)";
            cmdInsertRings.CommandType = CommandType.Text;

            paramRingScanId = cmdInsertRings.AddParameter("@ScanId", DbType.Int32);
            paramRingNum = cmdInsertRings.AddParameter("@RingNum", DbType.Byte);
            paramRingClass = cmdInsertRings.AddParameter("@ClassId", DbType.Byte);
            paramRingName = cmdInsertRings.AddParameter("@Name", DbType.String);
            paramRingInnerRad = cmdInsertRings.AddParameter("@InnerRad", DbType.Single);
            paramRingOuterRad = cmdInsertRings.AddParameter("@OuterRad", DbType.Single);
            paramRingMassMT = cmdInsertRings.AddParameter("@MassMT", DbType.Single);
            paramRingIsBelt = cmdInsertRings.AddParameter("@IsBelt", DbType.Boolean);

            cmdInsertRingName = ctx.Database.Connection.CreateCommand();
            cmdInsertRingName.CommandText = "INSERT INTO BodyScanRingCustomNames (ScanId, RingNum, Name) VALUES (@ScanId, @RingNum, @Name)";
            cmdInsertRingName.CommandType = CommandType.Text;

            paramRingNameScanId = cmdInsertRingName.AddParameter("@ScanId", DbType.Int32);
            paramRingNameNum = cmdInsertRingName.AddParameter("@RingNum", DbType.Byte);
            paramRingName = cmdInsertRingName.AddParameter("@Name", DbType.String);

            cmdInsertMats = ctx.Database.Connection.CreateCommand();
            cmdInsertMats.CommandText =
                "INSERT INTO BodyScanMaterials (Id, MaterialCarbon, MaterialIron, MaterialNickel, MaterialPhosphorus, MaterialSulphur, Material1Id, Material1Amt, Material2Id, Material2Amt, Material3Id, Material3Amt, Material4Id, Material4Amt, Material5Id, Material5Amt, Material6Id, Material6Amt) VALUES " +
                "(@Id, @MaterialCarbon, @MaterialIron, @MaterialNickel, @MaterialPhosphorus, @MaterialSulphur, @Material1Id, @Material1Amt, @Material2Id, @Material2Amt, @Material3Id, @Material3Amt, @Material4Id, @Material4Amt, @Material5Id, @Material5Amt, @Material6Id, @Material6Amt)";
            cmdInsertMats.CommandType = CommandType.Text;

            paramMatsId = cmdInsertMats.AddParameter("@Id", DbType.Int32);
            paramMatsCarbon = cmdInsertMats.AddParameter("@MaterialCarbon", DbType.Single);
            paramMatsIron = cmdInsertMats.AddParameter("@MaterialIron", DbType.Single);
            paramMatsNickel = cmdInsertMats.AddParameter("@MaterialNickel", DbType.Single);
            paramMatsPhosphorus = cmdInsertMats.AddParameter("@MaterialPhosphorus", DbType.Single);
            paramMatsSulphur = cmdInsertMats.AddParameter("@MaterialSulphur", DbType.Single);
            paramMat1Id = cmdInsertMats.AddParameter("@Material1Id", DbType.Byte);
            paramMat1Amt = cmdInsertMats.AddParameter("@Material1Amt", DbType.Single);
            paramMat2Id = cmdInsertMats.AddParameter("@Material2Id", DbType.Byte);
            paramMat2Amt = cmdInsertMats.AddParameter("@Material2Amt", DbType.Single);
            paramMat3Id = cmdInsertMats.AddParameter("@Material3Id", DbType.Byte);
            paramMat3Amt = cmdInsertMats.AddParameter("@Material3Amt", DbType.Single);
            paramMat4Id = cmdInsertMats.AddParameter("@Material4Id", DbType.Byte);
            paramMat4Amt = cmdInsertMats.AddParameter("@Material4Amt", DbType.Single);
            paramMat5Id = cmdInsertMats.AddParameter("@Material5Id", DbType.Byte);
            paramMat5Amt = cmdInsertMats.AddParameter("@Material5Amt", DbType.Single);
            paramMat6Id = cmdInsertMats.AddParameter("@Material6Id", DbType.Byte);
            paramMat6Amt = cmdInsertMats.AddParameter("@Material6Amt", DbType.Single);

            cmdInsertAtmos = ctx.Database.Connection.CreateCommand();
            cmdInsertAtmos.CommandText =
                "INSERT INTO BodyScanAtmospheres (Id, SurfacePressure, AtmosphereComponent1Id, AtmosphereComponent1Amt, AtmosphereComponent2Id, AtmosphereComponent2Amt, AtmosphereComponent3Id, AtmosphereComponent3Amt, AtmosphereId, AtmosphereTypeId, AtmosphereHot, AtmosphereThin, AtmosphereThick) VALUES " +
                "(@Id, @SurfacePressure, @AtmosphereComponent1Id, @AtmosphereComponent1Amt, @AtmosphereComponent2Id, @AtmosphereComponent2Amt, @AtmosphereComponent3Id, @AtmosphereComponent3Amt, @AtmosphereId, @AtmosphereTypeId, @AtmosphereHot, @AtmosphereThin, @AtmosphereThick)";
            cmdInsertAtmos.CommandType = CommandType.Text;

            paramAtmosScanId = cmdInsertAtmos.AddParameter("@Id", DbType.Int32);
            paramSurfacePressure = cmdInsertAtmos.AddParameter("@SurfacePressure", DbType.Single);
            paramAtmosComp1Id = cmdInsertAtmos.AddParameter("@AtmosphereComponent1Id", DbType.Byte);
            paramAtmosComp1Amt = cmdInsertAtmos.AddParameter("@AtmosphereComponent1Amt", DbType.Single);
            paramAtmosComp2Id = cmdInsertAtmos.AddParameter("@AtmosphereComponent2Id", DbType.Byte);
            paramAtmosComp2Amt = cmdInsertAtmos.AddParameter("@AtmosphereComponent2Amt", DbType.Single);
            paramAtmosComp3Id = cmdInsertAtmos.AddParameter("@AtmosphereComponent3Id", DbType.Byte);
            paramAtmosComp3Amt = cmdInsertAtmos.AddParameter("@AtmosphereComponent3Amt", DbType.Single);
            paramAtmosNameId = cmdInsertAtmos.AddParameter("@AtmosphereId", DbType.Byte);
            paramAtmosTypeId = cmdInsertAtmos.AddParameter("@AtmosphereTypeId", DbType.Byte);
            paramAtmosHot = cmdInsertAtmos.AddParameter("@AtmosphereHot", DbType.Boolean);
            paramAtmosThin = cmdInsertAtmos.AddParameter("@AtmosphereThin", DbType.Boolean);
            paramAtmosThick = cmdInsertAtmos.AddParameter("@AtmosphereThick", DbType.Boolean);
        }

        public void Update(XModels.XScanClass scandata, SystemBody dbbody, BodyScan dbscan)
        {
            if (dbbody.BodyID == -1 && scandata.Body.BodyID >= 0)
            {
                short bodyid = scandata.Body.BodyID;
                paramUpdateBodyId.Value = dbbody.Id;
                paramUpdateBodyBodyID.Value = bodyid;
                cmdUpdateBody.ExecuteNonQuery();
                dbbody.BodyID = bodyid;
            }

            if ((dbscan.ParentSetId == null && scandata.Scan.ParentSetId != 0) ||
                (dbscan.AxialTilt == null && scandata.Scan.HasAxialTilt) ||
                (dbscan.ReserveLevelId == null && scandata.Scan.ReserveLevelId != 0) ||
                (dbscan.TidalLock == null && scandata.Scan.HasTidalLock))
            {
                paramUpdateScanId.Value = dbscan.Id;
                paramUpdateScanAxialTilt.Value = (object)dbscan.AxialTilt ?? (scandata.Scan.HasAxialTilt ? (object)scandata.Scan.AxialTilt : DBNull.Value);
                paramUpdateScanReserveLevel.Value = (object)dbscan.ReserveLevelId ?? (scandata.Scan.ReserveLevelId != 0 ? (object)scandata.Scan.ReserveLevelId : DBNull.Value);
                paramUpdateScanTidalLock.Value = (object)dbscan.TidalLock ?? (scandata.Scan.HasTidalLock ? (object)scandata.Scan.TidalLock : DBNull.Value);
                paramUpdateScanParents.Value = (object)dbscan.ParentSetId ?? (scandata.Scan.HasParents ? (object)scandata.Parents.DbId : DBNull.Value);
                cmdUpdateScan.ExecuteNonQuery();
                dbscan.AxialTilt = dbscan.AxialTilt ?? (scandata.Scan.HasAxialTilt ? (float?)scandata.Scan.AxialTilt : null);
                dbscan.TidalLock = dbscan.TidalLock ?? (scandata.Scan.HasTidalLock ? (bool?)scandata.Scan.TidalLock : null);
                dbscan.ReserveLevelId = dbscan.ReserveLevelId ?? (scandata.Scan.ReserveLevelId != 0 ? (byte?)scandata.Scan.ReserveLevelId : null);
                dbscan.ParentSetId = dbscan.ParentSetId ?? (scandata.Scan.ParentSetId != 0 ? (byte?)scandata.Scan.ParentSetId : null);
            }

            if (dbscan is BodyScanPlanet pscan && scandata.Scan.IsPlanet)
            {
                if ((!pscan.HasComposition && scandata.Planet.HasComposition) ||
                    (pscan.VolcanismId == null && scandata.Planet.VolcanismId != 0) ||
                    (pscan.IsLandable == null && scandata.Planet.HasLandable) ||
                    (pscan.TerraformStateId == null && scandata.Planet.TerraformStateId != 0))
                {
                    pscan.CompositionMetal = pscan.HasComposition ? pscan.CompositionMetal : scandata.Planet.CompositionMetal;
                    pscan.CompositionRock = pscan.HasComposition ? pscan.CompositionRock : scandata.Planet.CompositionRock;
                    pscan.CompositionIce = pscan.HasComposition ? pscan.CompositionIce : scandata.Planet.CompositionIce;
                    pscan.HasComposition |= scandata.Planet.HasComposition;
                    pscan.IsLandable = pscan.IsLandable ?? (scandata.Planet.HasLandable ? (bool?)scandata.Planet.IsLandable : null);
                    pscan.TerraformStateId = pscan.TerraformStateId ?? (scandata.Planet.TerraformStateId != 0 ? (byte?)scandata.Planet.TerraformStateId : null);
                    pscan.VolcanismMajor = pscan.VolcanismId != null ? pscan.VolcanismMajor : scandata.Planet.VolcanismMajor;
                    pscan.VolcanismMinor = pscan.VolcanismId != null ? pscan.VolcanismMinor : scandata.Planet.VolcanismMinor;
                    pscan.VolcanismId = pscan.VolcanismId ?? (scandata.Planet.VolcanismId != 0 ? (byte?)scandata.Planet.VolcanismId : null);
                    paramUpdatePlanetScanId.Value = pscan.Id;
                    paramUpdatePlanetComposIce.Value = pscan.CompositionIce;
                    paramUpdatePlanetComposRock.Value = pscan.CompositionRock;
                    paramUpdatePlanetComposMetal.Value = pscan.CompositionMetal;
                    paramUpdatePlanetHasCompos.Value = pscan.HasComposition;
                    paramUpdatePlanetLandable.Value = (object)pscan.IsLandable ?? DBNull.Value;
                    paramUpdatePlanetTerraformState.Value = (object)pscan.TerraformStateId ?? DBNull.Value;
                    paramUpdatePlanetVolcanismId.Value = (object)pscan.VolcanismId ?? DBNull.Value;
                    paramUpdatePlanetVolcanismMajor.Value = pscan.VolcanismMajor;
                    paramUpdatePlanetVolcanismMinor.Value = pscan.VolcanismMinor;
                    cmdUpdatePlanet.ExecuteNonQuery();
                }

                if (pscan.Atmosphere == null)
                {
                    if (scandata.Planet.HasAtmosphere)
                    {
                        pscan.Atmosphere = new BodyScanAtmosphere(scandata.Atmosphere, pscan.Id);

                        paramAtmosScanId.Value = pscan.Atmosphere.Id;
                        paramSurfacePressure.Value = pscan.Atmosphere.SurfacePressure;
                        paramAtmosComp1Id.Value = (object)pscan.Atmosphere.AtmosphereComponent1Id ?? DBNull.Value;
                        paramAtmosComp1Amt.Value = pscan.Atmosphere.AtmosphereComponent1Amt;
                        paramAtmosComp2Id.Value = (object)pscan.Atmosphere.AtmosphereComponent2Id ?? DBNull.Value;
                        paramAtmosComp2Amt.Value = pscan.Atmosphere.AtmosphereComponent2Amt;
                        paramAtmosComp3Id.Value = (object)pscan.Atmosphere.AtmosphereComponent3Id ?? DBNull.Value;
                        paramAtmosComp3Amt.Value = pscan.Atmosphere.AtmosphereComponent3Amt;
                        paramAtmosNameId.Value = pscan.Atmosphere.AtmosphereId;
                        paramAtmosTypeId.Value = (object)pscan.Atmosphere.AtmosphereTypeId ?? DBNull.Value;
                        paramAtmosHot.Value = pscan.Atmosphere.AtmosphereHot;
                        paramAtmosThin.Value = pscan.Atmosphere.AtmosphereThin;
                        paramAtmosThick.Value = pscan.Atmosphere.AtmosphereThick;
                        cmdInsertAtmos.ExecuteNonQuery();
                    }
                }
                else if ((pscan.Atmosphere.AtmosphereTypeId == null && scandata.Atmosphere.AtmosphereTypeId != 0) ||
                         (pscan.Atmosphere.AtmosphereComponent1Id == null && scandata.Atmosphere.Component1Id != 0))
                {
                    pscan.Atmosphere.AtmosphereTypeId = pscan.Atmosphere.AtmosphereTypeId ?? (scandata.Atmosphere.AtmosphereTypeId != 0 ? (byte?)scandata.Atmosphere.AtmosphereTypeId : null);
                    if (pscan.Atmosphere.AtmosphereComponent1Id == null && scandata.Atmosphere.Component1Id != 0)
                    {
                        pscan.Atmosphere.AtmosphereComponent1Id = scandata.Atmosphere.Component1Id != 0 ? (byte?)scandata.Atmosphere.Component1Id : null;
                        pscan.Atmosphere.AtmosphereComponent1Amt = scandata.Atmosphere.Component1Amt;
                        pscan.Atmosphere.AtmosphereComponent2Id = scandata.Atmosphere.Component2Id != 0 ? (byte?)scandata.Atmosphere.Component2Id : null;
                        pscan.Atmosphere.AtmosphereComponent2Amt = scandata.Atmosphere.Component2Amt;
                        pscan.Atmosphere.AtmosphereComponent3Id = scandata.Atmosphere.Component3Id != 0 ? (byte?)scandata.Atmosphere.Component3Id : null;
                        pscan.Atmosphere.AtmosphereComponent3Amt = scandata.Atmosphere.Component3Amt;
                    }

                    paramAtmosUpdateScanId.Value = pscan.Atmosphere.Id;
                    paramAtmosUpdateTypeId.Value = (object)pscan.Atmosphere.AtmosphereTypeId ?? DBNull.Value;
                    paramAtmosUpdateComp1Id.Value = (object)pscan.Atmosphere.AtmosphereComponent1Id ?? DBNull.Value;
                    paramAtmosUpdateComp1Amt.Value = pscan.Atmosphere.AtmosphereComponent1Amt;
                    paramAtmosUpdateComp2Id.Value = (object)pscan.Atmosphere.AtmosphereComponent2Id ?? DBNull.Value;
                    paramAtmosUpdateComp2Amt.Value = pscan.Atmosphere.AtmosphereComponent2Amt;
                    paramAtmosUpdateComp3Id.Value = (object)pscan.Atmosphere.AtmosphereComponent3Id ?? DBNull.Value;
                    paramAtmosUpdateComp3Amt.Value = pscan.Atmosphere.AtmosphereComponent3Amt;
                    cmdUpdateAtmos.ExecuteNonQuery();
                }

                if (pscan.Materials == null && scandata.Planet.HasMaterials)
                {
                    pscan.Materials = new BodyScanMaterials(scandata.Materials, pscan.Id);
                    paramMatsId.Value = pscan.Materials.Id;
                    paramMatsCarbon.Value = pscan.Materials.MaterialCarbon;
                    paramMatsIron.Value = pscan.Materials.MaterialIron;
                    paramMatsNickel.Value = pscan.Materials.MaterialNickel;
                    paramMatsPhosphorus.Value = pscan.Materials.MaterialPhosphorus;
                    paramMatsSulphur.Value = pscan.Materials.MaterialSulphur;
                    paramMat1Id.Value = pscan.Materials.Material1Id;
                    paramMat1Amt.Value = pscan.Materials.Material1Amt;
                    paramMat2Id.Value = pscan.Materials.Material2Id;
                    paramMat2Amt.Value = pscan.Materials.Material2Amt;
                    paramMat3Id.Value = pscan.Materials.Material3Id;
                    paramMat3Amt.Value = pscan.Materials.Material3Amt;
                    paramMat4Id.Value = pscan.Materials.Material4Id;
                    paramMat4Amt.Value = pscan.Materials.Material4Amt;
                    paramMat5Id.Value = pscan.Materials.Material5Id;
                    paramMat5Amt.Value = pscan.Materials.Material5Amt;
                    paramMat6Id.Value = pscan.Materials.Material6Id;
                    paramMat6Amt.Value = pscan.Materials.Material6Amt;
                    cmdInsertMats.ExecuteNonQuery();
                }
            }
            else if (dbscan is BodyScanStar sscan && scandata.Scan.IsStar)
            {
                if (sscan.LuminosityId == null && scandata.Star.LuminosityId != 0)
                {
                    sscan.LuminosityId = scandata.Star.LuminosityId;
                    paramUpdateStarScanId.Value = sscan.Id;
                    paramUpdateStarLuminosity.Value = sscan.LuminosityId;
                    cmdUpdateStar.ExecuteNonQuery();
                }
            }

            if ((dbscan.Rings.Count == 0 || dbscan.Rings == null) && scandata.RingA.ClassId != 0)
            {
                dbscan.Rings = BodyScanRing.GetRings(scandata);
                foreach (var ring in dbscan.Rings)
                {
                    ring.ScanId = dbscan.Id;
                    paramRingScanId.Value = ring.ScanId;
                    paramRingNum.Value = ring.RingNum;
                    paramRingClass.Value = ring.ClassId;
                    paramRingInnerRad.Value = ring.InnerRad;
                    paramRingOuterRad.Value = ring.OuterRad;
                    paramRingMassMT.Value = ring.MassMT;
                    paramRingIsBelt.Value = ring.IsBelt;
                    cmdInsertRings.ExecuteNonQuery();

                    if (ring.CustomName != null && ring.CustomName.Name != null)
                    {
                        ring.CustomName.ScanId = ring.ScanId;
                        ring.CustomName.RingNum = ring.RingNum;
                        paramRingNameScanId.Value = ring.CustomName.ScanId;
                        paramRingNameNum.Value = ring.CustomName.RingNum;
                        paramRingName.Value = ring.Name;
                        cmdInsertRingName.ExecuteNonQuery();
                    }
                }
            }
        }

        public void Dispose()
        {
            if (cmdUpdateBody != null)
            {
                cmdUpdateBody.Dispose();
                cmdUpdateBody = null;
            }

            if (cmdUpdateAtmos != null)
            {
                cmdUpdateAtmos.Dispose();
                cmdUpdateAtmos = null;
            }

            if (cmdInsertAtmos != null)
            {
                cmdInsertAtmos.Dispose();
                cmdInsertAtmos = null;
            }

            if (cmdInsertMats != null)
            {
                cmdInsertMats.Dispose();
                cmdInsertMats = null;
            }

            if (cmdUpdatePlanet != null)
            {
                cmdUpdatePlanet.Dispose();
                cmdUpdatePlanet = null;
            }

            if (cmdUpdateStar != null)
            {
                cmdUpdateStar.Dispose();
                cmdUpdateStar = null;
            }

            if (cmdInsertRings != null)
            {
                cmdInsertRings.Dispose();
                cmdInsertRings = null;
            }

            if (cmdInsertRingName != null)
            {
                cmdInsertRingName.Dispose();
                cmdInsertRingName = null;
            }

            if (cmdUpdateScan != null)
            {
                cmdUpdateScan.Dispose();
                cmdUpdateScan = null;
            }
        }
    }
}
