using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web;

namespace DefinedContent
{
	public static class XPathResolver
	{
		public static int? ResolveStatic(string xPath, bool errorOnNotExists = true)
		{
			var content = umbraco.library.GetXmlNodeByXPath(xPath);

			if (content == null || content.Count == 0)
			{
				if (errorOnNotExists)
					throw new Exception("xPath " + xPath + " failed to return anything!");

				return null;
			}

			content.MoveNext();

			string idStr = content.Current.GetAttribute("id", "");

			return Convert.ToInt32(idStr);
		}

		public static int ResolveRelative(string xPath, int? currentPageId = null)
		{
			currentPageId = currentPageId ?? UmbracoContext.Current.PageId;

			if (!currentPageId.HasValue)
				throw new Exception("Cannot resolve the current page in the current context. Please pass in the current page id in to your call to DefinedContent.Id");

			xPath = xPath.Replace("$currentPage", string.Format("//* [@id='{0}']", currentPageId.Value));

			return ResolveStatic(xPath).Value;
		}
	}
}
