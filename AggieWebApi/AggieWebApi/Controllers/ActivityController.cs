using AggieGlobal.Models.Client;
using AggieGlobal.Models.Common;
using AggieGlobal.WebApi.Common;
using AggieGlobal.WebApi.Controllers.Common;
using AggieGlobal.WebApi.Filters;
using AggieGlobal.WebApi.Infrastructure;
using AggieWebApi.Business.Manager;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace AggieWebApi.Controllers
{
    [CustomFilter]
    public class ActivityController : AbstractController<ActivityDetail>
    {
        public ActivityController()
        {

        }
        public ActivityDetailResponse CreateUpdateActivity(ActivityDetailResponse det)
        {
            bool res = default(bool);
            if (det.ActivityDate == null || det.ProductName == string.Empty || det.ProductId == string.Empty)
            {
                det = new ActivityDetailResponse();
                det.Error = "Failed to create or update farm request structure invalid";
                det.Status = ResponseStatus.Failed;
                return det;
            }

            try
            {
                if (HttpContext.Current.Session[ApplicationConstant.UserSession] != null)
                {
                    SessionData sessionObject = (SessionData)HttpContext.Current.Session[ApplicationConstant.UserSession];

                    ActivityDetail response = new ActivityDetail();
                    var connectionString = "AggieGlobal";
                    var repo = new ActivityManager(connectionString);


                    response.ActivityId = (det.ActivityId !=null && det.ActivityId != string.Empty ? Convert.ToInt32(EncryptionHelper.AesDecryption(det.ActivityId.ToString(), EncryptionKey.LOG)) : default(int));


                    response.ProductTypeId = det.ProductTypeId;

                    response.UserId = sessionObject._userId;
                    response.CategoryId = Convert.ToInt32(EncryptionHelper.AesDecryption(det.CategoryId.ToString(), EncryptionKey.LOG));
                    response.ProductId = Convert.ToInt32(EncryptionHelper.AesDecryption(det.ProductId.ToString(), EncryptionKey.LOG));
                    response.LastHarvestedDate = (det.LastHarvestedDate.ToString().IndexOf("01-01-0001") > -1 ? DateTime.Now : det.LastHarvestedDate);
                    response.ProductTypeId = det.ProductTypeId;
                    response.IsHarvestedBefore = det.IsHarvestedBefore;//(det.IsHarvestedBefore==true?1 : 0);
                    response.IsSoldBefore = det.IsSoldBefore;// == true ? 1 : 0;
                    response.IsSoldBeforeNoReason = det.IsSoldBeforeNoReason;
                    response.SoldPrice = det.SoldPrice;
                    response.ActivityDate = (det.ActivityDate.ToString().IndexOf("01-01-0001") > -1 ? DateTime.Now : det.ActivityDate);
                    response.PlantationDate = (det.PlantationDate.ToString().IndexOf("01-01-0001") > -1 ? DateTime.Now : det.PlantationDate);



                    response.NumberOfLivestock = det.NumberOfLivestock;
                    response.LiveStockUsageId = det.LiveStockUsageId;
                    response.LiveStockUtilityId = det.LiveStockUtilityId;
                    response.IsLivestockSalable = det.IsLivestockSalable;
                    response.LastDateOfLivestockSold = (det.LastDateOfLivestockSold.ToString().IndexOf("01-01-0001") > -1 ? DateTime.Now : det.LastDateOfLivestockSold);
                    response.LivestocksellingLocationId = det.LivestocksellingLocationId;
                    response.LivestocksellingLocationName = det.LivestocksellingLocationName;
                    response.LiveStockName = det.LiveStockName;
                    response.LiveStockUsageName = det.LiveStockUsageName;
                    response.LiveStockUtilityName = det.LiveStockUtilityName;
                    response.SoldLiveStockAmount = det.SoldLiveStockAmount;
                    response.ActivityDate = (det.ActivityDate.ToString().IndexOf("01-01-0001") > -1 ? DateTime.Now : det.ActivityDate);

                    response.NumberOfResource = det.NumberOfResource;
                    response.ResourceTypeId = det.ResourceTypeId;
                    response.ResourceCostTypeId = det.ResourceCostTypeId;
                    response.ResourcePrice = det.ResourcePrice;
                    response.ResourceMaintenanceCostTypeId = det.ResourceMaintenanceCostTypeId;
                    response.ResourceMaintenancePrice = det.ResourceMaintenancePrice;


                    response.PlotId = Convert.ToInt32(EncryptionHelper.AesDecryption(det.PlotId.ToString(), EncryptionKey.LOG));
                    response.ActivityDescriptionId = det.ActivityDescriptionId;
                    response.TotalNumberOfResource = det.TotalNumberOfResource;

                    if(det.ProductResourceList!=null && det.ProductResourceList.Count()>default(int))
                    {
                        foreach (ProductResourcesResponse resource in det.ProductResourceList)
                        {
                            switch(resource.ProductRessourceType)
                            {
                                case ProductRessourceType.LivestocksellingLocation:
                                    {
                                        response.LivestocksellingLocationId = (resource.ProductResourceId.ToString()!=string.Empty? Convert.ToInt32(EncryptionHelper.AesDecryption(resource.ProductResourceId.ToString(), EncryptionKey.LOG)):default(int));
                                        response.LivestocksellingLocationName = resource.ProductResourceName;
                                        break;
                                    }
                                case ProductRessourceType.LiveStockUsage:
                                    {
                                        response.LiveStockUsageId = (resource.ProductResourceId.ToString() != string.Empty ? Convert.ToInt32(EncryptionHelper.AesDecryption(resource.ProductResourceId.ToString(), EncryptionKey.LOG)) : default(int));
                                        response.LiveStockUsageName = resource.ProductResourceName;
                                        break;
                                    }
                                case ProductRessourceType.LivestockUtility:
                                    {
                                        response.LiveStockUtilityId = (resource.ProductResourceId.ToString() != string.Empty ? Convert.ToInt32(EncryptionHelper.AesDecryption(resource.ProductResourceId.ToString(), EncryptionKey.LOG)) : default(int));
                                        response.LiveStockUtilityName = resource.ProductResourceName;
                                        break;
                                    }
                                case ProductRessourceType.ResourceType:
                                    {
                                        response.ResourceTypeId = (resource.ProductResourceId.ToString() != string.Empty ? Convert.ToInt32(EncryptionHelper.AesDecryption(resource.ProductResourceId.ToString(), EncryptionKey.LOG)) : default(int));
                                        response.ResourceTypeName = det.ResourceTypeName;
                                        break;
                                    }
                                case ProductRessourceType.ResourceCostType:
                                    {
                                        response.ResourceCostTypeId = (resource.ProductResourceId.ToString() != string.Empty ? Convert.ToInt32(EncryptionHelper.AesDecryption(resource.ProductResourceId.ToString(), EncryptionKey.LOG)):default(int));
                                        response.ResourcePrice = det.ResourcePrice;
                                        break;
                                    }
                                case ProductRessourceType.ResourceMaintenaceCostType:
                                    {
                                        response.ResourceMaintenanceCostTypeId = (resource.ProductResourceId.ToString() != string.Empty ? Convert.ToInt32(EncryptionHelper.AesDecryption(resource.ProductResourceId.ToString(), EncryptionKey.LOG)) : default(int));
                                        response.ResourceMaintenancePrice = det.ResourceMaintenancePrice;
                                        break;
                                    }
                            }
                        }
                    }
                    


                    res = repo.CreateUpdateActivity(response);

                    if (response.ActivityId > default(int))
                    {
                        det = new ActivityDetailResponse();
                        det.ActivityId = Convert.ToString(EncryptionHelper.AesEncryption(response.ActivityId.ToString(), EncryptionKey.LOG));
                        det.Status = ResponseStatus.Successful;
                    }
                    else
                    {
                        det = new ActivityDetailResponse();
                        det.Error = "Failed to insert data";
                        det.Status = ResponseStatus.Successful;
                    }
                    repo.Dispose();
                }
            }
            catch (Exception ex)
            {
                det = new ActivityDetailResponse();
                det.Error = "Failed to create or update farm";
                det.Status = ResponseStatus.Failed;
                AggieGlobalLogManager.Fatal("ActivityController :: CreateUpdateActivity failed :: " + ex.Message);
            }

            return det;
        }
        [HttpGet]
        [ActionName("DeleteActivity")]
        public ActivityDetailResponse DeleteActivity(string ActivityId)
        {
            ActivityId = ActivityId.Replace("+", "%20");
            ActivityId = System.Net.WebUtility.UrlDecode(ActivityId);
            ActivityId = ActivityId.Replace(" ", "+");
            ActivityDetailResponse requestData = new ActivityDetailResponse();
            int _activityId = default(int);
            if (ActivityId == string.Empty)
            {
                requestData.Error = "Failed to create or update farm request structure invalid";
                requestData.Status = ResponseStatus.Failed;
                return requestData;
            }
            else
            {
                _activityId = Convert.ToInt32(EncryptionHelper.AesDecryption(ActivityId, EncryptionKey.LOG));
            }
            try
            {
                if (HttpContext.Current.Session[ApplicationConstant.UserSession] != null)
                {
                    SessionData sessionObject = (SessionData)HttpContext.Current.Session[ApplicationConstant.UserSession];

                    ActivityDetail resdata = new ActivityDetail();
                    var connectionString = "AggieGlobal";
                    var repo = new ActivityManager(connectionString);
                    bool res = repo.DeleteActivity(_activityId);
                    if (res == true)
                    {
                        requestData.Status = ResponseStatus.Successful;

                    }
                    else
                    {
                        requestData.Error = "Failed to create or update farm request structure invalid";
                        requestData.Status = ResponseStatus.Failed;
                        return requestData;
                    }
                    repo.Dispose();
                }
            }
            catch (Exception ex)
            {
                requestData.Error = "Failed to create or update farm";
                requestData.Status = ResponseStatus.Failed;
                AggieGlobalLogManager.Fatal("ActivityController :: CreateUpdatePlot failed :: " + ex.Message);
            }

            return requestData;
        }

        public IList<ActivityDetailCountResponse> GetActivityCountByDate(string dateStamp)
        {
            IList<ActivityDetailCountResponse> requestData = new List<ActivityDetailCountResponse>();
            ActivityDetailCountResponse response = new ActivityDetailCountResponse();
            if (string.IsNullOrEmpty(dateStamp))
            {
                response.Error = "Failed to get activity list data imvalid parameter";
                response.Status = ResponseStatus.Failed;
                requestData.Add(response);
                return requestData;
            }
            try
            {
                if (HttpContext.Current.Session[ApplicationConstant.UserSession] != null)
                {
                    SessionData sessionObject = (SessionData)HttpContext.Current.Session[ApplicationConstant.UserSession];

                    ActivityDetail resdata = new ActivityDetail();
                    var connectionString = "AggieGlobal";
                    var repo = new ActivityManager(connectionString);
                    IEnumerable<ActivityDetail> res = repo.GetActivityCountByDate(sessionObject._userId, dateStamp);
                    if (res != null && res.Count() > default(int))
                    {
                        foreach (ActivityDetail det in res)
                        {
                            response = new ActivityDetailCountResponse();
                            response.ActivityDate = det.ActivityDate;
                            response.ActivityCount = det.ActivityCount;
                            response.Status = ResponseStatus.Successful;
                            requestData.Add(response);
                        }
                    }
                    else
                    {
                        response.Error = "Failed to get activity lis";
                        response.Status = ResponseStatus.Failed;
                        requestData.Add(response);
                        return requestData;
                    }
                    repo.Dispose();
                }
            }
            catch (Exception ex)
            {
                response.Error = "Failed to get activity list data";
                response.Status = ResponseStatus.Failed;
                requestData.Add(response);
                AggieGlobalLogManager.Fatal("ActivityController :: GetActivityCountByDate failed :: " + ex.Message);
            }

            return requestData;
        }
        public IList<ActivityDetailResponse> GetActivityListByMonth(string dateStamp)
        {
            IList<ActivityDetailResponse> requestData = new List<ActivityDetailResponse>();
            ActivityDetailResponse response = new ActivityDetailResponse();
            if (string.IsNullOrEmpty(dateStamp))
            {
                response.Error = "Failed to get activity list data imvalid parameter";
                response.Status = ResponseStatus.Failed;
                requestData.Add(response);
                return requestData;
            }
            try
            {
                if (HttpContext.Current.Session[ApplicationConstant.UserSession] != null)
                {
                    SessionData sessionObject = (SessionData)HttpContext.Current.Session[ApplicationConstant.UserSession];

                    ActivityDetail resdata = new ActivityDetail();
                    var connectionString = "AggieGlobal";
                    var repo = new ActivityManager(connectionString);
                    IEnumerable<ActivityDetail> res = repo.GetActivityListByMonth(sessionObject._userId, dateStamp);
                    if (res != null && res.Count() > default(int))
                    {
                        foreach (ActivityDetail det in res)
                        {
                            response = new ActivityDetailResponse();
                            response.ActivityId = EncryptionHelper.AesEncryption(det.ActivityId.ToString(), EncryptionKey.LOG).ToString();
                            response.ActivityDate = det.ActivityDate;
                            response.CategoryName = det.CategoryName.Trim();
                            response.CategoryId = EncryptionHelper.AesEncryption(det.CategoryId.ToString(), EncryptionKey.LOG);
                            response.IsHarvestedBefore = det.IsHarvestedBefore;//==1?true:false;
                            response.IsSoldBefore = det.IsSoldBefore;// == 1 ? true : false; 
                            response.IsSoldBeforeNoReason = det.IsSoldBeforeNoReason;
                            response.LastHarvestedDate = det.LastHarvestedDate;
                            response.PlantationDate = det.PlantationDate;
                            response.ProductId = EncryptionHelper.AesEncryption(det.ProductId.ToString(), EncryptionKey.LOG);
                            response.TotalNumberOfResource = det.TotalNumberOfResource;
                            response.ActivityDescription = (!string.IsNullOrEmpty(det.ActivityDescription) ? det.ActivityDescription.Trim() : string.Empty);
                            response.ProductName = det.ProductName;
                            response.catImageName = det.catImageName;
                            response.prodImageName = det.prodImageName;
                            response.Status = ResponseStatus.Successful;
                            requestData.Add(response);
                        }
                    }
                    else
                    {
                        response.Error = "Failed to get activity lis";
                        response.Status = ResponseStatus.Failed;
                        requestData.Add(response);
                        return requestData;
                    }
                    repo.Dispose();
                }
            }
            catch (Exception ex)
            {
                response.Error = "Failed to get activity list data";
                response.Status = ResponseStatus.Failed;
                requestData.Add(response);
                AggieGlobalLogManager.Fatal("ActivityController :: GetActivityListByMonth failed :: " + ex.Message);
            }

            return requestData;
        }
        public IList<ActivityDetailResponse> GetActivityDetail(string ActivityId)
        {
            ActivityId = ActivityId.Replace("+", "%20");
            ActivityId = System.Net.WebUtility.UrlDecode(ActivityId);
            ActivityId = ActivityId.Replace(" ", "+");

            IList<ActivityDetailResponse> requestData = new List<ActivityDetailResponse>();
            ActivityDetailResponse response = new ActivityDetailResponse();
            if (string.IsNullOrEmpty(ActivityId))
            {
                response.Error = "Failed to get activity list data imvalid parameter";
                response.Status = ResponseStatus.Failed;
                requestData.Add(response);
                return requestData;
            }
            try
            {
                if (HttpContext.Current.Session[ApplicationConstant.UserSession] != null)
                {
                    SessionData sessionObject = (SessionData)HttpContext.Current.Session[ApplicationConstant.UserSession];

                    ActivityDetail resdata = new ActivityDetail();
                    var connectionString = "AggieGlobal";
                    var repo = new ActivityManager(connectionString);
                    int _activityId= Convert.ToInt32(EncryptionHelper.AesDecryption(ActivityId, EncryptionKey.LOG));
                    IEnumerable<ActivityDetail> res = repo.GetActivityById(_activityId);
                    if (res != null && res.Count() > default(int))
                    {
                        foreach (ActivityDetail det in res)
                        {
                            response = new ActivityDetailResponse();
                            response.ActivityId = EncryptionHelper.AesEncryption(det.ActivityId.ToString(), EncryptionKey.LOG);
                            response.ProductId = EncryptionHelper.AesEncryption(det.ProductId.ToString(), EncryptionKey.LOG);
                            response.ProductName = det.ProductName;
                            response.PlotName = det.PlotName;
                            response.CategoryId = EncryptionHelper.AesEncryption(det.CategoryId.ToString(), EncryptionKey.LOG);
                            response.CategoryName = det.CategoryName;
                            response.LastHarvestedDate = det.LastHarvestedDate;
                            response.IsHarvestedBefore = det.IsHarvestedBefore;
                            response.IsSoldBefore = det.IsSoldBefore;
                            response.IsSoldBeforeNoReason = det.IsSoldBeforeNoReason;
                            response.SoldPrice = det.SoldPrice;
                            response.ActivityDate = det.ActivityDate;
                            response.PlantationDate = det.PlantationDate;
                            response.DeletionFlag = det.DeletionFlag;
                            response.ResourceName = det.ResourceName;
                            response.TotalNumberOfResource = det.TotalNumberOfResource;
                            response.ResourceCostType = det.ResourceCostType;
                            response.ResourcePrice = det.ResourcePrice;
                            response.ProductTypeId = det.ProductTypeId;
                            response.ActivityCount = det.ActivityCount;
                            response.PlotId = EncryptionHelper.AesEncryption(det.PlotId.ToString(), EncryptionKey.LOG);
                            response.NumberOfLivestock = det.NumberOfLivestock;
                            response.LiveStockUsageId = det.LiveStockUsageId;
                            response.LiveStockUtilityId = det.LiveStockUtilityId;
                            response.IsLivestockSalable = det.IsLivestockSalable;
                            response.LastDateOfLivestockSold = det.LastDateOfLivestockSold;
                            response.SoldLiveStockAmount = det.SoldLiveStockAmount;
                            response.LivestocksellingLocationId = det.LivestocksellingLocationId;
                            response.ActivationId = det.ActivationId;
                            response.LivestocksellingLocationName = det.LivestocksellingLocationName;
                            response.LiveStockName = det.LiveStockName;
                            response.LiveStockUsageName = det.LiveStockUsageName;
                            response.LiveStockUtilityName = det.LiveStockUtilityName;
                            response.NumberOfResource = det.NumberOfResource;
                            response.ResourceTypeId = det.ResourceTypeId;
                            response.ResourceTypeName = det.ResourceTypeName;
                            response.ResourceCostTypeId = det.ResourceCostTypeId;
                            response.SoldLiveStockProductId = det.SoldLiveStockProductId;
                            response.ActivityDescriptionId = det.ActivityDescriptionId;
                            response.ResourceMaintenanceCostTypeId = det.ResourceMaintenanceCostTypeId;
                            response.ResourceMaintenancePrice = det.ResourceMaintenancePrice;
                            response.ActivityDescription = (!string.IsNullOrEmpty(det.ActivityDescription) ? det.ActivityDescription.Trim() : string.Empty);
                            response.ProductTypeId = det.CategoryId;
                            response.ResourceMaintenaceCostTypeName = det.ResourceMaintenaceCostTypeName;
                            response.ResourceTypeName = det.ResourceTypeName;
                            response.ResourceCostTypeName = det.ResourceCostTypeName;
                            response.ResourceMaintenaceCostTypeName = det.ResourceMaintenaceCostTypeName;
                            response.catImageName = det.catImageName;
                            response.prodImageName = det.prodImageName;
                            response.Status = ResponseStatus.Successful;
                            requestData.Add(response);
                        }
                    }
                    else
                    {
                        response.Error = "Failed to get activity lis";
                        response.Status = ResponseStatus.Failed;
                        requestData.Add(response);
                        return requestData;
                    }
                    repo.Dispose();
                }
            }
            catch (Exception ex)
            {
                response.Error = "Failed to get activity list data";
                response.Status = ResponseStatus.Failed;
                requestData.Add(response);
                AggieGlobalLogManager.Fatal("ActivityController :: GetActivityDetail failed :: " + ex.Message);
            }

            return requestData;
        }
    }

}
