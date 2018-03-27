<%@ Page Language="C#" Async="true" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HelloWorldApp.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Hello World App</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>

        <h1>Welcome to the Connect to QuickBooks Hellow World App!</h1>
        </div>
         <div style="height:200px;">
            <!-- Space for styling only-->           
        </div>
        <div id="connect" runat="server" >
    
        <!-- Connect To QuickBooks Button -->
        <p style="color:forestgreen; font-size:20px;">Now connect your app to QuickBooks by clicking on this button </p>
        <asp:ImageButton id="btnC2QB" runat="server" AlternateText="Connect to Quickbooks"
                ImageAlign="left"
                ImageUrl="Images/C2QB_white_btn_lg_default.png"
                OnClick="ImgC2QB_Click" Height="40px" Width="200px"/>
            <br /><br /><br />
        </div>
    </form>
</body>
</html>