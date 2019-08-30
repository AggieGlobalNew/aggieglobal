
/* ========================================================================
 * Includes    : FormatterConfig.cs
 * Website     : http://www.atlassoft.com
 * Create Date : May, May 08, 2019 by Sudipta Sarkar
 * Details     : Implements various data type formatter registration logics for AggieGlobal.WebApi.
 * Copyright © 2019 Atlas Software Technologies All rights reserved
 * ========================================================================
 * Release History
 * ---------------
 * DESCRIPTION					CREATED / MODIFIED BY
 * -----------                  ---------------------
 * Initial Implementation		Sudipta Sarkar
 * ========================================================================
 */

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
using AggieGlobal.WebApi.Infrastructure;

namespace AggieWebApi.Controllers
{
    [CustomFilter]
    public class PlotController : AbstractController<PlotDetail>
    {
        public PlotController()
        {
        }

        [HttpPost]
        [ActionName("CreateUpdatePlot")]
        public PlotDetailResponse CreateUpdatePlot([FromBody]PlotDetailResponse det)
        {
            string str = Newtonsoft.Json.JsonConvert.SerializeObject(det);

            bool res = default(bool);
            if(det.PlotName==string.Empty && det.PlotSize<=default(int) && det.FarmId==string.Empty)
            {
                det.Error = "Failed to create or update farm request structure invalid";
                det.Status = ResponseStatus.Failed;
                return det;
            }
          
            try
            {
                if (HttpContext.Current.Session[ApplicationConstant.UserSession] != null)
                {
                    SessionData sessionObject = (SessionData)HttpContext.Current.Session[ApplicationConstant.UserSession];

                    PlotDetail resdata = new PlotDetail();
                    resdata.PlotName = det.PlotName;
                    resdata.PlotSize = det.PlotSize;
                    resdata.Organic = det.Organic;
                    resdata.SoilId = string.IsNullOrEmpty(det.SoilId) == true?default(int):Convert.ToInt32(EncryptionHelper.AesDecryption(det.SoilId,EncryptionKey.LOG));
                    resdata.SoilPhId = string.IsNullOrEmpty(det.SoilPhId) == true ? default(int) : Convert.ToInt32(EncryptionHelper.AesDecryption(det.SoilPhId, EncryptionKey.LOG));
                    resdata.Notes = det.Notes;
                    resdata.FarmId = Convert.ToInt32(EncryptionHelper.AesDecryption(det.FarmId, EncryptionKey.LOG));
                    resdata.PlotId = !string.IsNullOrEmpty(det.PlotId) ? Convert.ToInt32(EncryptionHelper.AesDecryption(det.PlotId, EncryptionKey.LOG)) : 0;
                    resdata.UserID = sessionObject._userId;

                    AggieGlobalLogManager.Info("PlotController :: CreateUpdatePlot started ");
                    var connectionString = "AggieGlobal";
                    var repo = new PlotManager(connectionString);
                    res = repo.CreateUpdatePlot(resdata);
                    det.Status = ResponseStatus.Successful;
                    det.PlotId= EncryptionHelper.AesEncryption(resdata.PlotId.ToString(), EncryptionKey.LOG);
                    repo.Dispose();
                }
            }
            catch (Exception ex)
            {
                det.Error = "Failed to create or update farm";
                det.Status = ResponseStatus.Failed;
                AggieGlobalLogManager.Fatal("PlotController :: CreateUpdatePlot failed :: " + ex.Message);
            }

            return det;
        }

        [HttpGet]
        [ActionName("GetPlotListDetails")]
        public IEnumerable<PlotDetailResponse> PlotListDetails(string farmid)
        {
            farmid = farmid.Replace("+", "%20");
            farmid = System.Net.WebUtility.UrlDecode(farmid);
            farmid = farmid.Replace(" ", "+");
            bool res = default(bool);
            IList<PlotDetailResponse> responsedata = new List<PlotDetailResponse>();
            IEnumerable<PlotDetail> internalPlots = null;
            try
            {
                if (HttpContext.Current.Session[ApplicationConstant.UserSession] != null)
                {
                    SessionData sessionObject = (SessionData)HttpContext.Current.Session[ApplicationConstant.UserSession];
                    AggieGlobalLogManager.Info("PlotController :: CreateUpdatePlot started ");
                    var connectionString = "AggieGlobal";
                    var repo = new PlotManager(connectionString);
                    internalPlots = repo.PlotListDetails(farmid, sessionObject._userId);
                    if (internalPlots != null && internalPlots.Count() > default(int))
                    {
                        foreach (PlotDetail det in internalPlots)
                        {
                            PlotDetailResponse resdata = new PlotDetailResponse();
                            resdata.PlotId = EncryptionHelper.AesEncryption(Convert.ToString(det.PlotId), EncryptionKey.LOG);
                            resdata.FarmId = EncryptionHelper.AesEncryption(Convert.ToString(det.FarmId), EncryptionKey.LOG);
                            resdata.PlotName = det.PlotName;
                            resdata.PlotSize = det.PlotSize;
                            resdata.Organic = det.Organic;
                            resdata.SoilId = Convert.ToString(EncryptionHelper.AesEncryption(det.SoilId.ToString(), EncryptionKey.LOG));
                            resdata.SoilPhId = Convert.ToString(EncryptionHelper.AesEncryption(det.SoilPhId.ToString(), EncryptionKey.LOG));
                            resdata.Notes = det.Notes;
                            resdata.Status = ResponseStatus.Successful;
                            responsedata.Add(resdata);
                        }
                    }
                    else
                    {
                        PlotDetailResponse resdata = new PlotDetailResponse();
                        resdata.Status = ResponseStatus.Failed;
                        resdata.Error = "Unable to retireve plot details";
                        responsedata.Add(resdata);

                    }
                    repo.Dispose();
                }
            }
            catch (Exception ex)
            {
                PlotDetailResponse resdata = new PlotDetailResponse();
                resdata.Status = ResponseStatus.Failed;
                resdata.Error = "Unable to retireve plot details";
                responsedata.Add(resdata);
                AggieGlobalLogManager.Fatal("PlotController :: CreateUpdatePlot failed :: " + ex.Message);
            }

            return responsedata;
        }
        [ActionName("GetPlotDetailsById")]
        public IEnumerable<PlotDetailResponse> GetPlotDetailsById(string plotid)
        {

            plotid = plotid.Replace("+", "%20");
            plotid = System.Net.WebUtility.UrlDecode(plotid);
            plotid = plotid.Replace(" ", "+");

            bool res = default(bool);
            IList<PlotDetailResponse> responsedata = new List<PlotDetailResponse>();
            IEnumerable<PlotDetail> internalPlots = null;
            try
            {
                if (HttpContext.Current.Session[ApplicationConstant.UserSession] != null)
                {
                    SessionData sessionObject = (SessionData)HttpContext.Current.Session[ApplicationConstant.UserSession];
                    AggieGlobalLogManager.Info("PlotController :: CreateUpdatePlot started ");
                    var connectionString = "AggieGlobal";
                    var repo = new PlotManager(connectionString);
                    internalPlots = repo.GetPlotDetailsById(plotid);
                    if (internalPlots != null && internalPlots.Count() > default(int))
                    {
                        foreach (PlotDetail det in internalPlots)
                        {
                            PlotDetailResponse resdata = new PlotDetailResponse();
                            resdata.PlotId = EncryptionHelper.AesEncryption(Convert.ToString(det.PlotId), EncryptionKey.LOG);
                            resdata.FarmId = EncryptionHelper.AesEncryption(Convert.ToString(det.FarmId), EncryptionKey.LOG);
                            resdata.PlotName = det.PlotName;
                            resdata.PlotSize = det.PlotSize;
                            resdata.Organic = det.Organic;
                            resdata.SoilId = Convert.ToString(EncryptionHelper.AesEncryption(det.SoilId.ToString(), EncryptionKey.LOG));
                            resdata.SoilPhId = Convert.ToString(EncryptionHelper.AesEncryption(det.SoilPhId.ToString(), EncryptionKey.LOG));
                            resdata.Notes = det.Notes;
                            resdata.Status = ResponseStatus.Successful;
                            responsedata.Add(resdata);
                        }
                    }
                    else
                    {
                        PlotDetailResponse resdata = new PlotDetailResponse();
                        resdata.Status = ResponseStatus.Failed;
                        resdata.Error = "Unable to retireve plot detail";
                        responsedata.Add(resdata);

                    }
                    repo.Dispose();
                }
            }
            catch (Exception ex)
            {
                PlotDetailResponse resdata = new PlotDetailResponse();
                resdata.Status = ResponseStatus.Failed;
                resdata.Error = "Unable to retireve plot details";
                responsedata.Add(resdata);
                AggieGlobalLogManager.Fatal("PlotController :: CreateUpdatePlot failed :: " + ex.Message);
            }

            return responsedata;
        }
        [ActionName("GetSoilDetails")]
        public IList<SoilDataResponse> GetSoilDetails()
        {
            IList<SoilDataResponse> dataresponse = new List<SoilDataResponse>();
            try
            {
                if (HttpContext.Current.Session[ApplicationConstant.UserSession] != null)
                {
                    SessionData sessionObject = (SessionData)HttpContext.Current.Session[ApplicationConstant.UserSession];
                                       
                    AggieGlobalLogManager.Info("PlotController :: GetSoilDetails started ");
                    var connectionString = "AggieGlobal";
                    var repo = new PlotManager(connectionString);
                    dataresponse = repo.GetSoilDetails();
                    repo.Dispose();
                }

            }
            catch (Exception e)
            {
                SoilDataResponse det = new SoilDataResponse();
                det.Status = ResponseStatus.Failed;
                det.Error = "Failed to retreive data";
                dataresponse.Add(det);
                AggieGlobalLogManager.Fatal("AccountRepository :: LoginCheck failed :: " + e.Message);
            }
            return dataresponse;
        }

    }
}
