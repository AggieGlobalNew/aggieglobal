using System;

namespace MyAggieNew
{
    public class ProductDetail
    {

    }

    /*public class ProductDetailResponse : ModelBase
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImageLocation { get; set; }
        public string CategoryID { get; set; }
        public string ProductTypeName { get; set; }
        public DateTime LastHarvestedDate { get; set; }
        public bool IsHarvestedBefore { get; set; }
        public bool IsSoldBefore { get; set; }
        public bool IsSoldBeforeNoReason { get; set; }
        public decimal SoldPrice { get; set; }
        public DateTime ActivityDate { get; set; }
        public DateTime PlantationDate { get; set; }
        public string ResourceName { get; set; }
        public int TotalNumberOfResource { get; set; }
        public int ResourceCostType { get; set; }
        public decimal ResourcePrice { get; set; }
    }*/

    public class ProductDetailResponse : ModelBase
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImageLocation { get; set; }
        public string catImageName { get; set; }
        public string prodImageName { get; set; }
        public string CategoryID { get; set; }
        public string ProductTypeName { get; set; }
        public ProductType prodType { get; set; }
    }

    public class ProductResourcesResponse : ModelBase
    {
        public string ProductResourceId { get; set; }
        public string ProductResourceName { get; set; }
        public ProductRessourceType ProductRessourceType { get; set; }
    }

    public class ProductActivityAdd : ProductDetailResponse
    {
        public int ActivityDescriptionId { get; set; }
        public string ActivityDescription { get; set; }
        public string PlotId { get; set; }
        public string PlotName { get; set; }

        public DateTime PlantDate { get; set; }
        public bool IsHarvest { get; set; }
        public DateTime LastActivityDate { get; set; }
        public bool IsProductSell { get; set; }
        public string SellDesc { get; set; }
        public decimal SellAmount { get; set; }

        public string LivestockTypeName { get; set; }
        public int ProductCount { get; set; }
        public string LivestockUtilityName { get; set; }
        public int LivestockUtilityId { get; set; }
        public int SellProductId { get; set; }
        public string SellProductName { get; set; }
        public string LiveStockUsageName { get; set; }
        public int LiveStockUsageId { get; set; }


        public string ResourceName { get; set; }
        public int ResourceTypeId { get; set; }
        public string ResourceTypeName { get; set; }
        public int ResourceCostTypeId { get; set; }
        public string ResourceCostTypeName { get; set; }
        public decimal ResourceCostValue { get; set; }
        public int ResourceMaintenaceCostTypeId { get; set; }
        public string ResourceMaintenaceCostTypeName { get; set; }
        public decimal ResourceMaintenaceCostTypeValue { get; set; }
    }

    public class ActivityDescriptions : ModelBase
    {
        public int ActivityDescriptionId { get; set; }
        public string ActivityDescription { get; set; }
        public int ProductType { get; set; }
    }    
}