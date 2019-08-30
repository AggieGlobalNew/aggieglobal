using System;
using System.Collections.Generic;
using System.Data.Common;
using AggieGlobal.Models.Client;
using AggieGlobal.WebApi.Models.Client;

namespace AggieGlobal.WebApi.DataAccess.Common
{
    public interface IActivityRepository
    {
        bool CreateActivity(ActivityDetail prodData);
        bool UpdateActivity(ActivityDetail prodData);
        bool DeleteActivity(int productId);
        IEnumerable<ActivityDetail> GetActivityListByMonth(int userID, string dateStamp);
        IEnumerable<ActivityDetail> GetActivityCountByDate(int userID, string dateStamp);
        IEnumerable<ActivityDetail> GetActivityById(int ActivityId);
    }
}