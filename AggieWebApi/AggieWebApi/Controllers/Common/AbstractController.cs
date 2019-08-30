/* ========================================================================
 * Includes    : AbstractController.cs
 * Website     : http://www.atlassoft.com
 * Create Date : May, May 08, 2019 by Sudipta Sarkar
 * Details     : Provides a collection of abstract functionalities for the controllers of AggieGlobal.WebApi.
 * Copyright © 2019 Atlas Software Technologies All rights reserved
 * ========================================================================
 * Release History
 * ---------------
 * DESCRIPTION					CREATED / MODIFIED BY
 * -----------                  ---------------------
 * Initial Implementation		Sudipta Sarkar
 * ========================================================================
 * Change History
 * --------------
 * Date: Wed, May 8, 2019 By Sudipta Sarkar
 * -------------------------------------
 * 1.PageResults [Existing]: Change in page calculation when ordinal is greater than zero.
 */

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http;
using AggieGlobal.Business.Manager;
using AggieGlobal.WebApi.DataAccess.Common;
using AggieGlobal.WebApi.Filters;
using AggieGlobal.WebApi.Infrastructure;
using AggieGlobal.WebApi.Models;
using AggieGlobal.WebApi.Models.Public;
using System.Collections.Concurrent;
using AggieGlobal.Models.Common;
using AggieGlobal.Models.Client;
using AggieWebApi.Business.Manager;
using AggieWebApi.Business;
using AggieGlobal.WebApi.Business;

namespace AggieGlobal.WebApi.Controllers.Common
{

    [ExceptionHandling]
    public abstract class AbstractController<TItem> : ApiController where TItem : ModelBase, new()
    {
        IRequestContext requestContext;
        IResponseContext responseContext;
        byte[] byteStream;

        private readonly IGlobalApp _GlobalApp = null;

        protected AbstractController(IRequestContext requestContext = null, IResponseContext responseContext = null)
        {
            _GlobalApp = GlobalApp.Instance;

            this.requestContext = requestContext ?? new WebRequestContext();
            this.responseContext = responseContext ?? new WebResponseContext();
        }

        protected IRequestContext RequestContext
        {
            get
            {
                return requestContext;
            }
        }

        protected IResponseContext ResponseContext
        {
            get
            {
                return responseContext;
            }
        }

        protected Account CurrentUser
        {
            get
            {
                return RequestContext.CurrentUser;
            }
        }

        protected NameValueCollection Settings
        {
            get
            {
                return ConfigurationManager.AppSettings;
            }
        }

       
        protected string DecryptHeader(string headerkey, bool decrypt = true)
        {
            if (decrypt)
                return EncryptionHelper.DecryptHeader(this.ControllerContext.Request.Headers, headerkey);
            else
            {
                var value = this.ControllerContext.Request.Headers.GetValues(headerkey).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(value))
                    throw new WebApiException(ErrorList.UnableToProcess, System.Net.HttpStatusCode.PreconditionFailed);

                return value;
            }
        }

        protected void AppendLocation(string resourceLocation, int resourceId)
        {
            if (resourceId > 0)
                ResponseContext.SetProperties(Request.Properties, WebHeaders.Location, Request.RequestUri.AbsoluteUri + resourceLocation + resourceId);
            else
                throw new WebApiException(ErrorList.UnableToProcess, System.Net.HttpStatusCode.NotAcceptable);
        }

        protected Page PageResults(int total, int ordinal = -1)
        {
            var pageSize = RequestContext.ContainsHeader(WebHeaders.PerPage) ? int.Parse(RequestContext.GetHeaderValue(WebHeaders.PerPage)) : 10;
            //TODO: need to populate according to specific device settings

            int start = 1, end = total;

            if (!RequestContext.ContainsHeader(WebHeaders.Page) && !RequestContext.ContainsHeader(WebHeaders.PageForId))
            {
                ResponseContext.SetProperties(Request.Properties, WebHeaders.AcceptRanges, WebHeaders.Page);
            }
            else
            {
                var maxPage = total % pageSize == 0 ? total / pageSize : total / pageSize + 1;
                if (Request != null)
                    ResponseContext.SetProperties(Request.Properties, WebHeaders.MaxPage, maxPage.ToString());
                var page = -1;

                if (ordinal > 0)
                {
                    var previousPage = (ordinal % pageSize == 0) ? (ordinal / pageSize) - 1 : ordinal / pageSize;
                    page = previousPage + 1;

                    start = previousPage * pageSize + 1;
                    end = (start + pageSize - 1) <= total ? (start + pageSize - 1) : total;
                }
                else
                {
                    page = int.Parse(RequestContext.GetHeaderValue(WebHeaders.Page));

                    if (page <= maxPage)
                    {
                        start = ((page - 1) * pageSize) + 1;
                        end = page * pageSize <= total ? page * pageSize : total;
                    }
                    else
                    {
                        start = 0;
                        end = 0;
                    }
                }

                if (start > 0 && end > 0 && page > 0)
                {
                    if (Request != null)
                    {
                        ResponseContext.SetProperties(Request.Properties, WebHeaders.ContentRange, string.Format("{0}-{1}/{2}", start, end, total));
                        ResponseContext.SetProperties(Request.Properties, WebHeaders.PreviousPage, (start == 1 ? 0 : page - 1).ToString());
                        ResponseContext.SetProperties(Request.Properties, WebHeaders.NextPage, (end < total ? page + 1 : 0).ToString());
                    }
                }
            }

            return new Page() { Start = start, End = end };
        }

        protected IEnumerable<TItem> PageResults(IEnumerable<TItem> items)
        {
            var page = PageResults(items.Count()); // this is a mojor performace hit
            // but since db cannot do such filtering app has to do it.
            // this is only when nothing else is possible

            return items.Skip(page.Start - 1).Take(page.End - page.Start + 1);
        }

        protected string BuildFilterClause(string format, string value)
        {
            string isScalable = HttpContext.Current.Session[ApplicationConstant.DBIdentier] as string;
            return !string.IsNullOrWhiteSpace(value) ? string.Format(isScalable.ToLower() == "false" ? format : format.Replace("ESCAPE '\\'", "ESCAPE '\\\\'"), value.Replace("'", "''")) : string.Empty;
        }

        protected string BuildFilterClauseWithEscape(string format, string value)
        {
            string isScalable = HttpContext.Current.Session[ApplicationConstant.DBIdentier] as string;
            return !string.IsNullOrWhiteSpace(value) ? string.Format(isScalable.ToLower() == "false" ? format : format.Replace("ESCAPE '\\'", "ESCAPE '\\\\'"), value.Replace("'", "''").Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_").Replace("[", "\\[")) : string.Empty;
        }

       

        protected class Page
        {
            public int Start;
            public int End;
        }

        #region Initialize bussiness manager
        protected IAccountManager AccountManager
        {
            get
            {
                return _GlobalApp.GetAccountManager(this.CurrentUser);
            }
        }
        protected IActivityManager ActivityManager
        {
            get
            {
                return _GlobalApp.GetActivityManager(this.CurrentUser);
            }

        }
        protected IFarmManager FarmManager
        {
            get
            {
                return _GlobalApp.GetFarmManager(this.CurrentUser);
            }

        }
        protected IProductManager ProductManager
        {
            get
            {
                return _GlobalApp.GetProductManager(this.CurrentUser);
            }

        }
        protected IPlotManager PlotManager
        {
            get
            {
                return _GlobalApp.GetPlotManager(this.CurrentUser);
            }

        }
        protected IProductResourcesManager ProductResourcesManager
        {
            get
            {
                return _GlobalApp.GetProductResourcesManager(this.CurrentUser);
            }

        }
       
        #endregion



    }
}