using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefinedContent.Models.Cache
{
	public class StaticCacheItem : CacheItem
	{
		public int StaticId { get; set; }

		public StaticCacheItem(DefinedContentItem item, int resolvedId)
			: base(item)
		{
			this.StaticId = resolvedId;
		}

		public override int ResolveId(int? currentPageId = null)
		{
			return this.StaticId;
		}
	}
}
