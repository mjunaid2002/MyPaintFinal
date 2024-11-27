<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CityReport.aspx.cs" Inherits="WebApplication2.Report.CityReport" %>
<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
   <script src='<%=ResolveUrl("~/crystalreportviewers13/js/crviewer/crv.js")%>' type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server" style="height:100%; width:100%;">
    <center>
        <CR:CrystalReportViewer ID="CityReportViewer" runat="server" AutoDataBind="true"></CR:CrystalReportViewer>
        <%--<CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" AutoDataBind="true" style="height:100%; width:100%;"  />--%>
   </center>
        </form>
</body>
</html>

