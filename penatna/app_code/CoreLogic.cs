using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using HtmlAgilityPack;

namespace Penatna
{
	public class CoreLogic
	{
		const string domain = "www.docsolutions.corelogic.com";
		const string baseUrl = "https://" + domain + "/";

		public static IList<Order> GetOrders(string usr, string pwd, out string debugHtml)
		{
			IList<Order> orders = new List<Order>();

			debugHtml = "";

			CookieContainer cookies;
			if (!LoginUser(usr, pwd, out cookies)) {
				return orders;
			}

			StringBuilder url = new StringBuilder(baseUrl);
			url.Append("index.aspx?Screen=ReportDetail");
			url.Append("&ReportName=ABSTRACT-CONTRACTOR");
			url.Append("&Parameters=").Append(usr).Append("$D");
			HttpWebRequest request = WebRequest.Create(url.ToString()) as HttpWebRequest;
			request.CookieContainer = cookies;
			HttpWebResponse response = request.GetResponse() as HttpWebResponse;

			string html;
			using (StreamReader sr = new StreamReader(response.GetResponseStream())) {
				html = sr.ReadToEnd();
			}
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(html);
			HtmlNode orderTable = doc.DocumentNode.FindNode("TABLE", "CLASS", "Report");
			if (orderTable == null) {
				return orders;
			}
			// pull order details from each table row (skipping header)
			bool headings = true;
			foreach (HtmlNode childNode in orderTable.ChildNodes) {
				if (String.Equals(childNode.Name, "TR", StringComparison.OrdinalIgnoreCase)) {
					if (headings) {
						headings = false;
						continue;
					}
					HtmlNode continueButton = childNode.FindNode("INPUT", "CLASS", "PushButton");
					string detailLink = continueButton.Attributes["onclick"].Value;
					int pageIndex = detailLink.IndexOf("index.aspx");
					string detailUrl = detailLink.Substring(pageIndex, detailLink.Length - pageIndex - 1);
					detailUrl = baseUrl + detailUrl.Replace("&amp;", "&");
					request = WebRequest.Create(detailUrl) as HttpWebRequest;
					request.CookieContainer = cookies;
					response = request.GetResponse() as HttpWebResponse;
					HtmlDocument detailDoc = new HtmlDocument();
					using (StreamReader sr = new StreamReader(response.GetResponseStream())) {
						html = sr.ReadToEnd();
					}
					//debugHtml = html;
					//break;
					detailDoc.LoadHtml(html);
					Order order = new Order();
					HtmlNode node = detailDoc.DocumentNode.FindNode("INPUT", "NAME", "CaseNbr");
					order.CaseNumber = Convert.ToInt32(node.Attributes["VALUE"].Value);
					node = detailDoc.DocumentNode.FindNode("INPUT", "NAME", "TaskId");
					order.TaskID = Convert.ToInt32(node.Attributes["VALUE"].Value);
					order.Name = GetOrderDetailValue(detailDoc, "Name");
					order.Address = GetOrderDetailValue(detailDoc, "Address");
					order.County = GetOrderDetailValue(detailDoc, "County");
					order.State = GetOrderDetailValue(detailDoc, "State");
					try { order.InstrumentDate = Convert.ToDateTime(GetOrderDetailValue(detailDoc, "Instrument Date")).Date;
					} catch (FormatException) { order.InstrumentDate = null; }
					try { order.RecordingDate = Convert.ToDateTime(GetOrderDetailValue(detailDoc, "Recording Date")).Date;
					} catch (FormatException) { order.RecordingDate = null; }
					order.MortgageAmount = Convert.ToDecimal((GetOrderDetailValue(detailDoc, "Mortgage Amount")).Substring(1));
					order.DateAssigned = Convert.ToDateTime(GetOrderDetailValue(detailDoc, "Received")).Date;
					orders.Add(order);
				}
			}
			return orders;
		}

		public static void GetAbstract(string usr, string pwd, string caseNum, string filePath)
		{
			// log in and get specific login cookies
			CookieContainer cookies;
			if (!LoginUser(usr, pwd, out cookies)) {
				throw new ApplicationException("Penatna was unable to log in to CoreLogic.");
			}

			// get response from request to exact url of the order's abstract button
			StringBuilder url = new StringBuilder(baseUrl);
			url.Append("index.aspx?Screen=CaseAbstractorLetter");
			url.Append("&CaseNbr=").Append(caseNum);
			url.Append("&Format=4");
			HttpWebRequest request = WebRequest.Create(url.ToString()) as HttpWebRequest;
			request.CookieContainer = cookies;
			HttpWebResponse response = request.GetResponse() as HttpWebResponse;


			int length = (int)response.ContentLength;
			using (FileStream file = File.OpenWrite(filePath)) {
				using (Stream s = response.GetResponseStream()) {
					s.CopyTo(file);
				}
			}
		}

		private static string GetOrderDetailValue(HtmlDocument detailDoc, string valueName)
		{
			HtmlNode node = detailDoc.DocumentNode.FindNode("TD", "TEXT", valueName);
			node = node.NextSibling("TD");
			string value = node.InnerText;
			return value;
		}

		static bool LoginUser(string usr, string pwd, out CookieContainer cookies)
		{
			// stop request from inserting header Expect: 100-continue;
			ServicePointManager.Expect100Continue = false;

			//request home page to start a session
			cookies = new CookieContainer();
			string url = baseUrl;
			HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
			request.CookieContainer = cookies;
			HttpWebResponse response = request.GetResponse() as HttpWebResponse;

			//logon to docsolutions (Function=LogonCheck)
			url = baseUrl + "index.aspx?Function=LogonCheck";
			request = WebRequest.Create(url) as HttpWebRequest;
			request.CookieContainer = cookies;
			request.Method = "POST";
			request.AllowAutoRedirect = false;
			request.UseDefaultCredentials = true;
			request.Headers.Add("Cache-Control", "max-age=0");
			request.ContentType = "application/x-www-form-urlencoded";
			string formParams = String.Format
				("hidWarning=&UserId={0}&Password={1}&QueryString=&btnLogon=Sign+On", usr, pwd);
			byte[] bytes = Encoding.ASCII.GetBytes(formParams);
			request.ContentLength = bytes.Length;
			using (Stream os = request.GetRequestStream()) {
				os.Write(bytes, 0, bytes.Length);
			}
			response = request.GetResponse() as HttpWebResponse;

			//request index.aspx frameset after login
			url = baseUrl + "index.aspx";
			request = WebRequest.Create(url) as HttpWebRequest;
			request.CookieContainer = cookies;
			response = request.GetResponse() as HttpWebResponse;
			string html;
			using (StreamReader sr = new StreamReader(response.GetResponseStream()))
				html = sr.ReadToEnd();
			return html.StartsWith("<HTML><HEAD><TITLE> CoreLogic Document Solutions");

			//get welcome page (Function=CurrentScreen)
			//string welcomeUrl = baseUrl + "index.aspx?Function=CurrentScreen";
			//HttpWebRequest welcomeReq = WebRequest.Create(welcomeUrl) as HttpWebRequest;
			//welcomeReq.Headers.Add("Cookie", cookieHeader);
			//HttpWebResponse welcomeResp = welcomeReq.GetResponse() as HttpWebResponse;

			//send hidden request (Funtion=Hidden)
			//string hiddenUrl = baseUrl + "index.aspx?Function=Hidden";
			//HttpWebRequest hiddenReq = WebRequest.Create(hiddenUrl) as HttpWebRequest;
			//hiddenReq.Headers.Add("Cookie", cookieHeader);
			//HttpWebResponse hiddenResp = hiddenReq.GetResponse() as HttpWebResponse;
		}
	}
}