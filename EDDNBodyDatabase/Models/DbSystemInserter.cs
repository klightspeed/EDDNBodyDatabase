using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data;

namespace EDDNBodyDatabase.Models
{
    public class DbSystemInserter : IDisposable
    {
        Func<DbCommand, int> execsyscmd = null;
        private DbCommand syscmd = null;
        private DbCommand namecmd = null;
        private BodyDbContext Context = null;
        DbParameter paramSysaddr;
        DbParameter paramX;
        DbParameter paramY;
        DbParameter paramZ;
        DbParameter paramRegionId;
        DbParameter paramSizeClass;
        DbParameter paramMid3;
        DbParameter paramMid2;
        DbParameter paramMid1b;
        DbParameter paramMid1a;
        DbParameter paramSequence;
        DbParameter paramEdsmId;
        DbParameter paramEdsmLastModified;
        DbParameter paramNameId;
        DbParameter paramNameSysAddr;
        DbParameter paramCustomName;

        public DbSystemInserter(BodyDbContext ctx)
        {
            Context = ctx;

            string syscmdtext = "INSERT INTO Systems (ModSystemAddress, X, Y, Z, RegionId, SizeClass, Mid3, Mid2, Mid1b, Mid1a, Sequence, EdsmId, EdsmLastModifiedSeconds)";
            string syscmdtext2 = " VALUES (@ModSystemAddress, @X, @Y, @Z, @RegionId, @SizeClass, @Mid3, @Mid2, @Mid1b, @Mid1a, @Sequence, @EdsmId, @EdsmLastModifiedSeconds)";

            execsyscmd = ctx.GetInsertIdentity(ref syscmdtext, ref syscmdtext2);

            if (execsyscmd == null)
            {
                return;
            }

            syscmdtext += syscmdtext2;

            if (ctx.Database.Connection.State != ConnectionState.Open)
            {
                ctx.Database.Connection.Open();
            }

            try
            {
                syscmd = ctx.Database.Connection.CreateCommand();
                syscmd.CommandText = syscmdtext;
                syscmd.CommandType = CommandType.Text;
                paramSysaddr = syscmd.AddParameter("@ModSystemAddress", DbType.Int64);
                paramX = syscmd.AddParameter("@X", DbType.Int32);
                paramY = syscmd.AddParameter("@Y", DbType.Int32);
                paramZ = syscmd.AddParameter("@Z", DbType.Int32);
                paramRegionId = syscmd.AddParameter("@RegionId", DbType.Int16);
                paramSizeClass = syscmd.AddParameter("@SizeClass", DbType.Byte);
                paramMid3 = syscmd.AddParameter("@Mid3", DbType.Byte);
                paramMid2 = syscmd.AddParameter("@Mid2", DbType.Byte);
                paramMid1b = syscmd.AddParameter("@Mid1b", DbType.Byte);
                paramMid1a = syscmd.AddParameter("@Mid1a", DbType.Byte);
                paramSequence = syscmd.AddParameter("@Sequence", DbType.Int16);
                paramEdsmId = syscmd.AddParameter("@EdsmId", DbType.Int32);
                paramEdsmLastModified = syscmd.AddParameter("@EdsmLastModifiedSeconds", DbType.Int32);

                namecmd = ctx.Database.Connection.CreateCommand();
                namecmd.CommandText = "INSERT INTO SystemCustomNames (Id, SystemAddress, CustomName) VALUES (@Id, @SystemAddress, @CustomName)";
                paramNameId = namecmd.AddParameter("@Id", DbType.Int32);
                paramNameSysAddr = namecmd.AddParameter("@SystemAddress", DbType.Int64);
                paramCustomName = namecmd.AddParameter("@CustomName", DbType.String);
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public void Insert(System sys)
        {
            if (syscmd == null)
            {
                Context.Set<System>().Add(sys);
                Context.SaveChanges();
            }
            else
            {
                syscmd.Transaction = Context.Database.CurrentTransaction?.UnderlyingTransaction;
                namecmd.Transaction = Context.Database.CurrentTransaction?.UnderlyingTransaction;
                paramSysaddr.Value = sys.ModSystemAddress;
                paramX.Value = sys.X;
                paramY.Value = sys.Y;
                paramZ.Value = sys.Z;
                paramRegionId.Value = sys.RegionId;
                paramSizeClass.Value = sys.SizeClass;
                paramMid3.Value = sys.Mid3;
                paramMid2.Value = sys.Mid2;
                paramMid1b.Value = sys.Mid1b;
                paramMid1a.Value = sys.Mid1a;
                paramSequence.Value = sys.Sequence;
                paramEdsmId.Value = sys.EdsmId;
                paramEdsmLastModified.Value = sys.EdsmLastModifiedSeconds;
                sys.Id = execsyscmd(syscmd);

                if (sys.SystemCustomName != null)
                {
                    sys.SystemCustomName.Id = sys.Id;
                    paramNameId.Value = sys.SystemCustomName.Id;
                    paramNameSysAddr.Value = sys.SystemCustomName.SystemAddress;
                    paramCustomName.Value = sys.SystemCustomName.CustomName;
                    namecmd.ExecuteNonQuery();
                }
            }
        }

        public void Dispose()
        {
            if (syscmd != null)
            {
                syscmd.Dispose();
            }

            if (namecmd != null)
            {
                namecmd.Dispose();
            }
        }
    }
}
