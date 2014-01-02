<%@ Page Language="C#" MasterPageFile="/umbraco/masterpages/umbracoPage.Master" AutoEventWireup="true" CodeBehind="permissions.aspx.cs" Inherits="Widget_Builder.PermissionsPage" %>
<%@ Register Namespace="umbraco.uicontrols" Assembly="controls" TagPrefix="umb"%>
<%@ Register TagPrefix="umb" Namespace="ClientDependency.Core.Controls" Assembly="ClientDependency.Core" %>

<asp:Content ID="Content" ContentPlaceHolderID="body" runat="server">
    
    <umb:UmbracoPanel ID="Panel1" runat="server" hasMenu="false" Text="Widget Builder Permissions">
        <umb:JsInclude ID="JsInclude1" runat="server" FilePath="/plugins/WidgetBuilder/permissions.js" PathNameAlias="UmbracoRoot" />
   
        <umb:CssInclude ID="CssInclude2" runat="server" FilePath="/plugins/WidgetBuilder/permissions.css" PathNameAlias="UmbracoRoot" />
        
        <div id="mainWrapper" runat="server">
    
        </div>
        
    </umb:UmbracoPanel>
</asp:Content>
