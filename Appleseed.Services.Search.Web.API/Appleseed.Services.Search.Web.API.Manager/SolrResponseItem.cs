using System;
using System.Runtime.Serialization;

namespace Appleseed.Services.Search.Web.API.Manager
{
    [DataContract]
    public class SolrResponseItem
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public string PortalId { get; set; }

        [DataMember]
        public string Content { get; set; }

        [DataMember]
        public string Type { get; set; }


        [DataMember]
        public string Name { get; set; }


        [DataMember]
        public string ViewRole { get; set; }


        [DataMember]
        public DateTime CreatedDate { get; set; }
    }
}