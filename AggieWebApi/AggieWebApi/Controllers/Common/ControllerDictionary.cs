/* ========================================================================
 * Includes    : ControllerDictionary.cs
 * Website     : http://www.atlassoft.com
 * Create Date : May, May 08, 2019 by Sudipta Sarkar
 * Details     : Provides a collection of static constants for AggieGlobal.WebApi.
 * Copyright © 2019 Atlas Software Technologies All rights reserved
 * ========================================================================
 * Release History
 * ---------------
 * DESCRIPTION					CREATED / MODIFIED BY
 * -----------                  ---------------------
 * Initial Implementation		Sudipta Sarkar
 * ========================================================================
 * Change History
 * --------------
 */

using System.Collections.Generic;

namespace AggieGlobal.WebApi.Controllers.Common
{
    internal static class WebHeaders
    {
        internal static string ActiveToken = "ActiveToken"; //for user related token
        internal static string AccessTokenKey = "AccessTokenKey";
        internal static string ApiKey = "ApiKey"; //for account related token
        internal static string AppId = "AppID";
        internal static string DetailsView = "DetailsView";
        internal static string ContentRange = "Content-Range";
        internal static string Email = "Email";
        internal static string isContact = "isContact";
        internal static string isForSharedProject = "isForSharedProject";
        internal static string isEncoded = "isEncoded";
        internal static string emailID = "emailID";
        internal static string EnableAnnotation = "EnableAnnotation";
        internal static string Failed = "Failed";
        internal static string FailedDeletion = "Failed-Deletion";
        internal static string FetchItemsCount = "FetchItemsCount";
        internal static string FromLinkModule = "FromLinkModule";
        internal static string IgnorePermission = "IgnorePermission";
        internal static string IsResetPasswordEmailAfterAccountLock = "IsResetPasswordEmailAfterAccountLock";
        internal static string LastSyncContext = "LastSyncContext";
        internal static string Location = "Location";
        internal static string LoginId = "LoginId";
        internal static string SecurityAnswer = "SecurityAnswer";
        internal static string QuestID = "QuestID";
        internal static string MaxPage = "Max-Page";
        internal static string NewLoginId = "NewLoginId";
        internal static string NewPassword = "NewPassword";
        internal static string NextPage = "Next-Page";
        internal static string ObjectId = "ObjectId";
        internal static string Page = "Page";
        internal static string PageForId = "PageForId";
        internal static string Password = "Password";
        internal static string Permission = "Permission";
        internal static string PerPage = "PerPage";
        internal static string PinProjectId = "PinProjectId";
        internal static string PINProjectId = "PINProjectId";
        internal static string PWAccountId = "PWAccountId";
        internal static string Prefix = "Header.";
        internal static string PreviousPage = "Previous-Page";
        internal static string ResponseStatus = "ResponseStatus";
        internal static string TokenExpires = "TokenExpires";
        internal static string TokenKey = "TokenKey";
        internal static string TotalRecordCount = "TotalRecordCount";
        internal static string UtcDate = "UtcDate";
        internal static string Viewer = "Viewer";
        internal static string ModuleKeyRequest = "ModuleKeyRequest";
        internal static string RegFromFnADevice = "RegFromFnADevice";
        internal static string PWUserId = "PWUserId";
        internal static string isGuestUser = "isGuestUser";
        internal static string PWPProjectUserID = "PWPProjectUserID";
        internal static string ObjectLinkKey = "ObjectLinkKey";
        internal static string ServerToken = "ServerToken"; //user related token for PWP website
        internal static string SystemName = "SystemName";
        internal static string IsLinked = "IsLinked";
        internal static string LastProjectSyncContext = "LastProjectSyncContext";
        internal static string LastLatestDrawingSyncContext = "LastLatestDrawingSyncContext";
        internal static string IsExpirable = "IsExpirable";
        internal static string TotalOrdinals = "TotalOrdinals";
        internal static string DocumentDesc = "DocumentDesc";
        internal static string DocumentId = "DocumentId";
        internal static string ClientCurrDate = "ClientCurrDate";
        internal static string RevisionNum = "RevisionNum";
        internal static string LatestDocumentStorage = "LatestDocumentStorage";
        internal static string LinkGroupID = "LinkGroupID";
        internal static string FileCount = "FileCount";
        internal static string ShortcutFileCount = "ShortcutFileCount";
        internal static string ChildDocumentUnpublishCount = "ChildDocumentUnpublishCount";
        internal static string SortBy = "SortBy";
        internal static string SortOrder = "SortOrder";
        internal static string FilterBy = "FilterBy";
        internal static string CustomFilterBy = "CustomFilterBy";
        internal static string TotalUnpublishCount = "TotalUnpublishCount";
        internal static string FailedPublishedDocs = "FailedPublishedDocs";
        internal static string OAuthProvider = "OAuthProvider";
        internal static string OAuthSPID = "OAuthSPID";
        internal static string AfterActivateEmail = "AfterActivateEmail";
        internal static string OAuthEmail = "OAuthEmail";
        internal static string FailureMode = "FailureMode";
        internal static string ExtentendedSheetLeft = "ExtentendedSheetLeft";
        internal static string TrailSheetAvailable = "TrailSheetAvailable";
        internal static string IsNewJason = "IsNewJason";
        internal static string HyperlinkReq = "HyperlinkReq";
        internal static string folderId = "folderId";
        internal static string hdrPwAccountId = "pwAccountId";
        internal static string PWPUserID = "PWPUserID";
        internal static string IsMobile = "IsMobile";
        internal static string sharefolderprintorder = "sharefolderprintorder";
        internal static string IsThisrefDocRevID = "IsThisrefDocRevID";
        internal static string StartDate = "StartDate";
        internal static string EndDate = "EndDate";
        internal static string SubmittalDownloadType = "SubmittalDownloadType";
        internal static string SubmittalDownloadRequestByDevice = "SubmittalDownloadRequestByDevice";
        internal static string DownloadObjectId = "DownloadObjectId";

        internal static string ZIPName = "ZipName";

        internal static string AllowMergeByName = "AllowMergeByName";


        internal static string CreateUploadLogIdAtServer = "CreateUploadLogIdAtServer";

        internal static string AutoRename = "AutoRename";
        internal static string LastPunchListSyncContext = "LastPunchListSyncContext";
        internal static string LastMasterItemSyncContext = "LastMasterItemSyncContext";
        internal static string LastRFIListSyncContext = "LastRFIListSyncContext";
        internal static string CreateChangeSet = "CreateChangeSet";
        internal static string CheckForDeleted = "CheckForDeleted";
        internal static string RegistrationKey = "RegistrationKey";
        internal static string ShareResponseStatus = "ShareResponseStatus";
        internal static string ProjectUserCurrentRole = "UserCurrentRole";

        internal static string DeviceType = "DeviceType";
        internal static string IncludeOriginal = "includeoriginal";
        internal static string IncludeSettings = "includesettings";
        internal static string UserId = "UserId";

        internal static string SessionID = "SessionID";
        internal static string ZoomLevel = "ZoomLevel";

        internal static string isScalable = "isScalable";
        internal static string AppVersion = "AppVersion";
        internal static string IsTwoStepEnabled = "IsTwoStepEnabled";
        internal static string PasswordPIN = "PasswordPIN";
        internal static string AuthSecurityMode = "AuthSecurityMode";

        internal static string AdditionalAuth = "AdditionalAuth";
        internal static string ErrorCode = "ErrorCode";
        internal static string ErrorMessage = "ErrorMessage";
        internal static string InvalidLoginAttemptCount = "InvalidLoginAttemptCount";
        internal static string RemainingLoginAttemptCount = "RemainingLoginAttemptCount";
        internal static string InvalidPINAttemptCount = "InvalidPINAttemptCount";
        internal static string RemainingPINAttemptCount = "RemainingPINAttemptCount";
        internal static string MarkupDeleted = "MarkupDeleted";

        internal static string IsSSORequest = "IsSSORequest";

        internal static string isFromFilePublishing = "isFromFilePublishing";
        internal static string docRevId = "docRevId";
        internal static string docPassword = "docPassword";
        internal static string ssoRequestId = "ssorequestid";

        //F&A viewer
        public static string ViewerAttachmentType = "ViewerAttachmentType";
        public static string IsDevice = "IsDevice";
        public static string IsMemcachecheck = "ismemcachecheck";
        internal static string AcceptRanges;
    }

    internal static class QueryConstant
    {
        internal static string ProjectAccepted = "accepted";
        internal static string ProjectNotYetResponded = "notyetresponded";
        internal static string ProjectDenied = "denied";
    }

    internal static class ApplicationConstant
    {
        internal static string DBIdentier = "DBIdentier";
        internal static string NotificationConfigPath = "NotificationConfigPath";
        internal static string UserSession = "UserSession";
    }

    internal static class OrderBy
    {
        internal static IList<string> DocumentOrder = new List<string>() { "PD.CreateDate", "PD.CreateDate DESC", "PD.DocumentName ASC, PD.OrdinalNum ASC", "PD.DocumentName DESC, PD.OrdinalNum ASC", "PD.OrdinalNum ASC", "PD.OrdinalNum ASC,DR.RevisionDate DESC", "PD.OrdinalNum DESC,DR.RevisionDate DESC", "DR.RevisionDate", "DR.RevisionDate DESC", "PD.Discipline ASC, PD.DocumentName ASC", "PD.Discipline DESC, PD.DocumentName ASC" };
        internal static IList<string> GroupMemberOrder = new List<string>() { "AO.FirstName", "AO.FirstName DESC", "AO.LastName", "AO.LastName DESC", "AO.CompanyName", "AO.CompanyName DESC", "AO.City", "AO.City DESC" };
        internal static IList<string> FolderOrder = new List<string>() { "PF.CreateDate", "PF.IsVirtualFolder DESC,PF.CreateDate DESC", "PF.IsVirtualFolder DESC,PF.FolderName", "PF.IsVirtualFolder DESC, PF.FolderName DESC", "PF.OrdinalNum ASC", "PF.OrdinalNum DESC", "PF.IsVirtualFolder DESC, PF.OrdinalNum DESC", "PF.IsVirtualFolder DESC, PF.OrdinalNum ASC" };
        internal static IList<string> ProjectOrder = new List<string>() { 
            " PR.ProjectName", " PR.ProjectName DESC", 
            " PR.ProjectNumber", " PR.ProjectNumber DESC",
            " PR.CreateDate", " PR.CreateDate DESC",
            " PR.ProjectStartDate", " PR.ProjectStartDate DESC",
            " PR.City", " PR.City DESC",
            " PPS.DownloadFileStorage", " PPS.DownloadFileStorage DESC",
            " AU1.FirstName,AU1.LastName", " AU1.FirstName DESC,AU1.LastName DESC",
            " IsFavorite DESC,PR.ProjectName"
        };
        internal static IList<string> AlbumOrder = new List<string>() { "CreateDate", "CreateDate DESC", "AlbumName", "AlbumName DESC" };
        internal static IList<string> AlbumImageOrder = new List<string>() { "CreateDate", "CreateDate DESC", "ImageName", "ImageName DESC" };
        internal static IList<string> UserOrder = new List<string>() { "AU.FirstName", "AU.FirstName DESC", "AU.LastName", "AU.LastName DESC", "AO.CompanyName", "AO.CompanyName DESC" };
        internal static IList<string> ShareUserOrder = new List<string>() { "FirstName", "FirstName DESC", "LastName", "LastName DESC", "CompanyName", "CompanyName DESC" };
        internal static IList<string> ActivityOrder = new List<string>() { "AG.ActivityDate", "AG.ActivityDate DESC" };
        internal static IList<string> UserSystemMapOrder = new List<string>() { " SystemName", " SystemName DESC", " CreateDateUtc", " CreateDateUtc DESC" };
        internal static IList<string> ContactOrder = new List<string>() { "AO.FirstName", "AO.FirstName DESC", "AO.LastName", "AO.LastName DESC", "AO.CompanyName", "AO.CompanyName DESC", "AO.City", "AO.City DESC" };
        internal static IList<string> SharedHistoryOrder = new List<string>() { 
            " PG.CreateDate", " PG.CreateDate DESC",
            " PG.Email", " PG.Email DESC",
            " PR.ProjectName", " PR.ProjectName DESC"
        };
        internal static IList<string> ProjectAllHistoryOrder = new List<string>() { 
            " T.CreateDate", " T.CreateDate DESC"
        };
        /*internal static IList<string> SharedProjectOrder = new List<string>() { 
            " PG.CreateDate", " PG.CreateDate DESC",
            " PR.ProjectName", " PR.ProjectName DESC"
        };*/
        internal static IList<string> AccountBillingOrder = new List<string>() { 
            " B.BillDate", " B.BillDate DESC",
            " B.BillStartDate", " B.BillStartDate DESC",
            " B.NetAmount", " B.NetAmount DESC",
        };
        internal static int GetSortIndexFolder(string sortBy, string sortOrder)
        {
            int sortIndex = 0;

            switch (sortBy)
            {
                case "OrdinalNumber": sortIndex = (sortOrder == "0") ? 7 : 6; break;
                case "Name": sortIndex = (sortOrder == "0") ? 2 : 3; break;
                case "RevisionDate": sortIndex = (sortOrder == "0") ? 7 : 6; break;
                case "Discipline": sortIndex = (sortOrder == "0") ? 7 : 6; break;
                case "CreateDate": sortIndex = (sortOrder == "0") ? 0 : 1; break;
                default: break;
            }

            return sortIndex;
        }
        internal static int GetSortIndexFile(string sortBy, string sortOrder)
        {
            int sortIndex = 0;

            switch (sortBy)
            {
                case "OrdinalNumber": sortIndex = (sortOrder == "0") ? 5 : 6; break;
                case "Name": sortIndex = (sortOrder == "0") ? 2 : 3; break;
                case "RevisionDate": sortIndex = (sortOrder == "0") ? 7 : 8; break;
                case "Discipline": sortIndex = (sortOrder == "0") ? 9 : 10; break;
                case "CreateDate": sortIndex = (sortOrder == "0") ? 0 : 1; break;
                default: break;
            }

            return sortIndex;
        }
        internal static IList<string> AppNoticeOrder = new List<string>() { "MN.NoticeStartDate", "MN.NoticeStartDate DESC", "MN.EventStartDate", "MN.EventStartDate DESC" };

        internal static int GetSortIndexAlbum(string sortBy, string sortOrder)
        {
            int sortIndex = 0;
            switch (sortBy)
            {
                case "CreateDate": sortIndex = (sortOrder == "0") ? 0 : 1; break;
                case "Name": sortIndex = (sortOrder == "0") ? 2 : 3; break;
                default: break;
            }
            return sortIndex;
        }

        internal static int GetSortIndexAlbumImage(string sortBy, string sortOrder)
        {
            int sortIndex = 0;
            switch (sortBy)
            {
                case "CreateDate": sortIndex = (sortOrder == "0") ? 0 : 1; break;
                case "Name": sortIndex = (sortOrder == "0") ? 2 : 3; break;
                default: break;
            }
            return sortIndex;
        }
        //any change in RFIOrder array will impact code in RFIManager inside "OrderFinalList" region 
        //please review after RFIOrder array change         
        internal static IList<string> RFIOrder = new List<string>() { "R.CreateDate ASC", "R.CreateDate DESC" };
        internal static IList<string> PunchOrder = new List<string>() { "PU.CreateDate ASC", "PU.CreateDate DESC", "PT.TitleDescription ASC", "PT.TitleDescription DESC", "PU.PunchDescription ASC", "PU.PunchDescription DESC", "PU.ModifyDate ASC", "PU.ModifyDate DESC" };
        internal static IList<string> SubmittalOrder = new List<string>() { "S.CreateDate ASC", "S.CreateDate DESC" };

        internal static IList<string> OutlookOrder = new List<string>() { 
            " CO.CreateDate ASC", " CO.CreateDate DESC", " CRL.ToRecipientEmails ASC", " CRL.ToRecipientEmails DESC", 
            " CO.MessageSubject ASC", " CO.MessageSubject DESC", " CO.ComposerEmail,SenderEmail"," CO.ComposerEmail DESC,SenderEmail DESC"
        };

        //F&A viewer
        public static IList<string> AnnotationOrder = new List<string>() { " MarkupTitle", " MarkupTitle DESC", " CreateDate", " CreateDate DESC" };
    }
}
