using DefinedContent.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DefinedContent.Models
{
	public class DefinedContentItem
	{
		public string Key { get; protected set; }
		public string DocumentTypeId { get; protected set; }
		public string Name { get; protected set; }
		public string Parent { get; protected set; }
		public ParentType ParentType { get; protected set; }

		public List<PropertyDefault> PropertyDefaults { get; set; }

		public List<DefinedContentItem> Children { get; protected set; }

		public DefinedContentItem(XElement xml)
		{

		}
	}
}
