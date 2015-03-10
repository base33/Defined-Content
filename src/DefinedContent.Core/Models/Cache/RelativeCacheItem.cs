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
		public RelativeCacheItem(DefinedContentItem item)
			: base(item)
		{
		}

		public override int ResolveId(int? currentPageId = null)
		{
			return XPathResolver.ResolveRelative(this.DefinedContentItem.ResolveValue, currentPageId);
		}
	}
}
