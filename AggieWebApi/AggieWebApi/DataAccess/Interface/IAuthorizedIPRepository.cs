using AggieGlobal.WebApi.Models.Client;

namespace AggieGlobal.WebApi.DataAccess.Common
{
    public interface IAuthorizedIPRepository
    {
        AuthorizedIP GetByAccountId(int pwAccountId, int pcModuleId);
    }
}
