namespace MyAggieNew
{
    public class CategoryMaster
    {

    }

    public class CategoryMasterResponse : ModelBase
    {
        public string ProductTypeId { get; set; }
        public string ProductTypeName { get; set; }
        public string catImageName { get; set; }
        public string ProductImageLocation { get; set; }
    }
}