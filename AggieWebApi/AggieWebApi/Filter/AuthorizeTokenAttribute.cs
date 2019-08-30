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

using AggieGlobal.WebApi.Common;
using AggieGlobal.WebApi.Controllers.Common;
using AggieGlobal.WebApi.DataAccess.Common;
using AggieGlobal.WebApi.Infrastructure;
using System;
using System.Linq;
using System.Web;
using AggieGlobal.Models.Common;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using AggieGlobal.Models.Client;
using System.Net.Http.Headers;
using System.Net;
using System.Net.Http;

namespace AggieGlobal.WebApi.Filters
{
    public class CustomFilter : AuthorizationFilterAttribute
    {

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext != null)
            {
                // By pass Authorization when AllowAnonymous Attribute found
                if (actionContext.ActionDescriptor.GetCustomAttributes<System.Web.Http.AllowAnonymousAttribute>().Any() || actionContext.ActionDescriptor.GetCustomAttributes<System.Web.Mvc.AllowAnonymousAttribute>().Any())
                    return;
                
                if (!AuthorizeRequest(actionContext))
                {
                    // consider writing to action response instead of throwing http exception
                    // throw new WebApiException(ErrorList.EmptyToken, HttpStatusCode.BadRequest);

                    var error = new ErrorObject(ErrorList.InvalidToken);

                    actionContext.Response = actionContext.Response ?? new HttpResponseMessage();
                    actionContext.Response.StatusCode = HttpStatusCode.BadRequest;
                    actionContext.Response.Content = error.ToContent();

                    //SkySiteLogManager.WriteLog(string.Format("Authorization failed: \r\n{0}", error.ToJson()), Severity.Error);
                }
            }
        }
        private bool AuthorizeRequest(HttpActionContext actionContext)
        {
            bool validateData = false;
            if (HttpContext.Current.Request.ServerVariables["HTTP_REFERER"] != null)
            {
                string strRef = HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];
                if (!string.IsNullOrWhiteSpace(strRef))
                {
                    if (strRef.ToLower().Contains("google"))
                    {
                        throw new WebApiException(ErrorList.PermissionDenied, System.Net.HttpStatusCode.Unauthorized);
                    }
                }
            }


            HttpRequestHeaders iheader = actionContext.Request.Headers;

            if (iheader.Contains("IsRegister") == true)
            {
                var IsRegister = iheader.GetValues("IsRegister").FirstOrDefault();
                if (Convert.ToBoolean(IsRegister) == true)
                {
                    validateData = true;
                }
                Account user = new Account();
                user.AuthReason = AuthReason.Success;
            }
            else if (iheader.Contains("TokenKey")==true)
            {
                var tokenValue = iheader.GetValues("TokenKey").FirstOrDefault();
                string decodetoken = EncryptionHelper.AesDecryption(tokenValue, EncryptionKey.LOG);
                string[] delimenator = new string[1];
                delimenator[0] = "-";
                string[] splitData = decodetoken.Split(delimenator, StringSplitOptions.None);
                Account user = null;
                if (splitData != null && splitData.Count() > default(int))
                {
                    SessionData idata = new SessionData();
                    idata._email = splitData[0];
                    idata._userId = Convert.ToInt32(splitData[1]);
                    idata._deviceid = splitData[2];
                    HttpContext.Current.Session[ApplicationConstant.UserSession] = idata;
                    user = new RepositoryCreator().AccountRepository.GetAcountUserDetailsById(idata._email, idata._userId, idata._deviceid);

                    if (user.EmailId == idata._email && user.UserId == idata._userId && user.UserDeviceId == idata._deviceid)
                    {
                        validateData = true;
                    }
                    if (validateData == true)
                        user.AuthReason = AuthReason.Success;
                    else
                        user.AuthReason = AuthReason.NotSubscribed;

                    if (user.AuthReason != AuthReason.Success)
                    {
                        throw new WebApiException(ErrorList.AccountNotApproved, System.Net.HttpStatusCode.Unauthorized);
                    }
                }
            }
            if (validateData == false)
                { throw new WebApiException(ErrorList.AccountNotApproved, System.Net.HttpStatusCode.Unauthorized); }

            return validateData;
        }

    }
}