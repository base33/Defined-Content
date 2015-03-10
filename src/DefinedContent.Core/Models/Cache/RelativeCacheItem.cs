using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web;

namespace DefinedContent.Models.Cache
{
	public class RelativeCacheItem : CacheItem
	{
		UmbracoHelper _umbraco;

		public RelativeCacheItem(DefinedContentItem item, UmbracoHelper helper)
			: base(item)
		{
			_umbraco = helper;	
		}

		public override int ResolveId(int? currentPageId = null)
		{
			currentPageId = currentPageId ?? UmbracoContext.Current.PageId;

			if (!currentPageId.HasValue)
				throw new Exception("Cannot resolve the current page in the current context. Please pass in the current page id in to your call to DefinedContent.Id - " + this.Key);

			string xPath = this.DefinedContentItem.ResolveValue;
			xPath = xPath.Replace("$currentPage", string.Format("//* ", currentPageId.Value));

			return 1;
		}
	}
}
