using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace AggieGlobal.WebApi.DataAccess.Common
{
    public abstract class BaseDataAccessRepository
    {
        protected string _connectionStringName = null;

        protected BaseDataAccessRepository(string connectionStringName)
        {
            this._connectionStringName = connectionStringName;
        }

        protected DbConnection GetConnection()
        {
            return DatabaseFactory.CreateDatabase(_connectionStringName).CreateConnection();
        }
    }
}