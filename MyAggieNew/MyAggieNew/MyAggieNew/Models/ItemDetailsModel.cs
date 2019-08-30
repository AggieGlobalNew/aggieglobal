using System.Collections.Generic;

namespace MyAggieNew
{
    public class ItemDetailsModel
    {
        public IList<ItemPayloadModel> lstpld { get; set; }
        public string Activity { get; set; }
        public string Rate { get; set; }
        public string Method { get; set; }
        public string Location { get; set; }
        public string Notes { get; set; }
    }
}