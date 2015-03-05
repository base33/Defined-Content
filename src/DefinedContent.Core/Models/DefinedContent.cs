using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;

namespace DefinedContent.Models
{
	public class DefinedContent
	{
		#region Singleton 

		public static DefinedContent Current { get; private set; }

		static DefinedContent()
		{
			Current = new DefinedContent();
		}

		#endregion

		protected List<DefinedContentItem> ContentItems { get; set; }

		protected DefinedContent()
		{

		}

		public static int GetId(string key)
		{
			throw new NotImplementedException();
		}

		public static IPublishedContent GetPublishedContent(string key)
		{
			throw new NotImplementedException();
		}
	}
}
