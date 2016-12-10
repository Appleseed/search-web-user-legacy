namespace Appleseed.Services.Search.Web.DesktopModules.CustomModules.Anant.Search
{
    using System;
    using System.Configuration;
    using System.Text;
    using System.Web;

    using System.Collections.Generic;
    using Appleseed.Services.Base.Engine.Services.Impl;
    using Appleseed.Services.Base.Model;
    using System.Web.UI.WebControls;

    public partial class CollectionIndexSearch : System.Web.UI.UserControl
    //public partial class CollectionIndexSearch : PortalModuleControl
    {
        private const int MaxPagesToDisplay = 10;

        //private string searchPage = string.Empty;

        public string ServicePathRoot
        {
            get { return this.ResolveUrl(ConfigurationManager.AppSettings["appleseed_services_search_predictionServiceRoot"]); }
        }

        /// <summary>
        /// Search Results.
        /// </summary>
        public SearchResult Results;

        /// <summary>
        /// First item on page (user format).
        /// </summary>
        private int fromItem;

        /// <summary>
        /// Last item on page (user format).
        /// </summary>
        private int toItem;

        /// <summary>
        /// Time it took to make the search.
        /// </summary>
        private TimeSpan duration;

        /// <summary>
        /// How many items can be showed on one page.
        /// </summary>
        private const int ResultsPerPage = 10;

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterHiddenField("__EVENTTARGET", "ButtonSearch");

            //if (this.searchPage == string.Empty)
            //{
            //    this.searchPage = Request.Url.AbsolutePath;
            //}

            if (!this.IsPostBack)
            {
                if (!string.IsNullOrEmpty(this.Query))
                {
                    this.PerformSearch();
                }
                else
                {
                    this.txtQuery.Text = string.Empty;
                }
            }
            else
            {
                this.Query = this.txtQuery.Text;
            }
        }

        private void PerformSearch()
        {
            var start = DateTime.Now;
            var filePath = @"C:\";
            var pathToIndex = ConfigurationManager.AppSettings["appleseed_services_search_defaultindex"];
            var luceneRepository = new LuceneIndexSearchService(pathToIndex, filePath);

            if (this.FacetFilters == null)
            {
                Dictionary<string, List<string>> filters = new Dictionary<string, List<string>>();
                filters.Add("ItemSource", new List<string>());
                filters.Add("ItemType", new List<string>());
                filters.Add("SmartItemKeywords", new List<string>());
                this.FacetFilters = filters;
                phFacets.Visible = true;
            }

            var request = new SearchRequest()
            {
                Query = this.Query,
                PageNumber = this.CurrentPage,
                RecordsPerPage = ResultsPerPage,
                FacetFilters = this.FacetFilters
            };
            this.Results = luceneRepository.GetSearchResults(request);
            // Clean up Results...
            foreach (var item in this.Results.Data)
            {
                if (string.IsNullOrWhiteSpace(item.ItemName))
                {
                    item.ItemName = !string.IsNullOrWhiteSpace(item.ItemPath) ? item.ItemPath : "Unknown Title";
                }
            }

            // result information
            this.duration = DateTime.Now - start;
            this.fromItem = ((this.CurrentPage - 1) * ResultsPerPage) + 1;
            this.toItem = Math.Min(((this.CurrentPage - 1) * ResultsPerPage) + ResultsPerPage, this.Results.TotalRecordsFound);

            if (!FilterStart)
            {
                rptTypeFacetContent.DataSource = this.Results.FacetContent["ItemType"];
                rptTypeFacetContent.DataBind();

                rptSourceFacetContent.DataSource = this.Results.FacetContent["ItemSource"];
                rptSourceFacetContent.DataBind();

                rptKeywordsFacetContent.DataSource = this.Results.FacetContent["SmartItemKeywords"];
                rptKeywordsFacetContent.DataBind();
            }

            this.DataBind();
        }

        /// <summary>
        /// Page links. DataTable might be overhead but there used to be more fields in previous version so I'm keeping it for now.
        /// </summary>
        protected string Paging
        {
            get
            {
                if (string.IsNullOrEmpty(this.Query))
                {
                    return string.Empty;
                }

                var pageStart = this.CurrentPage * ResultsPerPage;

                if (this.Results.TotalRecordsFound.Equals(0))
                {
                    return string.Empty;
                }

                if (this.Results.TotalRecordsFound <= ResultsPerPage)
                {
                    return "<b>1</b>";
                }

                var totalPages = (int)((this.Results.TotalRecordsFound + ResultsPerPage - 1) / ResultsPerPage);
                var currentPage = pageStart / ResultsPerPage;
                var firstPage = 1;
                var lastPage = Math.Min(totalPages, 10);

                if (currentPage > (MaxPagesToDisplay / 2))
                {
                    firstPage = currentPage - (MaxPagesToDisplay / 2);
                    lastPage = Math.Min(firstPage + MaxPagesToDisplay, totalPages);
                }

                return this.CreatePagingList(firstPage, lastPage, currentPage);
            }
        }

        private string CreatePagingList(int firstPage, int lastPage, int currentPage)
        {
            var paging = new StringBuilder();
            for (var i = firstPage; i <= lastPage; i++)
            {
                if (i.Equals(currentPage))
                {
                    paging.Append("<b>" + i + "</b>");
                }
                else
                {
                    ////var start = (i * this.ResultsPerPage) - this.ResultsPerPage;
                    //paging.AppendFormat("<a href=\"{0}?q={1}&p={2}\">{3}</a>", this.searchPage, this.Query, i, i);
                    paging.AppendFormat("<a href=\"#q={0}&p={1}\">{2}</a>", this.Query, i, i);
                }
            }

            return paging.ToString();
        }

        protected string Summary
        {
            get
            {
                if (this.Results.TotalRecordsFound > 0)
                {
                    return "Results <b>" + this.fromItem + " - " + this.toItem + "</b> of <b>" + this.Results.TotalRecordsFound + "</b> for <b>" + this.Query + "</b>. (" + this.duration.TotalSeconds + " seconds)";
                }

                return "No Results found";
            }
        }

        protected string Query { get; set; }

        /// <summary>
        /// Initializes startAt value. Checks for bad values.
        /// </summary>
        /// <returns></returns>
        private int CurrentPage { get; set; }

        public void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtQuery.Text))
            {
                return;
            }

            this.Query = Server.UrlDecode(this.txtQuery.Text).Replace(",", "").Trim();
            this.CurrentPage = 1;

            this.PerformSearch();
        }

        public void btnGoToPage_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtQuery.Text))
            {
                return;
            }

            this.Query = Server.UrlDecode(this.txtQuery.Text).Replace(",", "").Trim();
            int page;
            this.CurrentPage = int.TryParse(this.txtPageNumber.Value, out page) ? page : 1;

            Dictionary<string, List<string>> filters = new Dictionary<string, List<string>>();
            List<string> filtersSource = GetFilters("Source");
            List<string> filtersType = GetFilters("Type");
            List<string> filtersKeywords = GetFilters("Keywords");

            filters.Add("ItemSource", filtersSource);
            filters.Add("ItemType", filtersType);
            filters.Add("SmartItemKeywords", filtersKeywords);

            this.FacetFilters = filters;
            this.FilterStart = true;
            this.PerformSearch();

            rptSourceFacetContent.DataSource = this.FacetFilters["ItemSource"];
            rptSourceFacetContent.DataBind();
            if (this.FacetFilters["ItemSource"].Count != 0)
            {
                for (int i = 0; i < rptSourceFacetContent.Items.Count; i++)
                {
                    CheckBox chk = (CheckBox)rptSourceFacetContent.Items[i].FindControl("cbSourceFacet");
                    chk.Checked = true;
                }
            }



            rptTypeFacetContent.DataSource = this.FacetFilters["ItemType"];
            rptTypeFacetContent.DataBind();
            if (this.FacetFilters["ItemType"].Count != 0)
            {
                for (int i = 0; i < rptTypeFacetContent.Items.Count; i++)
                {
                    CheckBox chk = (CheckBox)rptTypeFacetContent.Items[i].FindControl("cbTypeFacet");
                    chk.Checked = true;
                }
            }

            rptKeywordsFacetContent.DataSource = this.FacetFilters["SmartItemKeywords"];
            rptKeywordsFacetContent.DataBind();
            if (this.FacetFilters["SmartItemKeywords"].Count != 0)
            {
                for (int i = 0; i < rptKeywordsFacetContent.Items.Count; i++)
                {
                    CheckBox chk = (CheckBox)rptKeywordsFacetContent.Items[i].FindControl("cbKeywordsFacet");
                    chk.Checked = true;
                }
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////


        public Dictionary<string, List<string>> FacetFilters { get; set; }

        public bool FilterStart { get; set; }

        protected void cbSourceFacet_changed(object sender, EventArgs e)
        {
            string queryContent = this.Query;
            if (!String.IsNullOrEmpty(queryContent))
            {
                Dictionary<string, List<string>> filters = new Dictionary<string, List<string>>();
                List<string> filtersSource = GetFilters("Source");
                List<string> filtersType = GetFilters("Type");
                List<string> filtersKeywords = GetFilters("Keywords");

                filters.Add("ItemSource", filtersSource);
                filters.Add("ItemType", filtersType);
                filters.Add("SmartItemKeywords", filtersKeywords);

                this.CurrentPage = 1;
                this.FacetFilters = filters;
                this.FilterStart = true;
                PerformSearch();

                rptSourceFacetContent.DataSource = this.FacetFilters["ItemSource"];
                rptSourceFacetContent.DataBind();
                for (int i = 0; i < rptSourceFacetContent.Items.Count; i++)
                {
                    CheckBox chk = (CheckBox)rptSourceFacetContent.Items[i].FindControl("cbSourceFacet");
                    chk.Checked = true;
                }


                rptTypeFacetContent.DataSource = this.Results.FacetContent["ItemType"];
                rptTypeFacetContent.DataBind();

                rptKeywordsFacetContent.DataSource = this.Results.FacetContent["SmartItemKeywords"];
                rptKeywordsFacetContent.DataBind();
            }
        }

        protected void cbTypeFacet_changed(object sender, EventArgs e)
        {
            string queryContent = this.Query;
            if (!String.IsNullOrEmpty(queryContent))
            {
                Dictionary<string, List<string>> filters = new Dictionary<string, List<string>>();
                List<string> filtersSource = GetFilters("Source");
                List<string> filtersType = GetFilters("Type");
                List<string> filtersKeywords = GetFilters("Keywords");

                filters.Add("ItemSource", filtersSource);
                filters.Add("ItemType", filtersType);
                filters.Add("SmartItemKeywords", filtersKeywords);

                this.CurrentPage = 1;
                this.FacetFilters = filters;
                this.FilterStart = true;
                PerformSearch();

                rptSourceFacetContent.DataSource = this.Results.FacetContent["ItemSource"];
                rptSourceFacetContent.DataBind();

                rptTypeFacetContent.DataSource = this.FacetFilters["ItemType"];
                rptTypeFacetContent.DataBind();

                rptKeywordsFacetContent.DataSource = this.Results.FacetContent["SmartItemKeywords"];
                rptKeywordsFacetContent.DataBind();
            }
        }

        protected void cbKeywordsFacet_changed(object sender, EventArgs e)
        {
            string queryContent = this.Query;
            if (!String.IsNullOrEmpty(queryContent))
            {
                Dictionary<string, List<string>> filters = new Dictionary<string, List<string>>();
                List<string> filtersSource = GetFilters("Source");
                List<string> filtersType = GetFilters("Type");
                List<string> filtersKeywords = GetFilters("Keywords");

                filters.Add("ItemSource", filtersSource);
                filters.Add("ItemType", filtersType);
                filters.Add("SmartItemKeywords", filtersKeywords);

                this.CurrentPage = 1;
                this.FacetFilters = filters;
                this.FilterStart = true;
                PerformSearch();

                rptSourceFacetContent.DataSource = this.Results.FacetContent["ItemSource"];
                rptSourceFacetContent.DataBind();

                rptTypeFacetContent.DataSource = this.Results.FacetContent["ItemType"];
                rptTypeFacetContent.DataBind();

                rptKeywordsFacetContent.DataSource = this.FacetFilters["SmartItemKeywords"];
                rptKeywordsFacetContent.DataBind();
                for (int i = 0; i < rptKeywordsFacetContent.Items.Count; i++)
                {
                    CheckBox chk = (CheckBox)rptKeywordsFacetContent.Items[i].FindControl("cbKeywordsFacet");
                    chk.Checked = true;
                }
            }
        }

        public List<string> GetFilters(string type)
        {
            List<string> filters = new List<string>();
            if (type == "Source")
            {
                for (int i = 0; i < rptSourceFacetContent.Items.Count; i++)
                {
                    CheckBox chk = (CheckBox)rptSourceFacetContent.Items[i].FindControl("cbSourceFacet");
                    if (chk.Checked)
                    {
                        filters.Add(((Label)rptSourceFacetContent.Items[i].FindControl("lbSourceFacetContent")).Text);
                    }

                }
            }
            else if (type == "Keywords")
            {
                for (int i = 0; i < rptKeywordsFacetContent.Items.Count; i++)
                {
                    CheckBox chk = (CheckBox)rptKeywordsFacetContent.Items[i].FindControl("cbKeywordsFacet");
                    if (chk.Checked)
                    {
                        filters.Add(((Label)rptKeywordsFacetContent.Items[i].FindControl("lbKeywordsFacetContent")).Text);
                    }

                }
            }
            else if (type == "Type")
            {
                for (int i = 0; i < rptTypeFacetContent.Items.Count; i++)
                {
                    CheckBox chk = (CheckBox)rptTypeFacetContent.Items[i].FindControl("cbTypeFacet");
                    if (chk.Checked)
                    {
                        filters.Add(((Label)rptTypeFacetContent.Items[i].FindControl("lbTypeFacetContent")).Text);
                    }

                }
            }
            return filters;
        }
    }
}