using System.Collections.Generic;
using SolrNet.Attributes;

namespace Appleseed.Services.Search.Web.API.Manager
{
    public class Document
    {
        [SolrUniqueKey("id")]
        public string Id { get; set; }

        [SolrField("path")]
        public ICollection<string> Path { get; set; }

        [SolrField("portalID")]
        public ICollection<string> PortalID { get; set; }

        [SolrField("content")]
        public ICollection<string> Content { get; set; }

        [SolrField("type")]
        public ICollection<string> Type { get; set; }


        [SolrField("name")]
        public ICollection<string> Name { get; set; }


        [SolrField("viewRole")]
        public ICollection<string> ViewRole { get; set; }


        [SolrField("createdDate")]
        public ICollection<string> CreatedDate { get; set; }


    }
}