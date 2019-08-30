using System.Collections.Generic;
using System.Xml.Serialization;
using System.ComponentModel;
using System;
using Newtonsoft.Json;
using AggieGlobal.Models.Common;

namespace AggieGlobal.WebApi.Models.Public
{
    public class AppVersionDetail : ModelBase
    {
        public int AppVersionID { get; set; }
        public OSType OSType { get; set; }
        public string AppVersionCode { get; set; }
        public string AppVersionName { get; set; }
        [JsonIgnore][XmlIgnore]
        public string OSVersion { get; set; }
        public string OSMinVersion { get; set; }
        public string OSMaxVersion { get; set; }
        public AppVersionUpdateType UpdateType { get; set; }
        public string UpdateMessage { get; set; }
        public DateTime CreateDate { get; set; }
        public int PCModuleID { get; set; }
        public Int16 IsActive { get; set; }
    }

    public enum AppVersionUpdateType
    {
        [Description("Required")]
        Required = 1,
        [Description("Optional")]
        Optional = 2,
        [Description("Info")]
        Info = 3
    }

    public enum OSType
    {
        [Description("IOS")]
        IOS = 1,
        [Description("Android")]
        Android = 2
    }
}
