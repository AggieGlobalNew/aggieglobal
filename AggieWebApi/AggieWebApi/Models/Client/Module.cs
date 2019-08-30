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