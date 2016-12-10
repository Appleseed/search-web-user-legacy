<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CollectionIndexSearch.ascx.cs" Inherits="Appleseed.Services.Search.Web.DesktopModules.CustomModules.Anant.Search.CollectionIndexSearch" %>

<link rel="stylesheet" href="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.4/css/bootstrap.min.css">

<style>
    .ui-autocomplete {
        max-height: 100px;
        overflow-y: auto;
        /* prevent horizontal scrollbar */
        overflow-x: hidden;
    }
    /* IE 6 doesn't support max-height
               * we use height instead, but this forces the menu to always be this tall
               */
    * html .ui-autocomplete {
        height: 100px;
    }

    /* "Current" page */
    .paging {
        margin: 10px;
    }

        .paging b {
            background-color: #465C71;
            color: white;
        }

        .paging b, .paging a {
            border: solid 1px #4E667D;
            border-radius: 3px;
            display: inline;
            float: none;
            font-size: 12px;
            margin-right: 2px;
            padding: 3px 6px;
        }

        .paging a {
            color: black;
            text-decoration: none;
        }

    .SearchResults {
    }

    .SearchResultsSummary {
        margin: 10px 0 20px;
    }

    .SearchResultItem {
        margin-bottom: 20px;
    }

    .SearchResultItemLink {
        font-size: larger;
        font-weight: bold;
    }

    .SearchResultItemPath {
    }

    .SearchResultItemSummary {
    }
</style>

<asp:UpdatePanel ID="searchResults" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
    </ContentTemplate>
</asp:UpdatePanel>

<table id="Table1" cellspacing="0" cellpadding="2" border="0">
    <tr>
        <td>Search&nbsp;
        </td>
        <td>
            <asp:HiddenField runat="server" ID="txtPageNumber" ClientIDMode="Static" />
            <asp:TextBox runat="server" ID="txtQuery" ClientIDMode="Static" />&nbsp;
                    <asp:Button runat="server" ID="btnSearch" ClientIDMode="Static" Text="Search" OnClick="btnSearch_Click" OnClientClick="ga_mp_calling();"/>
            <span style="display: none;">
                <asp:Button runat="server" ID="btnGoToPage" ClientIDMode="Static" OnClick="btnGoToPage_Click" /></span>
        <td>
            <asp:UpdateProgress ID="searchProgress" runat="server" AssociatedUpdatePanelID="searchResults">
                <ProgressTemplate>
                    <img id="Img1" src="~/ajax-loader.gif" runat="server" />
                </ProgressTemplate>
            </asp:UpdateProgress>
        </td>
    </tr>
</table>

<div class="SearchResults">
    <div class="SearchResultsSummary">
        <asp:Label ID="LabelSummary" runat="server" Text="<%# Summary %>"></asp:Label>
    </div>

    <div class="paging">
        <%=Paging %>
    </div>


    <asp:PlaceHolder ID="phFacets" runat="server" Visible="false">
        <div class="col-md-3">
            <table>
                <tr>
                    <td>Source:
            <asp:Repeater ID="rptSourceFacetContent" runat="server">
                <ItemTemplate>
                    <table>
                        <tr>
                            <td>
                                <asp:CheckBox ID="cbSourceFacet" runat="server" OnCheckedChanged="cbSourceFacet_changed" AutoPostBack="true" />
                            </td>
                            <td>
                                <asp:Label ID="lbSourceFacetContent" runat="server" Text='<%# Container.DataItem.ToString() %>'></asp:Label>
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
            </asp:Repeater>

                        <br />

                        Type:
            <asp:Repeater ID="rptTypeFacetContent" runat="server">
                <ItemTemplate>
                    <table>
                        <tr>
                            <td>
                                <asp:CheckBox ID="cbTypeFacet" runat="server" OnCheckedChanged="cbTypeFacet_changed" AutoPostBack="true" Checked="true"/>
                            </td>
                            <td>
                                <asp:Label ID="lbTypeFacetContent" runat="server" Text='<%# Container.DataItem.ToString() %>'></asp:Label>
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
            </asp:Repeater>


                        <br />


                        Smart Keywords:
            <asp:Repeater ID="rptKeywordsFacetContent" runat="server">
                <ItemTemplate>
                    <table>
                        <tr>
                            <td>
                                <asp:CheckBox ID="cbKeywordsFacet" runat="server" OnCheckedChanged="cbKeywordsFacet_changed" AutoPostBack="true" />
                            </td>
                            <td>
                                <asp:Label ID="lbKeywordsFacetContent" runat="server" Text='<%# Container.DataItem.ToString() %>'></asp:Label>
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
            </asp:Repeater>
                    </td>
                </tr>
            </table>
        </div>
    </asp:PlaceHolder>

    <div class="col-md-9">
        <asp:Repeater ID="Repeater1" runat="server" DataSource="<%# Results.Data %>">
            <ItemTemplate>
                <div class="SearchResultItem">
                    <div class="SearchResultItemLink">
                        <a href="<%# Eval("ItemPath") %>"><%# Eval("ItemName")  %></a>
                    </div>
                    <div class="SearchResultItemPath">
                        <%# Eval("ItemPath")%>
                    </div>
                    <div class="SearchResultItemSummary">
                        <%# Eval("ItemSummary")  %>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>



    <div class="paging">
        <%=Paging %>
    </div>

</div>

<asp:PlaceHolder ID="Placeholder2" runat="server">
    <script type="text/javascript">

        function hashChange() {
            var hash = location.hash.replace('#', '');
            var params = hash.split('&');
            var searchParams = {};
            for (var i = 0; i < params.length; i++) {
                var param = params[i].split('=');
                searchParams[param[0]] = param[1];
            }

            if (searchParams['p']) {
                $('#txtPageNumber').val(searchParams['p']);
            } else {
                $('#txtPageNumber').val('1');
            }

            $('#txtQuery').val(searchParams['q']);

            $('#btnGoToPage').click();
        }

        window.addEventListener("hashchange", hashChange, false);

        if (location.hash !== "") {
            hashChange();
        }

        function split(val) {
            return val.split(/,\s*/);
        }

        function extractLast(term) {
            return split(term).pop();
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
                                  return term.length >= 2;
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
                                  return false;
                              },
                              minLength: 2
                          });


        }

        InitializeAutoComplete();

        var prm = Sys.WebForms.PageRequestManager.getInstance();

        prm.add_endRequest(function () {
            // re-bind your jQuery events here
            InitializeAutoComplete();
        });

        $('#txtQuery').keypress(function (e) {
            if (e.which == 13) {

                ga_mp_calling();

                $('#txtPageNumber').val('1');
                $('#btnSearch').click();
                e.preventDefault();
            }
        });

        function ga_mp_calling() {
            try {
                trackSearch();
                trackSearchforGA();
            } catch (err) {
            }
        }
    </script>
</asp:PlaceHolder>
