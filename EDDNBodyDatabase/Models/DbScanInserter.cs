using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;


namespace EDDNBodyDatabase.Models
{
    public class DbScanInserter : IDisposable
    {
        Func<DbCommand, int> execscancmd = null;
        private BodyDbContext Context;

        DbCommand cmdScan = null;
        DbParameter paramSystemBodyId;
        DbParameter paramParentSetId;
        DbParameter paramScanBaseHash;
        DbParameter paramAxialTilt;
        DbParameter paramEccentricity;
        DbParameter paramOrbitalInclination;
        DbParameter paramPeriapsis;
        DbParameter paramSemiMajorAxis;
        DbParameter paramOrbitalPeriod;
        DbParameter paramRadius;
        DbParameter paramRotationPeriod;
        DbParameter paramSurfaceTemperature;
        DbParameter paramHasOrbit;
        DbParameter paramTidalLock;
        DbParameter paramReserveLevel;

        DbCommand cmdRing = null;
        DbParameter paramRingScanId;
        DbParameter paramRingNum;
        DbParameter paramRingClass;
        DbParameter paramRingInnerRad;
        DbParameter paramRingOuterRad;
        DbParameter paramRingMassMT;
        DbParameter paramRingIsBelt;

        DbCommand cmdRingName = null;
        DbParameter paramRingNameScanId;
        DbParameter paramRingNameNum;
        DbParameter paramRingName;

        DbCommand cmdStar = null;
        DbParameter paramStarId;
        DbParameter paramAbsoluteMagnitude;
        DbParameter paramStellarMass;
        DbParameter paramAgeMY;
        DbParameter paramStarTypeId;
        DbParameter paramLuminosityId;

        DbCommand cmdPlanet = null;
        DbParameter paramPlanetId;
        DbParameter paramPlanetClass;
        DbParameter paramCompositionMetal;
        DbParameter paramCompositionRock;
        DbParameter paramCompositionIce;
        DbParameter paramPlanetHasComposition;
        DbParameter paramPlanetMassEM;
        DbParameter paramSurfaceGravity;
        DbParameter paramVolcanismId;
        DbParameter paramVolcanismMinor;
        DbParameter paramVolcanismMajor;
        DbParameter paramIsLandable;

        DbCommand cmdMats = null;
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

        DbCommand cmdAtmos = null;
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

        public DbScanInserter(BodyDbContext ctx)
        {
            Context = ctx;

            string scancmdtext = "INSERT INTO BodyScans (SystemBodyId, ParentSetId, ScanBaseHash, AxialTilt, Eccentricity, OrbitalInclination, Periapsis, SemiMajorAxis, OrbitalPeriod, Radius, RotationPeriod, SurfaceTemperature, HasOrbit, TidalLock, ReserveLevelId)";
            string scancmdtext2 = " VALUES (@SystemBodyId, @ParentSetId, @ScanBaseHash, @AxialTilt, @Eccentricity, @OrbitalInclination, @Periapsis, @SemiMajorAxis, @OrbitalPeriod, @Radius, @RotationPeriod, @SurfaceTemperature, @HasOrbit, @TidalLock, @ReserveLevel)";

            execscancmd = ctx.GetInsertIdentity(ref scancmdtext, ref scancmdtext2);

            if (execscancmd == null)
            {
                return;
            }

            scancmdtext += scancmdtext2;

            if (ctx.Database.Connection.State != ConnectionState.Open)
            {
                ctx.Database.Connection.Open();
            }

            try
            {
                cmdScan = ctx.Database.Connection.CreateCommand();
                cmdScan.CommandText = scancmdtext;
                cmdScan.CommandType = CommandType.Text;

                paramSystemBodyId = cmdScan.AddParameter("@SystemBodyId", DbType.Int32);
                paramParentSetId = cmdScan.AddParameter("@ParentSetId", DbType.Int32);
                paramScanBaseHash = cmdScan.AddParameter("@ScanBaseHash", DbType.Int32);
                paramAxialTilt = cmdScan.AddParameter("@AxialTilt", DbType.Single);
                paramEccentricity = cmdScan.AddParameter("@Eccentricity", DbType.Single);
                paramOrbitalInclination = cmdScan.AddParameter("@OrbitalInclination", DbType.Single);
                paramPeriapsis = cmdScan.AddParameter("@Periapsis", DbType.Single);
                paramSemiMajorAxis = cmdScan.AddParameter("@SemiMajorAxis", DbType.Single);
                paramOrbitalPeriod = cmdScan.AddParameter("@OrbitalPeriod", DbType.Single);
                paramRadius = cmdScan.AddParameter("@Radius", DbType.Single);
                paramRotationPeriod = cmdScan.AddParameter("@RotationPeriod", DbType.Single);
                paramSurfaceTemperature = cmdScan.AddParameter("@SurfaceTemperature", DbType.Single);
                paramHasOrbit = cmdScan.AddParameter("@HasOrbit", DbType.Boolean);
                paramTidalLock = cmdScan.AddParameter("@TidalLock", DbType.Boolean);
                paramReserveLevel = cmdScan.AddParameter("@ReserveLevel", DbType.Byte);

                cmdRing = ctx.Database.Connection.CreateCommand();
                cmdRing.CommandText =
                    "INSERT INTO BodyScanRings (ScanId, RingNum, ClassId, InnerRad, OuterRad, MassMT, IsBelt) " +
                    "VALUES (@ScanId, @RingNum, @ClassId, @InnerRad, @OuterRad, @MassMT, @IsBelt)";
                cmdRing.CommandType = CommandType.Text;

                paramRingScanId = cmdRing.AddParameter("@ScanId", DbType.Int32);
                paramRingNum = cmdRing.AddParameter("@RingNum", DbType.Byte);
                paramRingClass = cmdRing.AddParameter("@ClassId", DbType.Byte);
                paramRingInnerRad = cmdRing.AddParameter("@InnerRad", DbType.Single);
                paramRingOuterRad = cmdRing.AddParameter("@OuterRad", DbType.Single);
                paramRingMassMT = cmdRing.AddParameter("@MassMT", DbType.Single);
                paramRingIsBelt = cmdRing.AddParameter("@IsBelt", DbType.Boolean);

                cmdRingName = ctx.Database.Connection.CreateCommand();
                cmdRingName.CommandText = "INSERT INTO BodyScanRingCustomNames (ScanId, RingNum, Name) VALUES (@ScanId, @RingNum, @Name)";
                cmdRingName.CommandType = CommandType.Text;

                paramRingNameScanId = cmdRingName.AddParameter("@ScanId", DbType.Int32);
                paramRingNameNum = cmdRingName.AddParameter("@RingNum", DbType.Byte);
                paramRingName = cmdRingName.AddParameter("@Name", DbType.String);

                cmdStar = ctx.Database.Connection.CreateCommand();
                cmdStar.CommandText =
                    "INSERT INTO BodyScanStars (Id, AbsoluteMagnitude, StellarMass, Age_MY, StarTypeId, LuminosityId) VALUES " +
                    "(@Id, @AbsoluteMagnitude, @StellarMass, @Age_MY, @StarTypeId, @LuminosityId)";
                cmdStar.CommandType = CommandType.Text;

                paramStarId = cmdStar.AddParameter("@Id", DbType.Int32);
                paramAbsoluteMagnitude = cmdStar.AddParameter("@AbsoluteMagnitude", DbType.Single);
                paramStellarMass = cmdStar.AddParameter("@StellarMass", DbType.Single);
                paramAgeMY = cmdStar.AddParameter("@Age_MY", DbType.Int16);
                paramStarTypeId = cmdStar.AddParameter("@StarTypeId", DbType.Byte);
                paramLuminosityId = cmdStar.AddParameter("@LuminosityId", DbType.Byte);

                cmdPlanet = ctx.Database.Connection.CreateCommand();
                cmdPlanet.CommandText =
                    "INSERT INTO BodyScanPlanets (Id, PlanetClassId, CompositionMetal, CompositionRock, CompositionIce, HasComposition, MassEM, SurfaceGravity, VolcanismId, VolcanismMinor, VolcanismMajor, IsLandable) VALUES " +
                    "(@Id, @PlanetClassId, @CompositionMetal, @CompositionRock, @CompositionIce, @HasComposition, @MassEM, @SurfaceGravity, @VolcanismId, @VolcanismMinor, @VolcanismMajor, @IsLandable)";
                cmdPlanet.CommandType = CommandType.Text;

                paramPlanetId = cmdPlanet.AddParameter("@Id", DbType.Int32);
                paramPlanetClass = cmdPlanet.AddParameter("@PlanetClassId", DbType.Byte);
                paramCompositionMetal = cmdPlanet.AddParameter("@CompositionMetal", DbType.Single);
                paramCompositionRock = cmdPlanet.AddParameter("@CompositionRock", DbType.Single);
                paramCompositionIce = cmdPlanet.AddParameter("@CompositionIce", DbType.Single);
                paramPlanetHasComposition = cmdPlanet.AddParameter("@HasComposition", DbType.Boolean);
                paramPlanetMassEM = cmdPlanet.AddParameter("@MassEM", DbType.Single);
                paramSurfaceGravity = cmdPlanet.AddParameter("@SurfaceGravity", DbType.Single);
                paramVolcanismId = cmdPlanet.AddParameter("@VolcanismId", DbType.Byte);
                paramVolcanismMinor = cmdPlanet.AddParameter("@VolcanismMinor", DbType.Boolean);
                paramVolcanismMajor = cmdPlanet.AddParameter("@VolcanismMajor", DbType.Boolean);
                paramIsLandable = cmdPlanet.AddParameter("@IsLandable", DbType.Boolean);

                cmdMats = ctx.Database.Connection.CreateCommand();
                cmdMats.CommandText =
                    "INSERT INTO BodyScanMaterials (Id, MaterialCarbon, MaterialIron, MaterialNickel, MaterialPhosphorus, MaterialSulphur, Material1Id, Material1Amt, Material2Id, Material2Amt, Material3Id, Material3Amt, Material4Id, Material4Amt, Material5Id, Material5Amt, Material6Id, Material6Amt) VALUES " +
                    "(@Id, @MaterialCarbon, @MaterialIron, @MaterialNickel, @MaterialPhosphorus, @MaterialSulphur, @Material1Id, @Material1Amt, @Material2Id, @Material2Amt, @Material3Id, @Material3Amt, @Material4Id, @Material4Amt, @Material5Id, @Material5Amt, @Material6Id, @Material6Amt)";
                cmdMats.CommandType = CommandType.Text;

                paramMatsId = cmdMats.AddParameter("@Id", DbType.Int32);
                paramMatsCarbon = cmdMats.AddParameter("@MaterialCarbon", DbType.Single);
                paramMatsIron = cmdMats.AddParameter("@MaterialIron", DbType.Single);
                paramMatsNickel = cmdMats.AddParameter("@MaterialNickel", DbType.Single);
                paramMatsPhosphorus = cmdMats.AddParameter("@MaterialPhosphorus", DbType.Single);
                paramMatsSulphur = cmdMats.AddParameter("@MaterialSulphur", DbType.Single);
                paramMat1Id = cmdMats.AddParameter("@Material1Id", DbType.Byte);
                paramMat1Amt = cmdMats.AddParameter("@Material1Amt", DbType.Single);
                paramMat2Id = cmdMats.AddParameter("@Material2Id", DbType.Byte);
                paramMat2Amt = cmdMats.AddParameter("@Material2Amt", DbType.Single);
                paramMat3Id = cmdMats.AddParameter("@Material3Id", DbType.Byte);
                paramMat3Amt = cmdMats.AddParameter("@Material3Amt", DbType.Single);
                paramMat4Id = cmdMats.AddParameter("@Material4Id", DbType.Byte);
                paramMat4Amt = cmdMats.AddParameter("@Material4Amt", DbType.Single);
                paramMat5Id = cmdMats.AddParameter("@Material5Id", DbType.Byte);
                paramMat5Amt = cmdMats.AddParameter("@Material5Amt", DbType.Single);
                paramMat6Id = cmdMats.AddParameter("@Material6Id", DbType.Byte);
                paramMat6Amt = cmdMats.AddParameter("@Material6Amt", DbType.Single);

                cmdAtmos = ctx.Database.Connection.CreateCommand();
                cmdAtmos.CommandText =
                    "INSERT INTO BodyScanAtmospheres (Id, SurfacePressure, AtmosphereComponent1Id, AtmosphereComponent1Amt, AtmosphereComponent2Id, AtmosphereComponent2Amt, AtmosphereComponent3Id, AtmosphereComponent3Amt, AtmosphereId, AtmosphereTypeId, AtmosphereHot, AtmosphereThin, AtmosphereThick) VALUES " +
                    "(@Id, @SurfacePressure, @AtmosphereComponent1Id, @AtmosphereComponent1Amt, @AtmosphereComponent2Id, @AtmosphereComponent2Amt, @AtmosphereComponent3Id, @AtmosphereComponent3Amt, @AtmosphereId, @AtmosphereTypeId, @AtmosphereHot, @AtmosphereThin, @AtmosphereThick)";
                cmdAtmos.CommandType = CommandType.Text;

                paramAtmosScanId = cmdAtmos.AddParameter("@Id", DbType.Int32);
                paramSurfacePressure = cmdAtmos.AddParameter("@SurfacePressure", DbType.Single);
                paramAtmosComp1Id = cmdAtmos.AddParameter("@AtmosphereComponent1Id", DbType.Byte);
                paramAtmosComp1Amt = cmdAtmos.AddParameter("@AtmosphereComponent1Amt", DbType.Single);
                paramAtmosComp2Id = cmdAtmos.AddParameter("@AtmosphereComponent2Id", DbType.Byte);
                paramAtmosComp2Amt = cmdAtmos.AddParameter("@AtmosphereComponent2Amt", DbType.Single);
                paramAtmosComp3Id = cmdAtmos.AddParameter("@AtmosphereComponent3Id", DbType.Byte);
                paramAtmosComp3Amt = cmdAtmos.AddParameter("@AtmosphereComponent3Amt", DbType.Single);
                paramAtmosNameId = cmdAtmos.AddParameter("@AtmosphereId", DbType.Byte);
                paramAtmosTypeId = cmdAtmos.AddParameter("@AtmosphereTypeId", DbType.Byte);
                paramAtmosHot = cmdAtmos.AddParameter("@AtmosphereHot", DbType.Boolean);
                paramAtmosThin = cmdAtmos.AddParameter("@AtmosphereThin", DbType.Boolean);
                paramAtmosThick = cmdAtmos.AddParameter("@AtmosphereThick", DbType.Boolean);
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public void Insert(BodyScan scan)
        {
            if (cmdScan == null)
            {
                Context.Set<BodyScan>().Add(scan);
                Context.SaveChanges();
            }
            else
            {
                cmdScan.Transaction = Context.Database.CurrentTransaction?.UnderlyingTransaction;
                cmdRing.Transaction = Context.Database.CurrentTransaction?.UnderlyingTransaction;
                cmdStar.Transaction = Context.Database.CurrentTransaction?.UnderlyingTransaction;
                cmdPlanet.Transaction = Context.Database.CurrentTransaction?.UnderlyingTransaction;
                cmdMats.Transaction = Context.Database.CurrentTransaction?.UnderlyingTransaction;
                cmdAtmos.Transaction = Context.Database.CurrentTransaction?.UnderlyingTransaction;

                paramSystemBodyId.Value = scan.SystemBodyId;
                paramParentSetId.Value = (object)scan.ParentSetId ?? DBNull.Value;
                paramScanBaseHash.Value = scan.ScanBaseHash;
                paramAxialTilt.Value = (object)scan.AxialTilt ?? DBNull.Value;
                paramEccentricity.Value = scan.Eccentricity;
                paramOrbitalInclination.Value = scan.OrbitalInclination;
                paramPeriapsis.Value = scan.Periapsis;
                paramSemiMajorAxis.Value = scan.SemiMajorAxis;
                paramOrbitalPeriod.Value = scan.OrbitalPeriod;
                paramRadius.Value = scan.Radius;
                paramRotationPeriod.Value = scan.RotationPeriod;
                paramSurfaceTemperature.Value = scan.SurfaceTemperature;
                paramHasOrbit.Value = scan.HasOrbit;
                paramTidalLock.Value = (object)scan.TidalLock ?? DBNull.Value;
                paramReserveLevel.Value = (object)scan.ReserveLevelId ?? DBNull.Value;
                scan.Id = execscancmd(cmdScan);

                if (scan.Rings != null)
                {
                    foreach (var ring in scan.Rings)
                    {
                        ring.ScanId = scan.Id;
                        paramRingScanId.Value = ring.ScanId;
                        paramRingNum.Value = ring.RingNum;
                        paramRingClass.Value = ring.ClassId;
                        paramRingInnerRad.Value = ring.InnerRad;
                        paramRingOuterRad.Value = ring.OuterRad;
                        paramRingMassMT.Value = ring.MassMT;
                        paramRingIsBelt.Value = ring.IsBelt;
                        cmdRing.ExecuteNonQuery();

                        if (ring.CustomName != null && ring.CustomName.Name != null)
                        {
                            ring.CustomName.ScanId = ring.ScanId;
                            ring.CustomName.RingNum = ring.RingNum;
                            paramRingNameScanId.Value = ring.CustomName.ScanId;
                            paramRingNameNum.Value = ring.CustomName.RingNum;
                            paramRingName.Value = ring.Name;
                            cmdRingName.ExecuteNonQuery();
                        }
                    }
                }

                if (scan is BodyScanStar star)
                {
                    paramStarId.Value = star.Id;
                    paramAbsoluteMagnitude.Value = star.AbsoluteMagnitude;
                    paramStellarMass.Value = star.StellarMass;
                    paramAgeMY.Value = star.Age_MY;
                    paramStarTypeId.Value = star.StarTypeId;
                    paramLuminosityId.Value = (object)star.LuminosityId ?? DBNull.Value;
                    cmdStar.ExecuteNonQuery();
                }
                else if (scan is BodyScanPlanet planet)
                {
                    paramPlanetId.Value = planet.Id;
                    paramPlanetClass.Value = planet.PlanetClassId;
                    paramCompositionMetal.Value = planet.CompositionMetal;
                    paramCompositionRock.Value = planet.CompositionRock;
                    paramCompositionIce.Value = planet.CompositionIce;
                    paramPlanetHasComposition.Value = planet.HasComposition;
                    paramPlanetMassEM.Value = planet.MassEM;
                    paramSurfaceGravity.Value = planet.SurfaceGravity;
                    paramVolcanismId.Value = (object)planet.VolcanismId ?? DBNull.Value;
                    paramVolcanismMinor.Value = planet.VolcanismMinor;
                    paramVolcanismMajor.Value = planet.VolcanismMajor;
                    paramIsLandable.Value = (object)planet.IsLandable ?? DBNull.Value;
                    cmdPlanet.ExecuteNonQuery();

                    if (planet.Materials != null)
                    {
                        planet.Materials.Id = scan.Id;
                        paramMatsId.Value = planet.Materials.Id;
                        paramMatsCarbon.Value = planet.Materials.MaterialCarbon;
                        paramMatsIron.Value = planet.Materials.MaterialIron;
                        paramMatsNickel.Value = planet.Materials.MaterialNickel;
                        paramMatsPhosphorus.Value = planet.Materials.MaterialPhosphorus;
                        paramMatsSulphur.Value = planet.Materials.MaterialSulphur;
                        paramMat1Id.Value = planet.Materials.Material1Id;
                        paramMat1Amt.Value = planet.Materials.Material1Amt;
                        paramMat2Id.Value = planet.Materials.Material2Id;
                        paramMat2Amt.Value = planet.Materials.Material2Amt;
                        paramMat3Id.Value = planet.Materials.Material3Id;
                        paramMat3Amt.Value = planet.Materials.Material3Amt;
                        paramMat4Id.Value = planet.Materials.Material4Id;
                        paramMat4Amt.Value = planet.Materials.Material4Amt;
                        paramMat5Id.Value = planet.Materials.Material5Id;
                        paramMat5Amt.Value = planet.Materials.Material5Amt;
                        paramMat6Id.Value = planet.Materials.Material6Id;
                        paramMat6Amt.Value = planet.Materials.Material6Amt;
                        cmdMats.ExecuteNonQuery();
                    }

                    if (planet.Atmosphere != null)
                    {
                        planet.Atmosphere.Id = scan.Id;
                        paramAtmosScanId.Value = planet.Atmosphere.Id;
                        paramSurfacePressure.Value = planet.Atmosphere.SurfacePressure;
                        paramAtmosComp1Id.Value = (object)planet.Atmosphere.AtmosphereComponent1Id ?? DBNull.Value;
                        paramAtmosComp1Amt.Value = planet.Atmosphere.AtmosphereComponent1Amt;
                        paramAtmosComp2Id.Value = (object)planet.Atmosphere.AtmosphereComponent2Id ?? DBNull.Value;
                        paramAtmosComp2Amt.Value = planet.Atmosphere.AtmosphereComponent2Amt;
                        paramAtmosComp3Id.Value = (object)planet.Atmosphere.AtmosphereComponent3Id ?? DBNull.Value;
                        paramAtmosComp3Amt.Value = planet.Atmosphere.AtmosphereComponent3Amt;
                        paramAtmosNameId.Value = planet.Atmosphere.AtmosphereId;
                        paramAtmosTypeId.Value = (object)planet.Atmosphere.AtmosphereTypeId ?? DBNull.Value;
                        paramAtmosHot.Value = planet.Atmosphere.AtmosphereHot;
                        paramAtmosThin.Value = planet.Atmosphere.AtmosphereThin;
                        paramAtmosThick.Value = planet.Atmosphere.AtmosphereThick;
                        cmdAtmos.ExecuteNonQuery();
                    }
                }
            }
        }

        public void Dispose()
        {
            if (cmdScan != null)
            {
                cmdScan.Dispose();
                cmdScan = null;
            }

            if (cmdRing != null)
            {
                cmdRing.Dispose();
                cmdRing = null;
            }

            if (cmdRingName != null)
            {
                cmdRingName.Dispose();
                cmdRingName = null;
            }

            if (cmdStar != null)
            {
                cmdStar.Dispose();
                cmdStar = null;
            }

            if (cmdPlanet != null)
            {
                cmdPlanet.Dispose();
                cmdPlanet = null;
            }

            if (cmdMats != null)
            {
                cmdMats.Dispose();
                cmdMats = null;
            }

            if (cmdAtmos != null)
            {
                cmdAtmos.Dispose();
                cmdAtmos = null;
            }
        }
    }
}
