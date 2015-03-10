using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefinedContent.Models.Cache
{
	public abstract class CacheItem
	{
		public string Key { get; set; }
		public DefinedContentItem DefinedContentItem { get; set; }

		public CacheItem(DefinedContentItem item)
		{
			this.Key = item.Key;

		}

		public abstract int ResolveId(int? currentPageId = null);
	}
}
