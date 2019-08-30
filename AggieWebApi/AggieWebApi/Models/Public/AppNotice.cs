using System;
using System.Xml.Serialization;
using AggieGlobal.Models.Common;
using AggieGlobal.WebApi.Models.Client;
using Newtonsoft.Json;

namespace AggieGlobal.WebApi.Models.Public
{
    public class AppNotice : ModelBase
    {
        public int NoticeID { get; set; }
        public AppType AppType { get; set; }
        public AppNoticeType NoticeType { get; set; }
        public string NoticeTitle { get; set; }
        public string Message { get; set; }
        public string NoticeStartDateString { get; set; }
        //public long NoticeStartDateLong { get; set; }
        public double NoticeStartDateDouble { get; set; }
        public string EventStartDateString { get; set; }
        //public long EventStartDateLong { get; set; }
        public double EventStartDateDouble { get; set; }
        public int? EventDuration { get; set; }
        public int NoOfWebNotification { get; set; }
        public int NoOfEmailBroadcast { get; set; }
        public int NoOfEmailBroadcastSent { get; set; }
        public TemplateSettings TemplateSetting { get; set; }
        public PlacementSettings PlacementSetting { get; set; }
        public bool IsActive { get; set; }

        [JsonIgnore][XmlIgnore]
        public DateTime NoticeStartDate { get; set; }
        [JsonIgnore][XmlIgnore]
        public DateTime EventStartDate { get; set; }
        [JsonIgnore][XmlIgnore]
        public int CreateBy { get; set; }
        [JsonIgnore][XmlIgnore]
        public DateTime CreateDate { get; set; }
    }

    public class AppNoticeUserResponse : ModelBase
    {
        public int UserResponseID { get; set; }
        public int NoticeID { get; set; }
        public int PWUserID { get; set; }
        public bool ReadFlag { get; set; }
        public int NoOfNotificationShown { get; set; } 
        [JsonIgnore][XmlIgnore]
        public DateTime ReadDate { get; set; }
    }

    /// <summary>
    /// CodeIdentifier is 'ANT' in LK_Codes
    /// </summary>
    public enum AppNoticeType
    {
        Outage = 1,
        Announcement = 2,
        UpgradeAvailable = 3
    }

    /// <summary>
    /// CodeIdentifier is 'PAT' in LK_Codes
    /// </summary>
    public enum AppType
    {
        AggieGlobal = 1
    }

    public enum TemplateSettings
    {
        Notification = 1,
        Modal = 2
    }

    public enum PlacementSettings
    {
        Inside = 1,
        Outside = 2
    }
}