using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace EDDNBodyDatabase.Models
{
    public static class DbExtensions
    {
        public static DbParameter AddParameter<T>(this DbCommand cmd, string name, DbType type, T val)
        {
            var param = AddParameter(cmd, name, type);
            param.Value = val;
            return param;
        }

        public static DbParameter AddParameter(this DbCommand cmd, string name, DbType type)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = name;
            param.DbType = type;
            cmd.Parameters.Add(param);
            return param;
        }
    }
}
