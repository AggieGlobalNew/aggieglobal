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
    internal class ActivityRepository : SqlDataAccessRepository<ActivityDetail>, IActivityRepository
    {
        public ActivityRepository()
        {

        }

        public ActivityRepository(DbTransaction trans)
            : base(trans)
        {

        }
        public bool CreateActivity(ActivityDetail prodData)
        {
            DbTransaction transaction = null;
            bool result = default(bool);
            int AuthenticationSuccessmode = 0;
            using (var connection = GetConnection())
            {
                try
                {
                    connection.Open();
                    if (prodData.ProductTypeId == ProductType.Crop.GetHashCode())
                        prodData.ActivityId = CreateRecord("CreateActivityCrop", prodData.UserId, prodData.ProductId, prodData.CategoryId, prodData.LastHarvestedDate, prodData.IsHarvestedBefore,
                                prodData.IsSoldBefore, prodData.IsSoldBeforeNoReason, prodData.SoldPrice, prodData.ActivityDate, prodData.PlantationDate,prodData.PlotId, 
                                prodData.ActivityDescriptionId, AuthenticationSuccessmode);
                    else if (prodData.ProductTypeId == ProductType.LiveStock.GetHashCode())
                                prodData.ActivityId = CreateRecord("CreateActivityLiveStock", prodData.UserId, prodData.ProductId, prodData.CategoryId,
                                prodData.NumberOfLivestock, prodData.LiveStockUsageId, prodData.LiveStockUtilityId,
                                prodData.IsLivestockSalable, prodData.LastDateOfLivestockSold, prodData.LivestocksellingLocationId, prodData.ActivityDate,
                                prodData.PlotId, prodData.ActivityDescriptionId, prodData.LivestocksellingLocationName, prodData.LiveStockName,
                                prodData.LiveStockUsageName, prodData.LiveStockUtilityName,prodData.SoldLiveStockAmount, AuthenticationSuccessmode);
                    else if (prodData.ProductTypeId == ProductType.Resource.GetHashCode())
                             prodData.ActivityId = CreateRecord("CreateActivityResources", prodData.UserId, prodData.ProductId,prodData.CategoryId, prodData.NumberOfResource,prodData.ResourceCostTypeId,
                             prodData.ResourceTypeId, prodData.ResourceTypeName,prodData.ResourceMaintenancePrice,prodData.ResourcePrice,prodData.ActivityDate,prodData.PlotId, prodData.ActivityDescriptionId,
                             prodData.ResourceMaintenanceCostTypeId,AuthenticationSuccessmode);
                }
                catch (Exception ex)
                {
                    string error = string.Empty;
                    using (System.IO.StringReader reader = new System.IO.StringReader(ex.Message))
                    {
                        error = reader.ReadLine();
                    }
                    string[] errorObj = null;
                    if (!string.IsNullOrEmpty(error))
                        errorObj = error.Split(Environment.NewLine.ToCharArray());
                    AggieGlobalLogManager.Fatal("ActivityRepository :: CreateActivity :: failed :: " + ex.Message);
                    result = false;
                    return result;
                }
            }
            return result;
        }
        public bool UpdateActivity(ActivityDetail prodData)
        {
            bool result = default(bool);
            int AuthenticationSuccessmode = 0;
            using (var connection = GetConnection())
            {
                try
                {
                    connection.Open();

                    if (prodData.ProductTypeId == ProductType.Crop.GetHashCode())
                        prodData.ActivityId = UpdateRecord("UpdateActivityCrop", prodData.UserId, prodData.ProductId, prodData.CategoryId, prodData.LastHarvestedDate, prodData.IsHarvestedBefore,
                                prodData.IsSoldBefore, prodData.IsSoldBeforeNoReason, prodData.SoldPrice, prodData.ActivityDate, prodData.PlantationDate, prodData.PlotId,
                                prodData.ActivityDescriptionId, prodData.ActivityId, AuthenticationSuccessmode);
                    else if (prodData.ProductTypeId == ProductType.LiveStock.GetHashCode())
                        prodData.ActivityId = UpdateRecord("UpdateActivityLiveStock", prodData.UserId, prodData.ProductId, prodData.CategoryId,
                        prodData.NumberOfLivestock, prodData.LiveStockUsageId, prodData.LiveStockUtilityId,
                        prodData.IsLivestockSalable, prodData.LastDateOfLivestockSold, prodData.LivestocksellingLocationId, prodData.ActivityDate,
                        prodData.PlotId, prodData.ActivityDescriptionId, prodData.LivestocksellingLocationName, prodData.LiveStockName,
                        prodData.LiveStockUsageName, prodData.LiveStockUtilityName, prodData.SoldLiveStockAmount, prodData.ActivityId, AuthenticationSuccessmode);
                    else if (prodData.ProductTypeId == ProductType.Resource.GetHashCode())
                        prodData.ActivityId = UpdateRecord("UpdateActivityResources", prodData.UserId, prodData.ProductId, prodData.CategoryId, prodData.NumberOfResource, prodData.ResourceCostTypeId,
                        prodData.ResourceTypeId, prodData.ResourceTypeName, prodData.ResourceMaintenancePrice, prodData.ResourcePrice, prodData.ActivityDate, prodData.PlotId, prodData.ActivityDescriptionId,
                        prodData.ResourceMaintenanceCostTypeId, prodData.ActivityId, AuthenticationSuccessmode);
                }
                catch (Exception ex)
                {
                    string error = string.Empty;
                    using (System.IO.StringReader reader = new System.IO.StringReader(ex.Message))
                    {
                        error = reader.ReadLine();
                    }
                    string[] errorObj = null;
                    if (!string.IsNullOrEmpty(error))
                        errorObj = error.Split(Environment.NewLine.ToCharArray());
                    AggieGlobalLogManager.Fatal("ActivityRepository :: UpdateActivity :: failed :: " + ex.Message);
                    result = false;
                    return result;
                }
            }
            return result;
        }
        public bool DeleteActivity(int productId)
        {
            DbTransaction transaction = null;
            bool result = default(bool);
            int AuthenticationSuccessmode = 0;
            try
            {
                using (var connection = GetConnection())
                {
                    try
                    {
                        connection.Open();
                        return (UpdateRecord("DeleteActivity", productId, AuthenticationSuccessmode) == 1 ? true : false);
                    }
                    catch (Exception ex)
                    {
                        string error = string.Empty;
                        using (System.IO.StringReader reader = new System.IO.StringReader(ex.Message))
                        {
                            error = reader.ReadLine();
                        }
                        string[] errorObj = null;
                        if (!string.IsNullOrEmpty(error))
                            errorObj = error.Split(Environment.NewLine.ToCharArray());
                        AggieGlobalLogManager.Fatal("DeleteActivity :: failed :: " + ex.Message);
                        result = false;
                        return result;
                    }
                }
            }
            catch(Exception e)
            {

            }
            return result;
        }
        public IEnumerable<ActivityDetail> GetActivityListByMonth(int userID, string dateStamp)
        {
            IEnumerable<ActivityDetail> proddetail = null;
            int AuthenticationSuccessmode = 0;
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    if (dateStamp != null)
                        proddetail = GetRecord("GetProductListByDate", userID, dateStamp, dateStamp, AuthenticationSuccessmode);
                }
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("ActivityRepository :: GetActivityListByMonth failed :: " + ex.Message);
            }
            return proddetail;
        }
        public IEnumerable<ActivityDetail> GetActivityCountByDate(int userID,string dateStamp)
        {
            DateTime date = Convert.ToDateTime("2016-02-01 12:31:00");
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            IEnumerable<ActivityDetail> proddetail = null;
            int AuthenticationSuccessmode = 0;
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    proddetail = GetRecord("GetActivityCountByDate", userID, dateStamp, AuthenticationSuccessmode);
                }
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("ActivityRepository :: GetActivityCountByDate failed :: " + ex.Message);
            }
            return proddetail;
        }
        public IEnumerable<ActivityDetail> GetActivityById(int ActivityId)
        {
            IEnumerable<ActivityDetail> proddetail = null;
            int AuthenticationSuccessmode = 0;
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    proddetail = GetRecord("GetActivityById", ActivityId, AuthenticationSuccessmode);
                }
            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal("ActivityRepository :: GetActivityById failed :: " + ex.Message);
            }
            return proddetail;
        }
    }
}