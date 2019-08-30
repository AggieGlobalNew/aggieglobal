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
using AggieGlobal.Models.Client;
using AggieGlobal.WebApi.Common;
using AggieGlobal.WebApi.DataAccess.Common;

namespace AggieGlobal.WebApi.Business.Managers
{
    internal abstract class BusinessManagerBase : ManagerBase
    {
        #region Member Variables
        protected readonly Account _userIdentity;        
        #endregion

        #region Constructor
        protected BusinessManagerBase()
            
        {

        }
        protected BusinessManagerBase(Account userIdentity)
        {
            this._userIdentity = userIdentity;                        
        }


        #endregion

        #region Public Methods/Properties
        /// <summary>
        /// Owner for whom this Manager will work for
        /// </summary>
        /// 
        public Account Owner
        {
            get
            {
                return _userIdentity;
            }
        }        

      
        #endregion

        #region Protected/Private Methods/Properties

        protected override void doCleanup()
        {
        }

        #endregion
    }
}