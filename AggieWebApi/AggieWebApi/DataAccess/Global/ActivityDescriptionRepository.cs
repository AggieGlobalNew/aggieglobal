using AggieGlobal.Models.Client;
using AggieGlobal.Models.Common;
using AggieGlobal.WebApi.Common;
using AggieGlobal.WebApi.DataAccess;
using AggieGlobal.WebApi.DataAccess.Common;
using AggieGlobal.WebApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace AggieWebApi.DataAccess.Global
{
    internal class ActivityDescriptionRepository : SqlDataAccessRepository<ActivityDescriptions>, IActivityDescriptionRepository
    {
        public ActivityDescriptionRepository()
        {

        }

        public ActivityDescriptionRepository(DbTransaction trans)
            : base(trans)
        {

        }

        public IEnumerable<ActivityDescriptions> GetAllActivityDescriptions()
        {
            IEnumerable<ActivityDescriptions> result = null;
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    result = GetRecord("GetAllActivityDescriptions");
                    return result;
                }
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("ActivityDescriptionRepository :: GetAllActivityDescriptions failed :: " + ex.Message);
            }
            return result;
        }
    }
}