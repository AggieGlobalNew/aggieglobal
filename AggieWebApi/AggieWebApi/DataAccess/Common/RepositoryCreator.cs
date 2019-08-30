
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

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Diagnostics;
using System.Web;
using AggieGlobal.WebApi.Controllers.Common;
using AggieGlobal.WebApi.DataAccess.Interface;
using AggieGlobal.WebApi.DataAccess.Global;
using AggieWebApi.DataAccess.Global;

namespace AggieGlobal.WebApi.DataAccess.Common
{
    public class RepositoryCreator
    {
        private const string _dbConnectionStringName = "AggieDB";

        public RepositoryCreator()
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session[ApplicationConstant.DBIdentier] != null)
            {
                if (ConfigurationManager.AppSettings["EnableSqlConnection"] != null && ConfigurationManager.AppSettings["EnableSqlConnection"] == "false")
                    HttpContext.Current.Session[ApplicationConstant.DBIdentier] = "true";

                string isScalable = HttpContext.Current.Session[ApplicationConstant.DBIdentier] as string;
            }
        }

        public RepositoryCreator(string dbIndentifier)
        {
            string enableSqlConnection = string.Empty;
            if (ConfigurationManager.AppSettings["EnableSqlConnection"] != null)
                enableSqlConnection = ConfigurationManager.AppSettings["EnableSqlConnection"];
            if (enableSqlConnection == "true") dbIndentifier = "true";
            else dbIndentifier = "false";
        }

        public IAccountRepository AccountRepository
        {
            get
            {
                return new AccountRepository();
            }
        }
        public IFarmRepository FarmRepository
        {
            get
            {
                return new FarmRepository();
            }
        }
        public IPlotRepository PlotRepository
        {
            get
            {
                return new PlotRepository();
            }
        }
        public ISoilRepository SoilRepository
        {
            get
            {
                return new SoilRepository();
            }
        }
        public ISoilPhRepository SoilPhRepository
        {
            get
            {
                return new SoilPhRepository();
            }
        }
        public IProductRepository ProductRepository
        {
            get
            {
                return new ProductRepository();
            }
        }
        public ICategoryRepository CatgeoryRepository
        {
            get
            {
                return new CategoryRepository();
            }
        }
        public IActivityRepository ActivityRepository
        {
            get
            {
                return new ActivityRepository();
            }
        }

        public IProductRessourcesRepository ProductRessourcesRepository
        {
            get
            {
                return new ProductRessourcesRepository();
            }
        }
        public IActivityDescriptionRepository ActivityDescriptionRepository
        {
            get
            {
                return new ActivityDescriptionRepository();
            }
        }
    }
}