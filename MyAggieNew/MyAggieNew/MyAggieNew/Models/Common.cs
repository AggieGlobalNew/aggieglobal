namespace MyAggieNew
{
    public static class Common
    {
        /// Local
        //public static string UrlBase = "http://192.168.0.200/AggieGlobal/api/";
        /// Staging
        ////public static string UrlBase = "http://106.51.71.50/AggieGlobal/api/";
        public static string UrlBase = "http://myaggie.atlassoft.com/AggieGlobal/api/";
        /// AWS
        //public static string UrlBase = "http://ec2-18-234-252-145.compute-1.amazonaws.com/AggieGlobal/api/";

        public static string DownloadUrlPart = "Common/DownloadFile?relativePath={0}";
    }

    public static class CommonChatBot
    {
        public static string UrlBase = "https://aggieglobal.azurewebsites.net/qnamaker/";
        public static string UrlQuestionMakerPart = "knowledgebases/8cb2d010-d8c6-436c-9db8-436d87b8e0fd/generateAnswer";
    }

    public static class CommonEmailSetup
    {
        public static string Host = "smtp.gmail.com";
        public static int Port = 587;
        public static string AdminEmailID = "admin@aggieglobal.com";
        public static string AdminEmalPassword = "@dm!nWatch";
        public static string SupportEmailID = "hello@aggieglobal.com";
        public static string SupportEmailPassword = "Aggie@dm!n";
    }

    public class CommonModuleResponse : ModelBase
    {
        public byte[] fileStream { get; set; }
        public ProductDetailResponse productdata { get; set; }
    }

    public enum ProductType
    {
        None = 0,
        Crop = 1,
        LiveStock = 2,
        Resource = 3
    }

    public enum ProductRessourceType
    {
        None = 0,
        LivestocksellingLocation = 1,
        LiveStockUsage = 2,
        LivestockUtility = 3,
        ResourceMaintenaceCostType = 4,
        ResourceType = 5,
        ResourceCostType = 6
    }
}