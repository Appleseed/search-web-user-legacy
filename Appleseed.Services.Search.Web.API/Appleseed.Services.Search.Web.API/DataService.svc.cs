using System;
using System.Collections.Generic;
using Appleseed.Services.Base.Model;
using Appleseed.Services.Search.Web.API.Manager;

namespace Appleseed.Services.Search.Web.API
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "DataService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select DataService.svc or DataService.svc.cs at the Solution Explorer and start debugging.
    public class DataService : IDataService
    {
        public SearchResult GetCollectionByQuery(string indexType, string collectionName, string query, string pageSize, string pageIndex)
        {
            SolrDataServiceManager manager = new SolrDataServiceManager(collectionName);

            var searchResult = manager.GetSearchResults(new SearchRequest() { Query = query, PageNumber = Convert.ToInt32(pageIndex), RecordsPerPage = Convert.ToInt32(pageSize) });

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
