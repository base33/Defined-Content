using DefinedContent.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace DefinedContent.Models
{
	public class DefinedContentItem : IEquatable<DefinedContentItem>
	{
		#region Properties

		//Resolve Information
		[XmlAttribute]
		public string Key { get; set; }

		[XmlAttribute]
		public ResolutionType ResolveType { get; set; }

		[XmlAttribute]
		public string ResolveValue { get; set; }

		[XmlAttribute]
		public string FilePath { get; set; }

		//Create Information
		[XmlAttribute]
		public DefinedContentItemType ItemType { get; set; }

		[XmlAttribute]
		public string ContentTypeAlias { get; set; }

		[XmlAttribute]
		public string Name { get; set; }

		[XmlAttribute]
		public string Parent { get; set; }

		[XmlElement]
		public ResolutionType? ParentType { get; set; }

		[XmlElement]
		public List<PropertyDefault> PropertyDefaults { get; set; }

		[XmlIgnore]
		public List<DefinedContentItem> Children { get; set; }

		#endregion

		#region Constructors

		public DefinedContentItem()
		{
			this.Children = new List<DefinedContentItem>();
		}

		#endregion

		#region Public Methods

		public bool CanCreate()
		{
			bool canCreate = ItemType == DefinedContentItemType.CreateAndResolve;

			if (string.IsNullOrEmpty(ContentTypeAlias)
				|| string.IsNullOrEmpty(Name)
				|| string.IsNullOrEmpty(Parent)
				|| !ParentType.HasValue)
			{
				canCreate = false;
			}

			return canCreate;
		}

		#endregion

		#region IEquatable members

		public bool Equals(DefinedContentItem other)
		{
			return this.Key == other.Key;
		}

		#endregion
	}
}
