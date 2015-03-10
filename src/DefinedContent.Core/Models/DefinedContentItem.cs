using DefinedContent.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DefinedContent.Models
{
	public class DefinedContentItem : IEquatable<DefinedContentItem>
	{
		#region Properties

		//Resolve Information
		public string Key { get; protected set; }
		public ResolutionType ResolveType { get; set; }
		public string ResolveValue { get; set; }

		public string FilePath { get; set; }

		//Create Information
		public DefinedContentItemType ItemType { get; set; }
		public string ContentTypeAlias { get; protected set; }
		public string Name { get; protected set; }
		public string Parent { get; protected set; }
		public ResolutionType? ParentType { get; protected set; }
		public List<PropertyDefault> PropertyDefaults { get; set; }

		public List<DefinedContentItem> Children { get; set; }

		#endregion

		#region Constructors

		public DefinedContentItem(string filePath)
		{
			this.FilePath = filePath;

			this.Children = new List<DefinedContentItem>();

			XElement xml = LoadXml(filePath);
			LoadAttributes(xml);
			LoadPropertyDefaults(xml);
		}

		#endregion

		#region Load from XML

		private XElement LoadXml(string filePath)
		{
			XElement xml = XElement.Load(filePath);

			return xml.Descendants().First();
		}

		private void LoadAttributes(XElement xml)
		{
			this.Key = xml.Attribute("key").Value;

			DefinedContentItemType? itemType = GetEnumValueFromAttribute<DefinedContentItemType>(xml.Attribute("type"));
			if (!itemType.HasValue)
				throw new Exception("Invalid item type on key " + this.Key);

			this.ItemType = itemType.Value;

			ResolutionType? resolutionType = GetEnumValueFromAttribute<ResolutionType>(xml.Attribute("resolveType"));
			if (!resolutionType.HasValue)
				throw new Exception("Invalid resolution type on key + " + this.Key);

			this.ResolveType = resolutionType.Value;
			this.ResolveValue = xml.Attribute("resolveValue").Value;

			this.ContentTypeAlias = GetAttributeValue(xml.Attribute("docTypeId"));
			this.Name = GetAttributeValue(xml.Attribute("name"));
			this.Parent = GetAttributeValue(xml.Attribute("parent"));
			this.ParentType = GetEnumValueFromAttribute<ResolutionType>(xml.Attribute("parentType"));
		}

		private string GetAttributeValue(XAttribute attribute)
		{
			if (attribute != null)
				return attribute.Value;

			return "";
		}

		private T? GetEnumValueFromAttribute<T>(XAttribute attribute) where T : struct
		{
			if (attribute == null)
				return null;

			T resolutionType;

			if (Enum.TryParse<T>(attribute.Value, out resolutionType))
				return resolutionType;

			return null;
		}

		private void LoadPropertyDefaults(XElement xml)
		{
			this.PropertyDefaults = new List<PropertyDefault>();

			XElement propertyDefaultsXml = xml.Descendants("propertyDefaults").FirstOrDefault();

			if (propertyDefaultsXml != null)
			{
				foreach (XElement propertyDefaultXml in propertyDefaultsXml.Descendants())
				{
					this.PropertyDefaults.Add(new PropertyDefault(propertyDefaultXml));
				}
			}
		}

		#endregion

		#region Public Methods

		public bool CanCreate()
		{
			bool canCreate = true;

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
