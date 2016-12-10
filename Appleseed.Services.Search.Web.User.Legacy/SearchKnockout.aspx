<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SearchKnockout.aspx.cs" Inherits="Appleseed.Services.Search.Web.SearchKnockout" %>
<%@ Register src="DesktopModules/CustomModules/Anant/Search/CollectionIndexKnockoutSearch.ascx" tagname="CollectionIndexKnockoutSearch" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:CollectionIndexKnockoutSearch ID="CollectionIndexKnockoutSearch1" 
        runat="server" />
</asp:Content>
