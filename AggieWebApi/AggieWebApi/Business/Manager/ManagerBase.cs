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
using System;
using AggieGlobal.WebApi.DataAccess.Common;
using System.Collections.Generic;
using System.Configuration;
using System.Collections.Specialized;

namespace AggieGlobal.WebApi.Business.Managers
{
    internal abstract class ManagerBase : Disposable, IBusinessManager
    {
        #region Member Variables
        protected readonly string _dbConnectionStringName;
        #endregion

        #region Constructor
        protected ManagerBase()
        { }

        protected ManagerBase(string dbConnectionStringName)
        {
            this._dbConnectionStringName = dbConnectionStringName;
        }

        #endregion

        #region Public Methods/Properties
        public int BrandServerID
        {
            get
            {
                int id = 1000;
                if (ConfigurationManager.AppSettings["BrandServerID"] != null)
                    int.TryParse(ConfigurationManager.AppSettings["BrandServerID"], out id);
                return id;
            }
        }

        protected NameValueCollection Settings
        {
            get
            {
                return ConfigurationManager.AppSettings;
            }
        }

        #endregion

        protected override void doCleanup()
        {
        }
    }
}