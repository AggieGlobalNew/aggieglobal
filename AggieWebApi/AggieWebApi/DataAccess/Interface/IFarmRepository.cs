using System;
using System.Collections.Generic;
using System.Data.Common;
using AggieGlobal.Models.Client;
using AggieGlobal.WebApi.Models.Client;

namespace AggieGlobal.WebApi.DataAccess.Common
{
    public interface IFarmRepository
    {
        int CreateUpdateFarm(FarmDetail requestData);
        IEnumerable<FarmDetail> GetFarmDetails(int userid);
        bool MapFarmByUserDetail(int FarmId, int UserId);
        IList<SoilDataResponse> GetSoilDetails();
        IEnumerable<FarmDetail> GetInActiveFarmDetails();
    }
}
