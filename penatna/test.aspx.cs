using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Penatna
{
	public partial class TestPage : System.Web.UI.Page
	{
		public IList<Order> Orders { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			//Orders = new List<Order>();
		}

		protected void GetOrdersButton_Click(object sender, EventArgs e)
		{
			string debugHtml;
			Orders = CoreLogic.GetOrders("ewexler", "40mil", out debugHtml);
			int count = PenatnaDB.Instance.InsertOrders(Orders);
			Response.Write("Inserted " + count + " orders into the database.");
			if (!String.IsNullOrWhiteSpace(debugHtml)) {
				debugHtml = Server.HtmlEncode(debugHtml);
				debugHtml = debugHtml.Replace("\r\n", "<br />");
				Response.Write(debugHtml);
			}
			OrderGrid.DataSource = Orders;
			OrderGrid.DataBind();
		}

		protected void GetAbstractButton_Click(object sender, EventArgs e)
		{
			CoreLogic.GetAbstract("ewexler", "40mil", "28358840", @"C:\Users\Jason\Projects\AbstractorTool\penatna\doc\testract.pdf");
		}
	}
}
