using System.Collections.Generic;

namespace MyAggieNew
{
    public class SoilMaster
    {

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
        public IEnumerable<SoilDetailResponse> soildetail { get; set; }
        public IEnumerable<SoilPhDetailResponse> soilphdetail { get; set; }
    }
}