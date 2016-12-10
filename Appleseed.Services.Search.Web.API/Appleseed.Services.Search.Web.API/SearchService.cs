using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text.RegularExpressions;
using System.Web;
using Appleseed.Services.Base.Engine.Services.Impl;
using Appleseed.Services.Base.Model;

namespace Appleseed.Services.Search.Web.API
{
    [ServiceContract(Namespace = "AppleseedServices")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SearchService
    {
        [OperationContract]
        [WebGet]
        public IList<string> GetAllPredictions(string termStartsWith)
        {
            if (string.IsNullOrEmpty(termStartsWith))
            {
                return null;
            }

            var query = HttpContext.Current.Server.UrlDecode(termStartsWith);

            // Use the repository to get the list of terms. 
            var luceneRepository = new LuceneIndexSearchService(this.LuceneIndexPath, "");

            var names = luceneRepository.GetSearchPredictions(new SearchRequest() { Query = query });


            var result = (from c in names
                where c.StartsWith(query)
                select c).ToList();

            return result;
        }

        [OperationContract]
        [WebGet]
        public Appleseed.Services.Base.Model.SearchResult GetCollectionByQuery(string query, int resultCount, int start, string filters)
        {
            int pageNumber = 1;
            if (start > resultCount)
            {
                pageNumber = start / resultCount;
            }
        
            query = query.Trim();
            if (query.EndsWith(","))
            {
                query = query.Remove(query.Length - 1);
            }


            var filePath = @"C:\";
            var luceneRepository = new LuceneIndexSearchService(this.LuceneIndexPath, filePath);

            // a filter has format like: &filters={f1=a,b,c},{f2=e,d} or &filters={f1=}
            Dictionary<string, List<string>> facetFilters = new Dictionary<string, List<string>>();
            Regex filterReg = new Regex(@"\{(?<filterField>[a-zA-Z0-9_]+):(?<filterContent>[a-zA-Z0-9_,]*)\}");
            var matches = filterReg.Matches(filters);
            if (matches.Count != 0)
            {
                foreach (Match match in matches)
                {
                    string key = match.Groups["filterField"].Value;
                    string values = match.Groups["filterContent"].Value;
                    List<string> valueArray = values.Split(',').ToList();
                    facetFilters.Add(key, valueArray);
                }
            }


            var request = new SearchRequest() { Query = query, PageNumber = pageNumber, RecordsPerPage = resultCount, FacetFilters = facetFilters };
            var results = luceneRepository.GetSearchResults(request);

            var output = (from c in results.Data
                select new
                    CollectionIndexItem
                {
                    ItemPath = c.ItemPath,
                    ItemName = c.ItemName,
                    ItemSummary = c.ItemSummary,
                    ItemType = c.ItemType,
                    ItemFileSize = c.ItemFileSize
                }).ToList();
            return new Appleseed.Services.Base.Model.SearchResult() { Data = output, TotalRecordsFound = results.TotalRecordsFound, FacetContent = results.FacetContent };
        }

        private string LuceneIndexPath
        {
            get
            {
                return ConfigurationManager.AppSettings["appleseed_services_search_defaultindex"];
            }
        }


    }
}
