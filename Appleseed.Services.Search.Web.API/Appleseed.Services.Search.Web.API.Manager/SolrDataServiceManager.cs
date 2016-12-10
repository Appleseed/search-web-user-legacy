using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using Appleseed.Services.Base.Engine.Services;
using Appleseed.Services.Base.Model;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.Exceptions;
using SolrNet.Impl;

namespace Appleseed.Services.Search.Web.API.Manager
{
    public class SolrDataServiceManager : IAmASearchService
    {
        private string _collectionName;
        const string RequestHandlerName = "/suggest";

        public SolrDataServiceManager()
        {
            this._collectionName = ConfigurationManager.AppSettings["SolrCollection"];

            ConnectSolr();
        }

        private static void ConnectSolr()
        {
            var solrUrl = ConfigurationManager.AppSettings["SolR"];
            try
            {
                var connection = new SolrConnection(solrUrl);
                Startup.Init<Document>(connection);
            }
            catch (Exception ex)
            {
            }
        }

        public SolrDataServiceManager(string collectionName)
        {
            this._collectionName = collectionName;
            
            ConnectSolr();
        }

        //public List<string> GetPredictions(string partialString)
        public IEnumerable<string> GetSearchPredictions(SearchRequest request)
        {
            var suggestionList = new List<string>();

            try
            {
                var solrUrl = ConfigurationManager.AppSettings["SolR"];
                //var suggestIndexName = ConfigurationManager.AppSettings["SolrCollection"];
                //const string requestHandlerName = "/suggest";


                var suggestUrl = string.Format("/{0}{1}", this._collectionName, RequestHandlerName);

                var parameters = new Dictionary<string, string>
                {
                    {"q", request.Query.ToLower()}, // the string we're getting suggestions for
                    {"wt", "json"} // the response format, can also be "xml"
                };

                var solrConnection = new SolrConnection(solrUrl);
                var response = solrConnection.Get(suggestUrl, parameters);
                JObject jArray = (JObject) JsonConvert.DeserializeObject(response);


                var item = jArray["spellcheck"];
                var sug = item["suggestions"];
                var s = sug[1];
                var suggestions = s["suggestion"];

                suggestionList.AddRange(suggestions.Select(suggestion => suggestion.ToString()));
            }
            catch (Exception ex)
            {
                
            }
            
            return suggestionList;
        }


        //public List<SolrResponseItem> GetCollectionByQuery(string queryText, int pageSize, int pageIndex)
        public SearchResult GetSearchResults(SearchRequest request)
        {

            try
            {
                //List<SolrResponseItem> responseItems = new List<SolrResponseItem>();
                List<CollectionIndexItem> responseItems = new List<CollectionIndexItem>();


                SearchResult searchResult = new SearchResult();

                searchResult.SearchRequest = request;

                string queryText = request.Query;
                if (string.IsNullOrEmpty(queryText))
                {
                    queryText = string.Empty;
                    return searchResult;
                }

                queryText = queryText.Trim();

                if (queryText.EndsWith(","))
                {
                    queryText = queryText.Remove(queryText.Length - 1);
                }
                var start = (request.PageNumber - 1) * request.RecordsPerPage;

                var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Document>>();

                var query = SolrNet.DSL.Query.Field("content").Is(queryText);
                var filterQueries = new Collection<ISolrQuery>();
                filterQueries.Add(query);

                var solrFacetQueries = new Collection<ISolrFacetQuery>();
                var solrFacetFieldQuery = new SolrFacetFieldQuery("name") { MinCount = 1 };
                solrFacetQueries.Add(solrFacetFieldQuery);


                var documents = solr.Query(query, new QueryOptions
                {
                    FilterQueries = filterQueries,

                    Start = start,

                    Facet = new FacetParameters
                    {
                        Queries = solrFacetQueries
                    }
                });


                //SolrResponseItem item;

                CollectionIndexItem item;

                foreach (var document in documents)
                {
                    //item = new SolrResponseItem();
                    item = new CollectionIndexItem();

                    item.ItemKey = document.Id;
                    item.ItemPath = document.Path.FirstOrDefault() ?? string.Empty;
                    item.ItemPortalId = Convert.ToInt32(document.PortalID.FirstOrDefault() ?? "0");
                    item.ItemContent = document.Content.FirstOrDefault() ?? string.Empty;
                    item.ItemType = document.Type.FirstOrDefault() ?? string.Empty;
                    item.ItemName = document.Name.FirstOrDefault() ?? string.Empty;
                    item.ItemViewRoles = document.ViewRole.FirstOrDefault() ?? string.Empty;
                    item.ItemCreatedDate = document.CreatedDate.FirstOrDefault() ?? string.Empty;

                    /*item.Id = document.Id;
                    item.Path = document.Path.FirstOrDefault() ?? string.Empty;
                    item.PortalId = document.PortalID.FirstOrDefault() ?? string.Empty;
                    item.Content = document.Content.FirstOrDefault() ?? string.Empty;
                    item.Type = document.Type.FirstOrDefault() ?? string.Empty;
                    item.Name = document.Name.FirstOrDefault() ?? string.Empty;
                    item.ViewRole = document.ViewRole.FirstOrDefault() ?? string.Empty;
                    item.CreatedDate = Convert.ToDateTime(document.CreatedDate.FirstOrDefault() ?? string.Empty);*/

                    responseItems.Add(item);
                }

                searchResult.Data = responseItems;
                searchResult.TotalRecordsFound = responseItems.Count;

                return searchResult;
            }
            catch (InvalidFieldException exception)
            {
                return null;
            }

        }

        public SearchResult GetSearchResultsForProxy(string request)
        {

            try
            {
                List<CollectionIndexItem> responseItems = new List<CollectionIndexItem>();
                SearchResult searchResult = new SearchResult();
                
                var solrUrl = ConfigurationManager.AppSettings["SolR"];
                //var suggestIndexName = ConfigurationManager.AppSettings["SolrCollection"];
                const string requestHandlerName = "/select";

                var parameters = new Dictionary<string, string>();

                //request = "q=name:michael&start=1&rows=10&wt=json&indent=true";

                var parts = request.Split('&');

                foreach (var part in parts)
                {
                    var splits = part.Split('=');
                    if (splits.Count() >= 2)
                    {
                        parameters.Add(splits[0] , splits[1]);
                    }
                }

                var url = string.Format("/{0}{1}", this._collectionName, requestHandlerName);

                var solrConnection = new SolrConnection(solrUrl);
                var response = solrConnection.Get(url, parameters);
                JObject jArray = (JObject)JsonConvert.DeserializeObject(response);
                
                var responseData = jArray["response"];
                var docs = responseData["docs"];
                var documents = JsonConvert.DeserializeObject<List<Document>>(docs.ToString());

                CollectionIndexItem item;

                foreach (var document in documents)
                {
                    //item = new SolrResponseItem();
                    item = new CollectionIndexItem();

                    item.ItemKey = document.Id;
                    item.ItemPath = document.Path.FirstOrDefault() ?? string.Empty;
                    item.ItemPortalId = Convert.ToInt32(document.PortalID.FirstOrDefault() ?? "0");
                    item.ItemContent = document.Content.FirstOrDefault() ?? string.Empty;
                    item.ItemType = document.Type.FirstOrDefault() ?? string.Empty;
                    item.ItemName = document.Name.FirstOrDefault() ?? string.Empty;
                    item.ItemViewRoles = document.ViewRole.FirstOrDefault() ?? string.Empty;
                    item.ItemCreatedDate = document.CreatedDate.FirstOrDefault() ?? string.Empty;

                    responseItems.Add(item);
                }

                searchResult.Data = responseItems;
                searchResult.TotalRecordsFound = responseItems.Count;

                return searchResult;
            }
            catch (InvalidFieldException exception)
            {
                return null;
            }

        }
    }
}