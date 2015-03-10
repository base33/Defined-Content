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

		public List<DefinedContentItem> Children { get; set; }

		#endregion

		#region Constructors

		public DefinedContentItem()
		{
			this.Children = new List<DefinedContentItem>();
		}

		public DefinedContentItem(string filePath)
		{
			//XElement xml = LoadXml(filePath);
			//LoadAttributes(xml);
			//LoadPropertyDefaults(xml);
		}

		#endregion

		//#region Load from XML

		//private XElement LoadXml(string filePath)
		//{
		//	XElement xml = XElement.Load(filePath);

		//	return xml.Descendants().First();
		//}

		//private void LoadAttributes(XElement xml)
		//{
		//	this.Key = xml.Attribute("key").Value;

		//	DefinedContentItemType? itemType = GetEnumValueFromAttribute<DefinedContentItemType>(xml.Attribute("type"));
		//	if (!itemType.HasValue)
		//		throw new Exception("Invalid item type on key " + this.Key);

		//	this.ItemType = itemType.Value;

		//	ResolutionType? resolutionType = GetEnumValueFromAttribute<ResolutionType>(xml.Attribute("resolveType"));
		//	if (!resolutionType.HasValue)
		//		throw new Exception("Invalid resolution type on key + " + this.Key);

		//	this.ResolveType = resolutionType.Value;
		//	this.ResolveValue = xml.Attribute("resolveValue").Value;

		//	this.ContentTypeAlias = GetAttributeValue(xml.Attribute("docTypeId"));
		//	this.Name = GetAttributeValue(xml.Attribute("name"));
		//	this.Parent = GetAttributeValue(xml.Attribute("parent"));
		//	this.ParentType = GetEnumValueFromAttribute<ResolutionType>(xml.Attribute("parentType"));
		//}

		//private string GetAttributeValue(XAttribute attribute)
		//{
		//	if (attribute != null)
		//		return attribute.Value;

		//	return "";
		//}

		//private T? GetEnumValueFromAttribute<T>(XAttribute attribute) where T : struct
		//{
		//	if (attribute == null)
		//		return null;

		//	T resolutionType;

		//	if (Enum.TryParse<T>(attribute.Value, out resolutionType))
		//		return resolutionType;

		//	return null;
		//}

		//private void LoadPropertyDefaults(XElement xml)
		//{
		//	this.PropertyDefaults = new List<PropertyDefault>();

		//	XElement propertyDefaultsXml = xml.Descendants("propertyDefaults").FirstOrDefault();

		//	if (propertyDefaultsXml != null)
		//	{
		//		foreach (XElement propertyDefaultXml in propertyDefaultsXml.Descendants())
		//		{
		//			this.PropertyDefaults.Add(new PropertyDefault(propertyDefaultXml));
		//		}
		//	}
		//}

		//#endregion

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
