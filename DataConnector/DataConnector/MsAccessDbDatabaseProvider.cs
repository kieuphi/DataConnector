using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataConnector
{
    public class MsAccessDbDatabaseProvider : DatabaseProvider
    {
        public override DbProviderFactory GetFactory()
        {
            return DbProviderFactories.GetFactory("System.Data.OleDb");
        }

        public override object ExecuteInsert(Database database, IDbCommand cmd, string primaryKeyName)
        {
            ExecuteNonQueryHelper(database, cmd);
            cmd.CommandText = "SELECT @@IDENTITY AS NewID;";
            return ExecuteScalarHelper(database, cmd);
        }

        public override string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            throw new NotSupportedException("The Access provider does not support paging.");
        }
    }
}