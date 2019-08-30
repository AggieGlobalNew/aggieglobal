using System;
using System.Net;
using System.Web.Http;
using AggieGlobal.Business.Manager;
using AggieGlobal.WebApi.Common;
using AggieGlobal.WebApi.Filters;
using AggieGlobal.WebApi.Infrastructure;
using AggieGlobal.WebApi.Models.Public;

namespace AggieGlobal.WebApi.Controllers.Common
{
    [CustomFilter]
    public class ConfigController : ApiController
    {

        /// <summary>
        /// Get restricted characters
        /// </summary>
        /// <param name="pinProjectId"></param>
        /// <returns></returns>
        [HttpGet]
        public string GetRestrictedChars(int pinProjectId)
        {
            if (pinProjectId <= 0)
                throw new WebApiException(ErrorList.EmptyArgument, HttpStatusCode.Forbidden);

            string restrictedChars = string.Empty;
            try
            {


            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal(ex, "ConfigController.GetRestrictedChars: -{0}", ex.Message);
                throw;
            }
            return restrictedChars;
        }

        /// <summary>
        /// Gets Supported photo extensions for upload
        /// </summary>
        /// <param name="pinProjectId"></param>
        /// <returns></returns>
        [HttpGet]
        public string[] GetSuppPhotoExtnsForUpload(int pinProjectId)
        {
            if (pinProjectId <= 0)
                throw new WebApiException(ErrorList.EmptyArgument, HttpStatusCode.Forbidden);

            string[] suppExtns = null;
            try
            {

            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal(ex, "ConfigController.GetSuppPhotoExtnsForUpload: -{0}", ex.Message);
                throw;
            }
            return suppExtns;
        }

        /// <summary>
        /// Gets application location based on db config table
        /// Time zone also will be based on this location
        /// </summary>
        /// <returns>Application location as string</returns>
        [HttpGet]
        public string GetAppLocation()
        {
            string strAppLocation = string.Empty;
            try
            {

            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal(ex, "ConfigController.GetAppLocation: -{0}", ex.Message);
                throw;
            }
            return strAppLocation;
        }

       
        [HttpGet]
        public int DefaultStateID()
        {
            var strDefaultStateID = default(int);
            try
            {

            }
            catch (Exception ex)
            {
                AggieGlobalLogManager.Fatal(ex, "ConfigController.GetAppLocation: -{0}", ex.Message);
                throw;
            }
            return strDefaultStateID;
        }
        
    }
}
