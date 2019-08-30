namespace MyAggieNew
{
    public class ItemPayloadModel
    {
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public int ItemIcon { get; set; }
    }

    public class ItemPayloadModelWithBitmap
    {
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public Android.Graphics.Bitmap ItemIcon { get; set; }
    }

    public class ItemPayloadModelWithBase64
    {
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string ItemIcon { get; set; }
        public ProductType prdType { get; set; }
    }
}