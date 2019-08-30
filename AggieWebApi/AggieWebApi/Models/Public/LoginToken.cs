using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using AggieGlobal.WebApi.Models.Client;
using AggieGlobal.Models.Common;
using Newtonsoft.Json;

namespace AggieGlobal.WebApi.Models.Public
{
    public class LoginToken : ModelBase
    {
        public string HashKey { get; set; }
        public int PWUserID { get; set; }
        public DateTime? TokenExpiryDate { get; set; }
        public bool IsActive { get; set; }


        [JsonIgnore][XmlIgnore]
        public int TokenID { get; set; }
        [JsonIgnore][XmlIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore][XmlIgnore]
        public DateTime ModifyDate { get; set; }
    }
}