using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using Appleseed.Services.Base.Model;

namespace Appleseed.Services.Search.Web.API
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IProxyService" in both code and config file together.
    [ServiceContract]
    public interface IProxyService
    {
        [OperationContract]
        [WebGet(UriTemplate = "{indexType}/{collectionName}/Select/{query}", ResponseFormat = WebMessageFormat.Json)]
        SearchResult GetCollectionByQuery(string indexType, string collectionName, string query);


        [OperationContract]
        [WebGet(UriTemplate = "{indexType}/{collectionName}/Prediction/{termStartsWith}", ResponseFormat = WebMessageFormat.Json)]
        List<string> GetPredictions(string indexType, string collectionName, string termStartsWith);

    }
}
