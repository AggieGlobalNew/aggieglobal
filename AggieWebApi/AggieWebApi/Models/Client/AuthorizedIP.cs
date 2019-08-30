using AggieGlobal.Models.Common;
using AggieGlobal.WebApi.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AggieGlobal.WebApi.Models.Client
{
    public class AuthorizedIP : ModelBase
    {
        public int PWAccountID { get; set; }
        public PCModule PCModuleID { get; set; }
        public string IPAddress { get; set; }
        public List<IPAddress> IPs
        {
            get
            {
                List<IPAddress> ip = new List<IPAddress>();

                if (!string.IsNullOrEmpty(this.IPAddress))
                    ip = JsonConvert.DeserializeObject<List<IPAddress>>(this.IPAddress);

                return ip;
            }
            set { }
        }
        public DateTime CreateDate { get; set; }
    }

    public class IPAddress : ModelBase
    {
        public string IP { get; set; }
        public string IPRanges { get; set; }
        public List<HostAddressRange> HostAddressRange { get; set; }
        public int MaskBits { get; set; }
        public int Subnet { get; set; }
        public string BaseOctet { get; set; }
        public int StartFourthOctet { get; set; }
        public int EndFourthOctet { get; set; }
    }

    public class HostAddressRange : ModelBase
    {
        public string StartIP { get; set; }
        public string EndIP { get; set; }
    }
}