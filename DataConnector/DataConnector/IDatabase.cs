using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataConnector
{
    /// <summary>
    ///     Specifies the database contract.
    /// </summary>
    public interface IDatabase : IDisposable, IQuery, IAlterPoco, IExecute, ITransactionAccessor
    {
    }
}