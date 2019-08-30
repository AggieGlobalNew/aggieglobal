using System;
using System.Collections.Generic;

namespace MyAggieNew
{
    public class ActivityDetail
    {

    }

    public class ActivityDetailCountByDateResponse : ModelBase
    {
        public int ActivityCount { get; set; }
        public DateTime ActivityDate { get; set; }
    }

    public class ActivityDetailResponse : ModelBase    {        public string ActivityId { get; set; }        public string ProductId { get; set; }        public string ProductName { get; set; }        public string CategoryID { get; set; }        public string CategoryName { get; set; }        public DateTime LastHarvestedDate { get; set; }        public bool IsHarvestedBefore { get; set; }        public bool IsSoldBefore { get; set; }        public string IsSoldBeforeNoReason { get; set; }        public decimal SoldPrice { get; set; }        public DateTime ActivityDate { get; set; }        public DateTime PlantationDate { get; set; }        public bool DeletionFlag { get; set; }        public string ResourceName { get; set; }        public int TotalNumberOfResource { get; set; }        public int ResourceCostType { get; set; }        public decimal ResourcePrice { get; set; }        public int ProductTypeId { get; set; }        public int ProductTypeName { get; set; }        public int ActivityCount { get; set; }        public string PlotId { get; set; }        public string PlotName { get; set; }        public int NumberOfLivestock { get; set; }        public int LiveStockUsageId { get; set; }        public int LiveStockUtilityId { get; set; }        public bool IsLivestockSalable { get; set; }        public DateTime LastDateOfLivestockSold { get; set; }        public Decimal SoldLiveStockAmount { get; set; }        public int LivestocksellingLocationId { get; set; }        public string LivestocksellingLocationName { get; set; }        public string LiveStockName { get; set; }        public string LiveStockUsageName { get; set; }        public string LiveStockUtilityName { get; set; }        public int NumberOfResource { get; set; }        public int ResourceTypeId { get; set; }        public string ResourceTypeName { get; set; }        public int ResourceCostTypeId { get; set; }        public string ResourceCostTypeName { get; set; }        public int SoldLiveStockProductId { get; set; }        public int ActivityDescriptionId { get; set; }        public int ResourceMaintenanceCostTypeId { get; set; }
        public string ResourceMaintenaceCostTypeName { get; set; }
        public string ActivityDescription { get; set; }
        public decimal ResourceMaintenancePrice { get; set; }        public List<ProductResourcesResponse> ProductResourceList { get; set; }    }
}