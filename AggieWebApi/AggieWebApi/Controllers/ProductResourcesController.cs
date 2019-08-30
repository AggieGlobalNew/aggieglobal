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


using AggieGlobal.Business.Manager;
using AggieGlobal.Models.Client;
using AggieGlobal.Models.Common;
using AggieGlobal.WebApi.Business;
using AggieGlobal.WebApi.Common;
using AggieGlobal.WebApi.Controllers.Common;
using AggieGlobal.WebApi.DataAccess.Common;
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
    public class ProductResourcesController : AbstractController<ProductResourcesResponse>
    {
        public ProductResourcesController()
            : this(null)
        {

        }

        public ProductResourcesController(IProductRessourcesRepository aRepository)
            : base()
        { }
        [ActionName("GetProductResourcesList")]
        public IList<ProductResourcesResponse> GetProductResourcesList()
        {
            IList<ProductResourcesResponse> result = new List<ProductResourcesResponse>();
            try
            {
                IEnumerable<ProductResources> listData = null;
                var connectionString = "AggieGlobal";
                var repo = new ProductResourcesManager(connectionString);
                listData = repo.GetProductResourcesList();
                if (listData != null && listData.Count() > default(int))
                {
                    foreach (ProductResources resData in listData)
                    {
                        ProductResourcesResponse resVal = new ProductResourcesResponse();
                        resVal.ProductResourceId = Convert.ToString(EncryptionHelper.AesEncryption(resData.ProductResourceId.ToString(), EncryptionKey.LOG)); 
                        resVal.ProductResourceName = resData.ProductResourceName;
                        resVal.ProductRessourceType = resData.ProductRessourceType;
                        resVal.Status = ResponseStatus.Successful;
                        result.Add(resVal);
                    }
                }
                else
                {
                    ProductResourcesResponse resVal = new ProductResourcesResponse();
                    resVal.Status = ResponseStatus.Failed;
                    resVal.Error = "No Data found";
                    result.Add(resVal);
                }
                repo.Dispose();
            }
            catch (Exception ex)
            {
                ProductResourcesResponse resVal = new ProductResourcesResponse();
                resVal.Status = ResponseStatus.Failed;
                resVal.Error = "Error while retrieving data";
                result.Add(resVal);

                AggieGlobalLogManager.Fatal("ProductResourcesController :: GetProductResourcesList failed :: " + ex.Message);
            }
            return result;
        }
        [ActionName("   ")]
        public IList<ActivityDescriptions> GetAllActivityDescriptions()
        {
            IList<ActivityDescriptions> result = new List<ActivityDescriptions>();
            try
            {
                IEnumerable<ActivityDescriptions> listData = null;
                var connectionString = "AggieGlobal";
                var repo = new ProductResourcesManager(connectionString);
                listData = repo.GetAllActivityDescriptions();
                if (listData == null && listData.Count() == default(int))
                {
                    ActivityDescriptions resVal = new ActivityDescriptions();
                    resVal.Status = ResponseStatus.Failed;
                    resVal.Error = "No Data found";
                    result.Add(resVal);
                }
                else if (listData != null && listData.Count() > default(int))
                {
                    foreach (ActivityDescriptions resData in listData)
                    {
                        ActivityDescriptions resVal = new ActivityDescriptions();
                        resVal.ActivityDescription = resData.ActivityDescription;
                        resVal.ActivityDescriptionId = resData.ActivityDescriptionId;
                        resVal.ProductType = resData.ProductType;
                        resVal.Status = ResponseStatus.Successful;
                        result.Add(resVal);
                    }
                }
                repo.Dispose();
            }
            catch (Exception ex)
            {
                ActivityDescriptions resVal = new ActivityDescriptions();
                resVal.Status = ResponseStatus.Failed;
                resVal.Error = "Error while retrieving data";
                result.Add(resVal);
                AggieGlobalLogManager.Fatal("ProductResourcesController :: GetAllActivityDescriptions failed :: " + ex.Message);
            }
            return result;
        }
    }
}
