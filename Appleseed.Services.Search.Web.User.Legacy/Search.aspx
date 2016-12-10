<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="Search.Web.Search" %>
<%@ Register src="DesktopModules/CustomModules/Anant/Search/CollectionIndexSearch.ascx" tagname="CollectionIndexSearch" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:CollectionIndexSearch ID="CollectionIndexSearch1" runat="server" />
</asp:Content>
