namespace MyAggieNew
{
    public class PlotDetail
    {

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
}