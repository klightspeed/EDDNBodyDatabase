using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public class DbScanHeaderInserter : IDisposable
    {
        Func<DbCommand, int> execscancmd = null;
        private BodyDbContext Context;
        DbCommand cmd;
        DbParameter paramBodyScanId;
        DbParameter paramGatewayTimestamp;
        DbParameter paramScanTimestamp;
        DbParameter paramSoftwareVersionId;
        DbParameter paramScanTypeId;
        DbParameter paramDistFromArrival;
        DbParameter paramHasSystemAddress;
        DbParameter paramHasBodyID;
        DbParameter paramHasParents;
        DbParameter paramHasComposition;
        DbParameter paramHasAxialTilt;
        DbParameter paramHasLuminosity;
        DbParameter paramIsMaterialsDict;
        DbParameter paramIsBasicScan;
        DbParameter paramIsPos3SigFig;
        DbParameter paramHasAtmosphereType;
        DbParameter paramHasAtmosphereComposition;
        DbCommand cmdJsonExtra;
        DbParameter paramJsonExtraId;
        DbParameter paramJsonExtra;

        public DbScanHeaderInserter(BodyDbContext ctx)
        {
            Context = ctx;

            string scancmdtext = "INSERT INTO EDDNJournalScans (BodyScanId, GatewayTimestampTicks, ScanTimestampSeconds, SoftwareVersionId, ScanTypeId, DistanceFromArrivalLS, HasSystemAddress, HasBodyID, HasParents, HasComposition, HasAxialTilt, HasLuminosity, IsMaterialsDict, IsBasicScan, IsPos3SigFig, HasAtmosphereType, HasAtmosphereComposition)";
            string scancmdtext2 = " VALUES (@BodyScanId, @GatewayTimestampTicks, @ScanTimestampSeconds, @SoftwareVersionId, @ScanTypeId, @DistanceFromArrivalLS, @HasSystemAddress, @HasBodyID, @HasParents, @HasComposition, @HasAxialTilt, @HasLuminosity, @IsMaterialsDict, @IsBasicScan, @IsPos3SigFig, @HasAtmosphereType, @HasAtmosphereComposition)";

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

            cmd = ctx.Database.Connection.CreateCommand();
            cmd.CommandText = scancmdtext;
            cmd.CommandType = CommandType.Text;
            paramBodyScanId = cmd.AddParameter("@BodyScanId", DbType.Int32);
            paramGatewayTimestamp = cmd.AddParameter("@GatewayTimestampTicks", DbType.Int64);
            paramScanTimestamp = cmd.AddParameter("@ScanTimestampSeconds", DbType.Int32);
            paramSoftwareVersionId = cmd.AddParameter("@SoftwareVersionId", DbType.Int16);
            paramDistFromArrival = cmd.AddParameter("@DistanceFromArrivalLS", DbType.Single);
            paramScanTypeId = cmd.AddParameter("@ScanTypeId", DbType.Byte);
            paramHasSystemAddress = cmd.AddParameter("@HasSystemAddress", DbType.Boolean);
            paramHasBodyID = cmd.AddParameter("@HasBodyID", DbType.Boolean);
            paramHasParents = cmd.AddParameter("@HasParents", DbType.Boolean);
            paramHasComposition = cmd.AddParameter("@HasComposition", DbType.Boolean);
            paramHasAxialTilt = cmd.AddParameter("@HasAxialTilt", DbType.Boolean);
            paramHasLuminosity = cmd.AddParameter("@HasLuminosity", DbType.Boolean);
            paramIsMaterialsDict = cmd.AddParameter("@IsMaterialsDict", DbType.Boolean);
            paramIsBasicScan = cmd.AddParameter("@IsBasicScan", DbType.Boolean);
            paramIsPos3SigFig = cmd.AddParameter("@IsPos3SigFig", DbType.Boolean);
            paramHasAtmosphereType = cmd.AddParameter("@HasAtmosphereType", DbType.Boolean);
            paramHasAtmosphereComposition = cmd.AddParameter("@HasAtmosphereComposition", DbType.Boolean);
            paramGatewayTimestamp.Precision = 6;

            cmdJsonExtra = ctx.Database.Connection.CreateCommand();
            cmdJsonExtra.CommandText = "INSERT INTO EDDNJournalScanJsonExtras (Id, JsonExtra) VALUES (@Id, @JsonExtra)";
            cmdJsonExtra.CommandType = CommandType.Text;
            paramJsonExtraId = cmdJsonExtra.AddParameter("@Id", DbType.Int32);
            paramJsonExtra = cmdJsonExtra.AddParameter("@JsonExtra", DbType.String);
        }

        public void Insert(EDDNJournalScan scan)
        {
            if (cmd == null)
            {
                Context.Set<EDDNJournalScan>().Add(scan);
                Context.SaveChanges();
            }
            else
            {
                cmd.Transaction = Context.Database.CurrentTransaction?.UnderlyingTransaction;
                paramBodyScanId.Value = scan.BodyScanId;
                paramGatewayTimestamp.Value = scan.GatewayTimestampTicks;
                paramScanTimestamp.Value = scan.ScanTimestampSeconds;
                paramSoftwareVersionId.Value = scan.SoftwareVersionId;
                paramScanTypeId.Value = (object)scan.ScanTypeId ?? DBNull.Value;
                paramDistFromArrival.Value = scan.DistanceFromArrivalLS;
                paramHasSystemAddress.Value = (object)scan.HasSystemAddress ?? DBNull.Value;
                paramHasBodyID.Value = scan.HasBodyID;
                paramHasParents.Value = scan.HasParents;
                paramHasComposition.Value = scan.HasComposition;
                paramHasAxialTilt.Value = scan.HasAxialTilt;
                paramHasLuminosity.Value = scan.HasLuminosity;
                paramIsMaterialsDict.Value = scan.IsMaterialsDict;
                paramIsBasicScan.Value = scan.IsBasicScan;
                paramIsPos3SigFig.Value = scan.IsPos3SigFig;
                paramHasAtmosphereType.Value = scan.HasAtmosphereType;
                paramHasAtmosphereComposition.Value = scan.HasAtmosphereComposition;
                scan.Id = execscancmd(cmd);

                if (scan.JsonExtraRef != null)
                {
                    scan.JsonExtraRef.Id = scan.Id;
                    paramJsonExtraId.Value = scan.JsonExtraRef.Id;
                    paramJsonExtra.Value = scan.JsonExtraRef.JsonExtra;
                }
            }
        }

        public void Dispose()
        {
            if (cmd != null)
            {
                cmd.Dispose();
            }
        }
    }
}
