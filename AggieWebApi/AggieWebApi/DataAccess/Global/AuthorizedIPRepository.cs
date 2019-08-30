using AggieGlobal.WebApi.DataAccess.Common;
using AggieGlobal.WebApi.Models.Client;
using System;

namespace AggieGlobal.WebApi.DataAccess.Global
{
    internal class AuthorizedIPRepository : SqlDataAccessRepository<AuthorizedIP>, IAuthorizedIPRepository
    {
        public AuthorizedIP GetByAccountId(int pwAccountId, int pcModuleId)
        {
            throw new NotImplementedException();
        }
    }
}