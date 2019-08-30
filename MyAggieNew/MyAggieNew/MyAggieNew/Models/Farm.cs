using System;

namespace MyAggieNew
{
    public class Farm
    {

    }

    public class FarmDetailResponse : ModelBase
    {
        public string FarmName { get; set; }
        public int FarmSize { get; set; }
        public string FarmSizeUnit { get; set; }
        public DateTime FarmEstablishedDate
        {
            get
            {
                return this._FarmEstablishedDate.HasValue ? this._FarmEstablishedDate.Value : DateTime.Now;
            }
            set
            {
                this._FarmEstablishedDate = value;
            }
        }
        public string FarmAddress { get; set; }
        public string CoOpName { get; set; }
        public string FarmId { get; set; }
    }
}