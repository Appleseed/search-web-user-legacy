using System.Collections.Generic;
using Appleseed.Services.Base.Model;
using Appleseed.Services.Search.Web.API.Manager;

namespace Appleseed.Services.Search.Web.API
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ProxyService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ProxyService.svc or ProxyService.svc.cs at the Solution Explorer and start debugging.
    public class ProxyService : IProxyService
    {
        public SearchResult GetCollectionByQuery(string indexType, string collectionName, string query)
        {
            SolrDataServiceManager manager = new SolrDataServiceManager(collectionName);

            var searchResult = manager.GetSearchResultsForProxy(query);

            return searchResult;
        }

        public List<string> GetPredictions(string indexType, string collectionName, string termStartsWith)
        {
            var manager = new SolrDataServiceManager(collectionName);

            var items = manager.GetSearchPredictions(new SearchRequest() { Query = termStartsWith });

            return (List<string>)items;
        }
    }
}
