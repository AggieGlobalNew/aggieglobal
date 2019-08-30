using AggieGlobal.Models.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AggieGlobal.WebApi.DataAccess.Common
{
    public interface ISoilRepository
    {
        IEnumerable<SoilDetailResponse> GetSoilDetails();
    }
}