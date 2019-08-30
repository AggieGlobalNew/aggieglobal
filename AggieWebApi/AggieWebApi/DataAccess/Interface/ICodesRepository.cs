using System.Collections.Generic;
using AggieGlobal.WebApi.Models.Public;

namespace AggieGlobal.WebApi.DataAccess.Common
{
    public interface ICodesRepository //: IRepository<LK_Codes>
    {
        IEnumerable<LK_Codes> GetByCodeIdentifier(string codeIdentifier);
    }
}
