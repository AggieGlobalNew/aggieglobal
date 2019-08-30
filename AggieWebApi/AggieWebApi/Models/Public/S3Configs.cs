using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AggieGlobal.WebApi.Infrastructure;
using Newtonsoft.Json;
using System.Xml.Serialization;
using AggieGlobal.WebApi.Infrastructure;

namespace AggieGlobal.WebApi.Models.Public
{
    public sealed class SharedAppSettings
    {
        public string AccountNeutralS3BucketName { get; set; }
        public KeyValuePair<string, string>? AWSAccessKeys { get; set; }
        public string FileChunkServiceEndpoint { get; set; }
        public string FileChunkServiceGetChunkInfoCmd { get; set; }
        public string PreSignedURLGenerationCmd { get; set; }
        public string AWSRegionEndpoint { get; set; }
        //public int UploadBufferSize { get; set; }
        //public int FileSystemTimeOutInMS { get; set; }
        //public int MaxDurationInMinuteAfterLastFileAccessForDelete { get; set; }
        //public int WaitDurationBeforeRetryUploadOrDownloadInMs { get; set; }
    }

    public sealed class AccountBucketMap
    {
        public string PinProjectIds { get; set; }
        public int PWAccountID { get; set; }
        public string BucketName { get; set; }
        public string RootPath { get; set; }
        public string CognitoIdentityPoolID { get; set; }
        public string CognitoRegionType { get; set; }
        public string DefaultServiceRegionType { get; set; }
        public string AWSAccessKey { get; set; }
        public string AWSSecretKey { get; set; }
        public string AWSRegionEndpoint { get; set; }
        public string NeutralBucketName { get; set; }
        public string NeutralBucketRootFolderName { get; set; }
        public string AWSNeutralBucketAccessKey { get; set; }
        public string AWSNeutralBucketAccessSecretKey { get; set; }
        public int? BucketType { get; set; }
        public AccountBucketMap ShallowCopy()
        {
            return (AccountBucketMap)this.MemberwiseClone();
        }

    }

    public class BucketInfoResponse
    {
        [JsonIgnore][XmlIgnore]
        public AccountBucketMapInfo DefaultBucketInfo { get; set; }
        [JsonIgnore][XmlIgnore]
        public List<AccountBucketMapInfo> PrivateBucketInfo { get; set; }
        
        public string EncryptPrivateBucketInfo
        {
            get
            {
                return PrivateBucketInfo != null ? EncryptionHelper.AesEncryption(JsonConvert.SerializeObject(PrivateBucketInfo), EncryptionKey.LOG) : string.Empty;
            }
        }
        
        public string EncryptDefaultBucketInfo
        {
            get
            {
                return DefaultBucketInfo != null ? EncryptionHelper.AesEncryption(JsonConvert.SerializeObject(DefaultBucketInfo), EncryptionKey.LOG) : string.Empty;
            }
        }
    }

    public sealed class AccountBucketMapInfo
    {
        public int PWAccountID { get; set; }
        [JsonIgnore][XmlIgnore]
        public int PINProjectID { get; set; }
        public string PinProjectIds { get; set; }
        public string BucketName { get; set; }
        public string RootPath { get; set; }
        public IDictionary<string, string> RootPaths { get; set; }
        public string CognitoIdentityPoolID { get; set; }
        public string CognitoRegionType { get; set; }
        public string DefaultServiceRegionType { get; set; }
        public string AWSAccessKey { get; set; }
        public string AWSSecretKey { get; set; }
        public int BucketType { get; set; }
    }

    public sealed class DefaultAccountBucketDetails
    {
        public int PWAccountID { get; set; }
        public string BucketName { get; set; }
        public string RootPath { get; set; }
        public string CognitoIdentityPoolID { get; set; }
        public string CognitoRegionType { get; set; }
        public string DefaultServiceRegionType { get; set; }
        public string AWSAccessKey { get; set; }
        public string AWSSecretKey { get; set; }
        public int BucketType { get; set; }
    }


    public enum S3LocatorType { S3PhotosLocator, S3RFILocator, S3PunchLocator, S3DocumentLocator };

    public class AccountBucketMapDetails
    {
        [JsonIgnore][XmlIgnore]
        public AccountBucketMap DefaultBucketInfo { get; set; }

        public string EncryptAccountBucketInfo
        {
            get
            {
                return EncryptionHelper.AesEncryption(JsonConvert.SerializeObject(DefaultBucketInfo), EncryptionKey.LOG);
            }
        }
    }
}