using System.Collections.Generic;
using AggieGlobal.WebApi.DataAccess.Common;
using AggieGlobal.WebApi.Models.Public;

namespace AggieGlobal.WebApi.DataAccess
{
    internal class CodesRepository : SqlDataAccessRepository<LK_Codes>, ICodesRepository
    {
        public IEnumerable<LK_Codes> GetByCodeIdentifier(string codeIdentifier)
        {
            return GetRecord("GetByCodeIdentifier", codeIdentifier);
        }
    }
}