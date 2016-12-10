<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CollectionIndexKnockoutSearch.ascx.cs" Inherits="Appleseed.Services.Search.Web.DesktopModules.CustomModules.Anant.Search.CollectionIndexKnockoutSearch" %>
  <script language="javascript" type="text/javascript">
      function split(val) {
          return val.split(/,\s*/);
      }
      function extractLast(term) {
          return split(term).pop();
      }
   
        var SearchResultsVM = {
            SearchResults: ko.observable()
        };

        function InitializeSearchResultsViewModel() {
            SearchResultsVM.SearchResults = ko.observable([]);
            ko.applyBindings(SearchResultsVM);
        }

        function UpdateSearchResultsViewModel(newquery) {
            $.getJSON('/Search.Web.User.Legacy/API/SearchService.svc/GetCollectionByQuery?query=' + newquery + '&resultCount=10&start=0', function (data) {
                SearchResultsVM.SearchResults(data.d.Data);
            });    //getJSON
        }

      function InitializeAutoComplete() {
          $("#txtQuery")
                        .bind("keydown", function (event) {
                            if (event.keyCode === $.ui.keyCode.TAB && $(this).data("ui-autocomplete").menu.active) {
                                event.preventDefault();
                            }

                        }).autocomplete({
                            source: function (request, response) {
                                $.ajax({
                                    type: "GET",
                                    contentType: "application/json; charset=utf-8",
                                    url: "/Search.Web.User.Legacy/API/SearchService.svc/GetAllPredictions",
                                    data: "termStartsWith=" + extractLast(request.term),
                                    dataType: "json",
                                    async: true,
                                    success: function (data) {
                                        response(data.d);
                                    },
                                    error: function (result) {
                                        alert("Due to unexpected errors we were unable to load data");
                                    }
                                });
                            },

                            search: function () {
                                // custom minLength
                                var term = extractLast(this.value);
                                if (term.length < 2) {
                                    return false;
                                }

                                return true;
                            },
                            focus: function () {
                                // prevent value inserted on focus
                                return false;
                            },
                            select: function (event, ui) {
                                var terms = split(this.value);
                                // remove the current input
                                terms.pop();
                                // add the selected item
                                terms.push(ui.item.value);
                                // add placeholder to get the comma-and-space at the end
                                terms.push("");
                                this.value = terms.join(", ");

                                UpdateSearchResultsViewModel(this.value);

                                return false;
                            },
                            minLength: 2
                        });
          

      }

      $(document).ready(function() {
          InitializeAutoComplete();
          InitializeSearchResultsViewModel();

          $('#txtQuery').keypress(function(e) {
              if (e.which == 13) {
                  try {
                      trackSearch();
                      trackSearchforGA();
                  } catch (err) {
                  }
                  UpdateSearchResultsViewModel($('#txtQuery').val());
                  e.preventDefault();
              }
          });
      });
</script>

        <%-- 
        // use this to show on screen 
        <pre data-bind="text: ko.toJSON($data)"></pre>  
        
        // use this to show in console 
        <span data-bind="visible: console.log($data) "></span>
        
        --%>

Search <input type="text" id="txtQuery" />

<div data-bind="foreach: SearchResults">
<p>
    <a data-bind="attr: { href: ItemPath, title: ItemName }" class="link"><span class="name" data-bind="text: ItemName"></a><br/>
        
        <span class="sample" data-bind="html: ItemSummary"></span>
        <br>
		<span class="path" data-bind="text: ItemPath">								    
		</span>
        <span class="type" data-bind="text: ItemType">								    
		</span>
</p>
</div>
<%-- Table Version 
<table class="searchResults"  >    
    <thead><tr><th>Path</th><th>Name</th><th>Summary</th></tr></thead>
    <tbody data-bind="foreach: SearchResults ">
        <tr>
            <td data-bind="text: ItemPath"></td>
            <td data-bind="text: ItemName"></td>
            <td data-bind="html: ItemSummary"></td>
        </tr>     
    </tbody>
</table> --%>
