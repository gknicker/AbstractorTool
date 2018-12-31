<%@ Page Title="Test" Language="C#" MasterPageFile="~/penatna.master"
	AutoEventWireup="true" CodeFile="test.aspx.cs" Inherits="Penatna.TestPage" %>

<asp:Content ID="Content" ContentPlaceHolderID="ContentArea" runat="Server">
	<asp:Button ID="GetOrdersButton" OnClick="GetOrdersButton_Click" Text="GetOrders" runat="server" />
	<asp:GridView ID="OrderGrid" DataKeyNames="Id" AutoGenerateColumns="true" runat="server"></asp:GridView>
	<asp:Button ID="GetAbstractButton" OnClick="GetAbstractButton_Click" Text="GetAbstract" runat="server" />
	</script>
</asp:Content>
