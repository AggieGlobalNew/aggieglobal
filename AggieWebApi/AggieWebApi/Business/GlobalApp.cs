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
using AggieGlobal.WebApi.Common;
using AggieWebApi.Business;
using AggieWebApi.Business.Manager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace AggieGlobal.WebApi.Business
{
    internal sealed class GlobalApp : Disposable, IGlobalApp
    {
        #region Static Variables
        private static IGlobalApp m_oneAndOnlyApp;
        #endregion


        #region Member Variables
        private readonly string _dbConnectionStringName;
        #endregion

        #region  Singleton Implementation
        public static void Initialize(string dbConnectionStringName)
        {
            if (m_oneAndOnlyApp != null)
            {
                throw new InvalidOperationException("GlobalApp object has already been initialized!");
            }
            //Trace.TraceInformation("Initializing GlobalApp Instance...");
            m_oneAndOnlyApp = new GlobalApp(dbConnectionStringName);
        }

        public static IGlobalApp Instance
        {
            get
            {
                if (m_oneAndOnlyApp == null)
                {
                    throw new InvalidOperationException("GlobalApp object has never been initialized!");
                }
                return m_oneAndOnlyApp;
            }
        }
        #endregion


        #region Constructor
        private GlobalApp(string dbConnectionStringName)
        {
            AggieGlobalLogManager.Info("Instantiating the 1 & only GlobalApp instance [#:{0}]", GetHashCode());
            this._dbConnectionStringName = dbConnectionStringName;
        }

        protected override void doCleanup()
        {
            AggieGlobalLogManager.OnExceptionLogged -= OnExceptionLogged;
        }

        private void OnExceptionLogged(DateTime exceptionRaisedAt, Exception e, string errorMessage, string[] exceptionDetail)
        {

        }


        public IAccountManager GetAccountManager(Account currentUser)
        {
            IAccountManager m = new AccountManager(_dbConnectionStringName);
            return m;
        }
        public IActivityManager GetActivityManager(Account currentUser)
        {
            IActivityManager m = new ActivityManager(_dbConnectionStringName);
            return m;
        }
        public IFarmManager GetFarmManager(Account currentUser)
        {
            IFarmManager m = new FarmManager(_dbConnectionStringName);
            return m;
        }
        public IPlotManager GetPlotManager(Account currentUser)
        {
            IPlotManager m = new PlotManager(_dbConnectionStringName);
            return m;
        }
        public IProductManager GetProductManager(Account currentUser)
        {
            IProductManager m = new ProductManager(_dbConnectionStringName);
            return m;
        }
        public IProductResourcesManager GetProductResourcesManager(Account currentUser)
        {
            IProductResourcesManager m = new ProductResourcesManager(_dbConnectionStringName);
            return m;
        }
        #endregion
    }
}