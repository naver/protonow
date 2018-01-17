using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace Naver.Compass.Module.PreviewModule
{
    public class ConstData
    {
        public static  string DefaultProjectName = "Untitled";
        public static  string DefaultStartName = "index.html";
        public static string PrivateDomain = "";
        public static string PublishDomain = "";
        public static string ConfigPath = "\\Subversion";

       // public static string uploadURL = "http://10.101.60.159:8080/v1/file/";

        /// <summary>
        /// upload parameter
        /// </summary>
        public static string sRemoteID = "RemoteID";
        public static string sProjectPassword = "ProjectPassword";
         public static string uploadURL = "";

        public static string uploadReferer = "naver.com";
        public static string uploadProtectName = "X-Protection";
        public static string uploadProtectValue = "protonow";
        public static string uploadVersionName = "Version";
    }

    [DataContract]
    class ResponseData
    {
        [DataMember(Order = 0)]
        public string id { get; set; }
        [DataMember(Order = 1)]
        public string shortUrl { get; set; }

        public bool CheckData()
        {
            if (String.IsNullOrEmpty(id) || String.IsNullOrEmpty(shortUrl))
            {
                return false;
            }

            return true;
        }
    }

    [DataContract]
    class GetFileResponseData
    {
        [DataMember]
        public string createdAt { get; set; }
        [DataMember]
        public bool is_public { get; set; }
        [DataMember]
        public string shortUrl { get; set; }
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public bool is_exist { get; set; }
        [DataMember]
        public string updatedAt { get; set; }
    }
}
