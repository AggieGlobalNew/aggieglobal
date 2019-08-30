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

using System.Collections.Generic;
using System.Xml.Serialization;
using AggieGlobal.Models.Common;
using Newtonsoft.Json;

namespace AggieGlobal.WebApi.Models.Client
{
    public class Module: ModelBase
    {
        public int? MModuleID { get; set; }
        public int ModuleID { get; set; }
        public string ModuleName { get; set; }        
        public IList<Module> Modules { get; set; }
        [JsonIgnore][XmlIgnore]
        public string ModuleCode { get; set; }
        [JsonIgnore][XmlIgnore]
        public int OrdinalNum { get; set; }
        //public byte[] RawIcon { get; set; }
    }
   
}