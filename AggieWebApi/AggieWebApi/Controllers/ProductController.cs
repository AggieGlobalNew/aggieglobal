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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace AggieWebApi.Controllers
{
    [CustomFilter]
    public class ProductController : AbstractController<ProductDetail>
    {
        public ProductController()
        {
            
        }
        [ActionName("GetCategoryList")]
        public IList<CategoryMasterResponse> GetCategoryList()
        {
            IEnumerable<CategoryMaster> categoryData = null;
            IList<CategoryMasterResponse> responsedata = new List<CategoryMasterResponse>();
            try
            {
                if (HttpContext.Current.Session[ApplicationConstant.UserSession] != null)
                {
                    SessionData sessionObject = (SessionData)HttpContext.Current.Session[ApplicationConstant.UserSession];
                    var connectionString = "AggieGlobal";
                    var repo = new ProductManager(connectionString);
                    categoryData = repo.GetCategoryList();
                    if (categoryData != null && categoryData.Count() > default(int))
                    {
                        foreach (CategoryMaster detail in categoryData)
                        {
                            CategoryMasterResponse response = new CategoryMasterResponse();
                            response.ProductTypeId = EncryptionHelper.AesEncryption(Convert.ToString(detail.ProductTypeId), EncryptionKey.LOG);
                            response.ProductTypeName = detail.ProductTypeName.Trim();
                            response.ProductImageLocation = detail.ProductImageLocation; //fileurl(detail.ProductTypeName.Trim());
                            response.catImageName = detail.catImageName;
                            response.prodImageName = detail.prodImageName;
                            response.Status = ResponseStatus.Successful;
                            responsedata.Add(response);
                        }
                    }
                    else
                    {
                        CategoryMasterResponse response = new CategoryMasterResponse();
                        response.Error = "Failed to retreive data";
                        response.Status = ResponseStatus.Failed;
                        responsedata.Add(response);
                    }
                    repo.Dispose();
                }
            }
            catch (Exception ex)
            {
                CategoryMasterResponse response = new CategoryMasterResponse();
                response.Error = "Failed to retreive data";
                response.Status = ResponseStatus.Failed;
                responsedata.Add(response);
                AggieGlobalLogManager.Fatal("ProductController :: GetCategoryList failed :: " + ex.Message);
            }
            return responsedata;
       }


        [HttpPost]
        [ActionName("CreateSubCategory")]
        public CommonModuleResponse CreateSubCategory([FromBody] CommonModuleResponse file)
        {
            string relativePath = string.Empty;

            var myFile = System.Web.Hosting.HostingEnvironment.MapPath("~/ModuleImg/" + relativePath);

            if (file != null && file.fileStream != null && file.fileStream.Length > default(int) && file.productdata != null)
                try
                {
                    if (HttpContext.Current.Session[ApplicationConstant.UserSession] != null)
                    {
                        try
                        {
                            string imageName = Guid.NewGuid() + ".png";
                            relativePath = file.productdata.ProductImageLocation + imageName;

                            var connectionString = "AggieGlobal";
                            var repo = new ProductManager(connectionString);
                            if (HttpContext.Current.Session[ApplicationConstant.UserSession] != null)
                            {
                                SessionData sessionObject = (SessionData)HttpContext.Current.Session[ApplicationConstant.UserSession];
                                ProductDetail detailData = new ProductDetail();
                                detailData.ProductName = file.productdata.ProductName.Trim();
                                detailData.ProductTypeName = file.productdata.ProductTypeName.Trim();
                                detailData.ProductImageLocation = imageName;
                                detailData.ProductTypeName = file.productdata.ProductTypeName.Trim();
                                detailData.CategoryID = Convert.ToInt32(EncryptionHelper.AesDecryption(file.productdata.CategoryID, EncryptionKey.LOG));
                                detailData.UserId = sessionObject._userId;
                                detailData.Status = ResponseStatus.Successful;
                                detailData.ProductId = repo.CreateSubCategory(detailData);
                                if (detailData.ProductId > default(int))
                                {
                                    myFile += relativePath;
                                    File.WriteAllBytes(myFile, file.fileStream);

                                    file = new CommonModuleResponse();
                                    file.productdata = new ProductDetailResponse();
                                    file.productdata.ProductId = System.Net.WebUtility.UrlEncode(Convert.ToString(EncryptionHelper.AesEncryption(detailData.ProductId.ToString(), EncryptionKey.LOG)));
                                    file.productdata.Status = ResponseStatus.Successful;
                                }
                                else
                                {
                                    file = new CommonModuleResponse();
                                    file.Error = "Failed to save product";
                                    file.Status = ResponseStatus.Failed;
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            file = new CommonModuleResponse();
                            file.Status = ResponseStatus.Failed;
                            file.Error = "Unable to save product file";
                            AggieGlobalLogManager.Fatal("CommonController :: UploadFile failed :: " + ex.Message);
                        }
                    }
                }
                catch (Exception e)
                {
                    file = new CommonModuleResponse();
                    file.Status = ResponseStatus.Failed;
                    file.Error = "Unable to save product file";
                    AggieGlobalLogManager.Fatal("CommonController :: UploadFile failed :: " + e.Message);
                }
            return file;
        }


        [ActionName("GetSubCategoryList")]
        public IEnumerable<ProductDetailResponse> GetSubCategoryList(string catId)
        {
            catId = System.Net.WebUtility.UrlDecode(catId);

            catId = catId.Replace("+", "%20");
            catId = System.Net.WebUtility.UrlDecode(catId);
            catId = catId.Replace(" ", "+");

            bool ret = default(bool);
            IEnumerable<ProductDetail> categoryData = null;
            int ProductTypeId = 0;
            IList<ProductDetailResponse> responsedata = new List<ProductDetailResponse>();
            try
            {
                if (HttpContext.Current.Session[ApplicationConstant.UserSession] != null)
                {
                    SessionData sessionObject = (SessionData)HttpContext.Current.Session[ApplicationConstant.UserSession];

                    if (string.IsNullOrEmpty(catId))
                    {
                        ProductDetailResponse response = new ProductDetailResponse();
                        response.Status = ResponseStatus.Failed;
                        response.Error = "Parameter mismatch";
                        responsedata.Add(response);
                    }
                    else
                    {
                        ProductTypeId = Convert.ToInt32(EncryptionHelper.AesDecryption(Convert.ToString(catId), EncryptionKey.LOG));
                        var connectionString = "AggieGlobal";
                        var repo = new ProductManager(connectionString);
                        categoryData = repo.GetSubCategoryList(ProductTypeId, sessionObject._userId);
                        if (categoryData != null && categoryData.Count() > default(int))
                        {
                            foreach (ProductDetail detail in categoryData)
                            {
                                ProductDetailResponse response = new ProductDetailResponse();
                                response.ProductName = detail.ProductName.Trim();
                                response.ProductTypeName = detail.ProductTypeName.Trim();
                                response.ProductImageLocation = detail.ProductImageLocation; //fileurl(detail.ProductName.Trim());
                                response.ProductTypeName = detail.ProductTypeName.Trim();
                                response.ProductId = EncryptionHelper.AesEncryption(Convert.ToString(detail.ProductId), EncryptionKey.LOG);
                                response.catImageName = detail.catImageName;
                                response.prodImageName = detail.prodImageName;
                                response.Status = ResponseStatus.Successful;
                                if(detail.CategoryID==ProductType.Crop.GetHashCode()) response.prodType = ProductType.Crop;
                                else if (detail.CategoryID == ProductType.LiveStock.GetHashCode()) response.prodType = ProductType.LiveStock;
                                else if (detail.CategoryID == ProductType.Resource.GetHashCode()) response.prodType = ProductType.Resource;
                                responsedata.Add(response);
                            }
                        }
                        else
                        {
                            ProductDetailResponse response = new ProductDetailResponse();
                            response.Error = "Failed to retreive data";
                            response.Status = ResponseStatus.Failed;
                            responsedata.Add(response);
                        }
                        repo.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                ProductDetailResponse response = new ProductDetailResponse();
                response.Status = ResponseStatus.Failed;
                response.Error = "Failed to retreive data";
                responsedata.Add(response);
                AggieGlobalLogManager.Fatal("ProductController :: GetCategoryList failed :: " + ex.Message);
            }
            return responsedata;
        }

        #region Private Functions
        private string fileurl(string fileName)
        {
            string baseurl = string.Empty;
            baseurl += "Common/DownloadFile?relativePath=" + fileName + ".png";
            return baseurl;
        }
        public string GetBaseUrl()
        {
            var request = HttpContext.Current.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (appUrl != "/")
                appUrl = "/" + appUrl;

            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl) + "api/";

            return baseUrl;
        }
        #endregion

    }
}
