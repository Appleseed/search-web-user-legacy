using System.Runtime.Serialization;

namespace Appleseed.Services.Search.Web.API
{
    /// <summary>
    /// Summary description for ResponseCollectionItem
    /// </summary>
    [DataContract]
    public class ResponseCollectionItem
    {
        [DataMember]
        public string ItemPath { get; set; }

        [DataMember]
        public string ItemKey { get; set; }

        [DataMember]
        public string FileSize { get; set; }

        [DataMember]
        public string ItemName { get; set; }
    
        [DataMember]
        public string ItemType { get; set; }
    
        [DataMember]
        public string ItemSummary { get; set; }
    }
}