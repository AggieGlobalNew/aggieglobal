using AggieGlobal.Models.Common;
using AggieGlobal.WebApi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AggieGlobal.Models.Client 
{
    public class ProductDetail : ModelBase
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImageLocation { get; set; }
        public int CategoryID { get; set; }
        public string ProductTypeName { get; set; }
        public int UserId { get; set; }
        public string catImageName { get; set; }
        public string prodImageName { get; set; }
    }
    public class ProductDetailResponse : ModelBase
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImageLocation { get; set; }
        public string CategoryID { get; set; }
        public string ProductTypeName { get; set; }
        public ProductType prodType { get; set; }
        public string catImageName { get; set; }
        public string prodImageName { get; set; }
    }
    public class ActivityDetail : ModelBase
    {
        public int ActivityId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string PlotName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int UserId { get; set; }
        public DateTime LastHarvestedDate { get; set; }
        public bool IsHarvestedBefore { get; set; }
        public bool IsSoldBefore { get; set; }
        public string IsSoldBeforeNoReason { get; set; }
        public decimal SoldPrice { get; set; }
        public DateTime ActivityDate { get; set; }
        public DateTime PlantationDate { get; set; }
        public bool DeletionFlag { get; set; }
        public string ResourceName { get; set; }
        public int TotalNumberOfResource { get; set; }
        public int ResourceCostType { get; set; }
        public decimal ResourcePrice { get; set; }
        public string ProductTypeName { get; set; }
        public int ProductTypeId { get; set; }
        public int ActivityCount { get; set; }
        public string catImageName { get; set; }
        public string prodImageName { get; set; }
        public int PlotId { get; set; }
        public int NumberOfLivestock { get; set; }
        public int LiveStockUsageId { get; set; }
        public int LiveStockUtilityId { get; set; }
        public bool IsLivestockSalable { get; set; }
        public DateTime LastDateOfLivestockSold { get; set; }
        public Decimal SoldLiveStockAmount { get; set; }
        public int LivestocksellingLocationId { get; set; }
        public int ActivationId { get; set; }
        public string LivestocksellingLocationName { get; set; }
        public string LiveStockName { get; set; }
        public string LiveStockUsageName { get; set; }
        public string LiveStockUtilityName { get; set; }
        public int NumberOfResource { get; set; }
        public int ResourceTypeId { get; set; }
        public int ResourceCostTypeId { get; set; }
        public int SoldLiveStockProductId { get; set; }
        public int ActivityDescriptionId { get; set; }
        public int ResourceMaintenanceCostTypeId { get; set; }
        public decimal ResourceMaintenancePrice { get; set; }

        public string ActivityDescription { get; set; }

        public string ResourceTypeName { get; set; }
        public string ResourceCostTypeName { get; set; }
        public string ResourceMaintenaceCostTypeName { get; set; }

    }

    public class ActivityDetailResponse : ModelBase
    {
        public string ActivityId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string PlotName { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public DateTime LastHarvestedDate { get; set; }
        public bool IsHarvestedBefore { get; set; }
        public bool IsSoldBefore { get; set; }
        public string IsSoldBeforeNoReason { get; set; }
        public decimal SoldPrice { get; set; }
        public DateTime ActivityDate { get; set; }
        public DateTime PlantationDate { get; set; }
        public bool DeletionFlag { get; set; }
        public string ResourceName { get; set; }
        public int TotalNumberOfResource { get; set; }
        public int ResourceCostType { get; set; }
        public decimal ResourcePrice { get; set; }
        public int ProductTypeId { get; set; }
        public int ActivityCount { get; set; }
        public string catImageName { get; set; }
        public string prodImageName { get; set; }
        public string PlotId { get; set; }
        public int NumberOfLivestock { get; set; }
        public int LiveStockUsageId { get; set; }
        public int LiveStockUtilityId { get; set; }
        public bool IsLivestockSalable { get; set; }
        public DateTime LastDateOfLivestockSold { get; set; }
        public Decimal SoldLiveStockAmount { get; set; }
        public int LivestocksellingLocationId { get; set; }
        public int ActivationId { get; set; }
        public string LivestocksellingLocationName { get; set; }
        public string LiveStockName { get; set; }
        public string LiveStockUsageName { get; set; }
        public string LiveStockUtilityName { get; set; }

        public int NumberOfResource { get; set; }
        public int ResourceTypeId { get; set; }
        public int ResourceCostTypeId { get; set; }
        public int SoldLiveStockProductId { get; set; }
        public int ActivityDescriptionId { get; set; }
        public int ResourceMaintenanceCostTypeId { get; set; }
        public decimal ResourceMaintenancePrice { get; set; }

        public string ActivityDescription { get; set; }

        public string ResourceTypeName { get; set; }
        public string ResourceCostTypeName { get; set; }
        public string ResourceMaintenaceCostTypeName { get; set; }

        public List<ProductResourcesResponse> ProductResourceList { get; set; }


    }
    public class ActivityDetailCountResponse : ModelBase
    {
        public int ActivityCount { get; set; }
        public DateTime ActivityDate { get; set; }
    }
    public class CategoryMaster
    {
        public int ProductTypeId { get; set; }
        public string ProductTypeName { get; set; }
        public string ProductImageLocation { get; set; }
        public string catImageName { get; set; }
        public string prodImageName { get; set; }
    }
    public class CategoryMasterResponse:ModelBase
    {
        public string ProductTypeId { get; set; }
        public string ProductTypeName { get; set; }
        public string ProductImageLocation { get; set; }
        public string catImageName { get; set; }
        public string prodImageName { get; set; }

    }
    public class UserProductDetail
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
    }
    public enum ProductType
    {
        None=0,
        Crop=1,
        LiveStock=2, 
        Resource=3
    }

    public class ProductResources
    {
        public int ProductResourceId { get; set; }
        public string ProductResourceName { get; set; }
        public ProductRessourceType ProductRessourceType { get; set; }
        public string ProductRessourceTypeName { get; set; }
        public bool IsActive {get;set;}
    }
    public class ProductResourcesResponse : ModelBase
    {
        public string ProductResourceId { get; set; }
        public string ProductResourceName { get; set; }
        public ProductRessourceType ProductRessourceType { get; set; }
    }

    public enum ProductRessourceType
    {
        None=0,
        LivestocksellingLocation =1,
        LiveStockUsage=2,
        LivestockUtility=3,
        ResourceMaintenaceCostType=4,
        ResourceType=5,
        ResourceCostType=6
    }

    public class ActivityDescriptions : ModelBase
    {
        public int ActivityDescriptionId { get; set; }
        public string ActivityDescription { get; set; }
        public int ProductType { get; set; }
    }
}