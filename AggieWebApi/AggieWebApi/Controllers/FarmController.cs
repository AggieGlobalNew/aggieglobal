using AggieGlobal.Models.Client;
using AggieGlobal.Models.Common;
using AggieGlobal.WebApi.Common;
using AggieGlobal.WebApi.Controllers.Common;
using AggieGlobal.WebApi.Filters;
using AggieGlobal.WebApi.Infrastructure;
using AggieWebApi.Business.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace AggieWebApi.Controllers
{
    [CustomFilter]
    public class FarmController : AbstractController<FarmDetail>
    {
        public FarmController()
        {
        }

        [ActionName("FarmDetails")]
        public bool FarmDetails()
        {
            bool ret = default(bool);

            return ret;
        }

        [HttpPost]
        [ActionName("CreateUpdateFarm")]
        public FarmDetailResponse CreateUpdateFarm(FarmDetailResponse requestData)
        {
            int res = default(int);
            FarmDetailResponse responsedata = new FarmDetailResponse();
            try
            {
                if (HttpContext.Current.Session[ApplicationConstant.UserSession] != null)
                {
                    SessionData sessionObject = (SessionData)HttpContext.Current.Session[ApplicationConstant.UserSession];
                    AggieGlobalLogManager.Info("FarmDetailsController :: GetFarmsDetails started ");
                    var connectionString = "AggieGlobal";
                    var repo = new FarmManager(connectionString);
                    FarmDetail response = new FarmDetail();
                    response.FarmId = string.IsNullOrEmpty(requestData.FarmId) == true ? default(int) : Convert.ToInt32(EncryptionHelper.AesDecryption(requestData.FarmId, EncryptionKey.LOG)); ;
                    response.FarmName = responsedata.FarmName;
                    response.FarmSize = requestData.FarmSize;
                    response.FarmSizeUnit = requestData.FarmSizeUnit;
                    response.FarmAddress = requestData.FarmAddress;
                    response.FarmEstablishedDate = requestData.FarmEstablishedDate;

                    res = repo.CreateUpdateFarm(response);

                    if (res > default(int))
                    {
                        requestData.FarmId = (res == default(int) ? string.Empty : Convert.ToString(EncryptionHelper.AesEncryption(res.ToString(), EncryptionKey.LOG)));
                        responsedata.Status = ResponseStatus.Successful;
                    }
                    else
                    {
                        responsedata.Status = ResponseStatus.Failed;
                        responsedata.Error = "Failed to create or update farm";
                    }
                    repo.Dispose();
                }
            }
            catch (Exception ex)
            {
                responsedata.Error = "Failed to create or update farm";
                responsedata.Status = ResponseStatus.Failed;
                AggieGlobalLogManager.Fatal("FarmDetailsController :: CreateUpdateFarm failed :: " + ex.Message);
            }

            return requestData;
        }

        [ActionName("GetFarmsDetails")]
        public FarmDetailResponse GetFarmsDetails()
        {
            bool ret = default(bool);
            IEnumerable<FarmDetail> farmData = null;
            FarmDetailResponse responsedata = new FarmDetailResponse();
            try
            {
                if (HttpContext.Current.Session[ApplicationConstant.UserSession] != null)
                {
                    SessionData sessionObject = (SessionData)HttpContext.Current.Session[ApplicationConstant.UserSession];
                    AggieGlobalLogManager.Info("FarmDetailsController :: GetFarmsDetails started ");
                    var connectionString = "AggieGlobal";
                    var repo = new FarmManager(connectionString);
                    farmData= repo.GetFarmDetails(sessionObject._userId);
                    if (farmData != null && farmData.Count() > default(int))
                    {
                        responsedata.FarmId = EncryptionHelper.AesEncryption(Convert.ToString(farmData.FirstOrDefault().FarmId),EncryptionKey.LOG);
                        responsedata.FarmName = farmData.FirstOrDefault().FarmName;
                        responsedata.CoOpName = farmData.FirstOrDefault().CoOpName;
                        responsedata.FarmAddress = farmData.FirstOrDefault().FarmAddress;
                        responsedata.FarmEstablishedDate = farmData.FirstOrDefault().FarmEstablishedDate;
                        responsedata.FarmSize = farmData.FirstOrDefault().FarmSize;
                        responsedata.FarmSizeUnit = farmData.FirstOrDefault().FarmSizeUnit;
                        responsedata.Status = ResponseStatus.Successful;
                    }
                    else
                    {
                        responsedata.Error = "Failed to retreive data";
                        responsedata.Status = ResponseStatus.Failed;
                    }
                    repo.Dispose();
                }
            }
            catch (Exception ex)
            {
                responsedata.Status = ResponseStatus.Failed;
                AggieGlobalLogManager.Fatal("FarmDetailsController :: GetFarmsDetails failed :: " + ex.Message);
            }
            return responsedata;
        }

        [ActionName("GetAllFarmsDetails")]
        public IList<FarmDetailResponse> GetAllFarmsDetails()
        {
            bool ret = default(bool);
            IEnumerable<FarmDetail> farmData = null;
            IList <FarmDetailResponse> responsedata = new List<FarmDetailResponse>();
            try
            {
                    AggieGlobalLogManager.Info("FarmDetailsController :: GetFarmsDetails started ");
                    var connectionString = "AggieGlobal";
                    var repo = new FarmManager(connectionString);
                    farmData = repo.GetFarmDetails(default(int));
                    if (farmData != null && farmData.Count() > default(int))
                    {
                        foreach (FarmDetail det in farmData)
                        {
                            FarmDetailResponse resdata = new FarmDetailResponse();
                            resdata.FarmId = EncryptionHelper.AesEncryption(Convert.ToString(det.FarmId), EncryptionKey.LOG);
                            resdata.FarmName = det.FarmName;
                            resdata.CoOpName = det.CoOpName;
                            resdata.FarmAddress = det.FarmAddress;
                            resdata.FarmEstablishedDate = det.FarmEstablishedDate;
                            resdata.FarmSize = det.FarmSize;
                            resdata.FarmSizeUnit = det.FarmSizeUnit;
                            resdata.Status = ResponseStatus.Successful;
                            responsedata.Add(resdata);
                        }
                    }
                    else
                    {
                        FarmDetailResponse resdata = new FarmDetailResponse();
                        resdata.Error = "Failed to retreive data";
                        resdata.Status = ResponseStatus.Failed;
                        responsedata.Add(resdata);
                    }
                    repo.Dispose();
            }
            catch (Exception ex)
            {


                FarmDetailResponse resdata = new FarmDetailResponse();
                resdata.Error = "Failed to retreive data";
                resdata.Status = ResponseStatus.Failed;
                responsedata.Add(resdata);

                AggieGlobalLogManager.Fatal("FarmDetailsController :: GetFarmsDetails failed :: " + ex.Message);
            }
            return responsedata;
        }

        [ActionName("MapFarmByUserDetail")]
        public FarmDetailResponse MapFarmByUserDetail(string FarmId)
        {
            bool res = default(bool);
            FarmDetailResponse resposne = null;
            SessionData sessionObject = null;
            int farmId = default(int);
            int userId = default(int);
            try
            {
                if (HttpContext.Current.Session[ApplicationConstant.UserSession] != null)
                {
                    sessionObject = (SessionData)HttpContext.Current.Session[ApplicationConstant.UserSession];
                    resposne = new FarmDetailResponse();
                    if (string.IsNullOrEmpty(FarmId))
                    {
                        resposne.Error = "Failed to process";
                        resposne.Status = ResponseStatus.Failed;
                        return resposne;
                    }
                    else
                    {
                        farmId = Convert.ToInt32(EncryptionHelper.AesDecryption(FarmId, EncryptionKey.LOG));
                        userId = sessionObject._userId;
                    }


                    AggieGlobalLogManager.Info("FarmDetailsController :: MapFarmByUserDetail started ");
                    var connectionString = "AggieGlobal";

                    var repo = new FarmManager(connectionString);

                    res = repo.MapFarmByUserDetail(farmId, userId);
                    if (res == true)
                    {
                        resposne.Status = ResponseStatus.Successful;
                    }
                    else
                    {
                        resposne.Error = "Failed to process data";
                        resposne.Status = ResponseStatus.Failed;
                    }
                    AggieGlobalLogManager.Info("FarmDetailsController :: MapFarmByUserDetail ended ");
                    repo.Dispose();
                }
            }
            catch (Exception ex)
            {
                resposne.Error = "Failed to process";
                resposne.Status = ResponseStatus.Failed;
                AggieGlobalLogManager.Fatal("FarmManager :: MapFarmByUserDetail failed :: " + ex.Message);
            }
            return resposne;
        }

    }
}
