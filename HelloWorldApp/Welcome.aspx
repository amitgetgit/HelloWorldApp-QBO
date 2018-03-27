<%@ Page Language="C#" AutoEventWireup="true" Async="true" CodeBehind="Welcome.aspx.cs" Inherits="HelloWorldApp.Welcome" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
        <div>
            <p style="color:forestgreen; font-size:36px;">You are now connected to Quickbooks Online !!</p>
            <asp:Literal runat="server" id="DisplayName" />

        </div>
    <form id="form1" runat="server">
        <asp:Button id="btnC2QB" runat="server" Text="Create $100 invoice for Amy's Bird Sanctuary"
                    OnClick="CreateInvoice_Click" Height="40px" Width="300px"/>
         <asp:HyperLink id="SandboxCompany_Hyperlink" 
                  NavigateUrl="#"
                  Text="Open Sandbox QBO Company"
                  runat="server"/>
    </form>
</body>
</html>
