using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;

namespace Penatna
{
	/// <summary>
	/// Summary description for HtmlUtility
	/// </summary>
	public static class HtmlUtility
	{
		internal static HtmlNode FindNode(this HtmlNode node, string nodeName, string attributeName, string attributeValue)
		{
			StringComparison comp = StringComparison.OrdinalIgnoreCase;
			if (String.Equals(node.Name, nodeName, comp)) {
				if (attributeName == null) {
					return node;
				}
				if (attributeName == "TEXT") {
					if (node.InnerText == attributeValue) {
						return node;
					}
				} else if (node.HasAttributes && node.Attributes.Contains(attributeName)) {
					if (String.Equals(node.Attributes[attributeName].Value, attributeValue, comp)) {
						return node;
					}
				}
			}
			foreach (HtmlNode childNode in node.ChildNodes) {
				HtmlNode found = FindNode(childNode, nodeName, attributeName, attributeValue);
				if (found != null) {
					return found;
				}
			}
			return null;
		}

		internal static HtmlNode NextSibling(this HtmlNode node, string nodeName)
		{
			HtmlNode nextSibling = node;
			do { nextSibling = nextSibling.NextSibling; 
			} while(nextSibling != null && !String.Equals(nextSibling.Name, nodeName, StringComparison.OrdinalIgnoreCase));

			return nextSibling;
		}
	}
}