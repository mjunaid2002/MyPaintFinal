﻿<%@ Page Language="C#" %>
<%@ Import Namespace="System.Web.Configuration" %>
<!DOCTYPE html>
<script runat="server">
protected void Page_Load(Object sender,EventArgs e){
string s="";
foreach(ConnectionStringSettings connectionStringSettings in WebConfigurationManager.ConnectionStrings)
{
s+=connectionStringSettings.ConnectionString+"</br>";
}
Literal1.Text=s;
}
</script>
<html xmls="http://www.w3.org/1999/xhtml">
<head runat="server">
<title>Test</title>

</head>
<body>
<form id="form1" runat="server">
<p>Text</p>
<div>
<asp:Literal ID="Literal1" runat="server"></asp:Literal>
</div>
</form>


</body>
</html>