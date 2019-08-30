using System.Collections.Generic;

namespace AggieGlobal.WebApi.DataAccess.Common
{
    internal class DataRepository : SqlDataAccessRepository<dynamic>
    {
        internal DataRepository(string connectionString)
            : base(connectionString)
        {}

        public IEnumerable<dynamic> GetQueryResult(string query)
        {
            //return GetData(query);
            return new List<dynamic>();
        }

        public int ExecuteQuery(string query)
        {
            //return Execute(query);
            return 0;
        }
    }
}