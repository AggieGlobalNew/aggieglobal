using AggieGlobal.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AggieGlobal.Models.Client
{
    public class PlotDetail : ModelBase
    {
        public int PlotId {get;set;}
        public string PlotName { get;set;}
        public decimal PlotSize  {get;set;}
        public bool Organic { get;set;}
        public float SoilPhId { get;set;}
        public int SoilId { get;set;}
        public string Notes { get;set;}
        public int FarmId { get;set;}
        public int UserID { get; set; }
    }
    public class PlotDetailResponse : ModelBase
    {
        public string PlotName { get; set; }
        public decimal PlotSize { get; set; }
        public bool Organic { get; set; }
        public string SoilPhId { get; set; }
        public string SoilId { get; set; }
        public string Notes { get; set; }
        public string FarmId { get; set; }
        public string PlotId { get; set; }
    }

    public class SoilPhDetail : ModelBase
    {
        public int SoilPhId { get; set; }
        public int SoilPhvalue { get; set; }
    }
    public class SoilDetail : ModelBase
    {
        public int SoilId { get; set; }
        public string SoilName { get; set; }
    }
    public class SoilPhDetailResponse : ModelBase
    {
        public string SoilPhId { get; set; }
        public int SoilPhvalue { get; set; }
    }
    public class SoilDetailResponse : ModelBase
    {
        public string SoilId { get; set; }
        public string SoilName { get; set; }
    }
    public class SoilDataResponse : ModelBase
    {
        public IEnumerable<SoilDetailResponse> soildetail {get;set;}
        public IEnumerable<SoilPhDetailResponse> soilphdetail { get; set; }
    }
}
