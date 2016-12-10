using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using Appleseed.Services.Base.Model;
using Appleseed.Services.Search.Web.API.Manager;

namespace Appleseed.Services.Search.Web.API
{

    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SolrSearchService
    {

        /*[OperationContract]
        [WebGet]
        public IEnumerable<string> GetSearchPredictions(SearchRequest request)
        {
            var manager = new SolrServerManager();

            var items = manager.GetPredictions(request);

            return items;
        }

        [OperationContract]
        [WebGet]
        public SearchResult GetSearchResults(SearchRequest request)
        {
            SolrServerManager manager = new SolrServerManager();

            var items = manager.GetCollectionByQuery(request);

            return items;
        }*/
        
        
        
        
        
        
        [OperationContract]
        [WebGet]
        public SearchResult GetCollectionByQuery(string queryText, int pageSize, int pageIndex)
        {

            SolrDataServiceManager manager = new SolrDataServiceManager();

            var searchResult = manager.GetSearchResults( new SearchRequest(){Query = queryText, PageNumber = pageIndex,RecordsPerPage = pageSize});

            return searchResult;
        }


        [OperationContract]
        [WebGet]
        public List<string> GetPredictions(string partialString)
        {
            var manager = new SolrDataServiceManager();

            var items = manager.GetSearchPredictions(new SearchRequest(){Query = partialString});

            return (List<string>) items;
        }


        
    }
}
