using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDNBodyDatabase.Models
{
    public class DbBodyInserter : IDisposable
    {
        Func<DbCommand, int> execbodycmd = null;
        private BodyDbContext Context;
        DbCommand bodycmd = null;
        DbCommand namecmd = null;
        DbParameter paramSystemId;
        DbParameter paramBodyID;
        DbParameter paramStars;
        DbParameter paramIsBelt;
        DbParameter paramPlanet;
        DbParameter paramMoon1;
        DbParameter paramMoon2;
        DbParameter paramMoon3;
        DbParameter paramScanBaseHash;
        DbParameter paramCustomNameId;
        DbParameter paramNameId;
        DbParameter paramNameSystemId;
        DbParameter paramNameBodyID;
        DbParameter paramCustomName;

        public DbBodyInserter(BodyDbContext ctx)
        {
            Context = ctx;

            string bodycmdtext = "INSERT INTO SystemBodies (SystemId, BodyID, Stars, Planet, Moon1, Moon2, Moon3, IsBelt, ScanBaseHash, CustomNameId)";
            string bodycmdtext2 = " VALUES (@SystemId, @BodyID, @Stars, @Planet, @Moon1, @Moon2, @Moon3, @IsBelt, @ScanBaseHash, @CustomNameId)";

            execbodycmd = ctx.GetInsertIdentity(ref bodycmdtext, ref bodycmdtext2);

            if (execbodycmd == null)
            {
                return;
            }

            bodycmdtext += bodycmdtext2;

            if (ctx.Database.Connection.State != ConnectionState.Open)
            {
                ctx.Database.Connection.Open();
            }

            try
            {
                bodycmd = ctx.Database.Connection.CreateCommand();
                bodycmd.CommandText = bodycmdtext;
                paramSystemId = bodycmd.AddParameter("@SystemId", DbType.Int32);
                paramBodyID = bodycmd.AddParameter("@BodyID", DbType.Int16);
                paramStars = bodycmd.AddParameter("@Stars", DbType.Byte);
                paramIsBelt = bodycmd.AddParameter("@IsBelt", DbType.Boolean);
                paramPlanet = bodycmd.AddParameter("@Planet", DbType.Byte);
                paramMoon1 = bodycmd.AddParameter("@Moon1", DbType.Byte);
                paramMoon2 = bodycmd.AddParameter("@Moon2", DbType.Byte);
                paramMoon3 = bodycmd.AddParameter("@Moon3", DbType.Byte);
                paramScanBaseHash = bodycmd.AddParameter("@ScanBaseHash", DbType.Int32);
                paramCustomNameId = bodycmd.AddParameter("@CustomNameId", DbType.Int16);

                namecmd = ctx.Database.Connection.CreateCommand();
                namecmd.CommandText = "INSERT INTO SystemBodyCustomNames (Id, SystemId, BodyID, CustomName) VALUES (@Id, @SystemId, @BodyID, @CustomName)";
                paramNameId = namecmd.AddParameter("@Id", DbType.Int32);
                paramNameSystemId = namecmd.AddParameter("@SystemId", DbType.Int32);
                paramNameBodyID = namecmd.AddParameter("@BodyID", DbType.Int16);
                paramCustomName = namecmd.AddParameter("@CustomName", DbType.String);
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public void Insert(SystemBody body)
        {
            if (bodycmd == null)
            {
                Context.Set<SystemBody>().Add(body);
                Context.SaveChanges();
            }
            else
            {
                bodycmd.Transaction = Context.Database.CurrentTransaction?.UnderlyingTransaction;
                namecmd.Transaction = Context.Database.CurrentTransaction?.UnderlyingTransaction;
                paramSystemId.Value = body.SystemId;
                paramBodyID.Value = body.BodyID;
                paramStars.Value = body.Stars;
                paramIsBelt.Value = body.IsBelt;
                paramPlanet.Value = body.Planet;
                paramMoon1.Value = body.Moon1;
                paramMoon2.Value = body.Moon2;
                paramMoon3.Value = body.Moon3;
                paramScanBaseHash.Value = body.ScanBaseHash;
                paramCustomNameId.Value = body.CustomNameId;
                body.Id = execbodycmd(bodycmd);

                if (body.CustomName != null)
                {
                    body.CustomName.Id = body.Id;
                    paramNameId.Value = body.Id;
                    paramNameSystemId.Value = body.SystemId;
                    paramNameBodyID.Value = body.CustomName.BodyID;
                    paramCustomName.Value = body.CustomName.CustomName;
                    namecmd.ExecuteNonQuery();
                }
            }
        }

        public void Dispose()
        {
            if (bodycmd != null)
            {
                bodycmd.Dispose();
                bodycmd = null;
            }

            if (namecmd != null)
            {
                namecmd.Dispose();
                namecmd = null;
            }
        }
    }
}
