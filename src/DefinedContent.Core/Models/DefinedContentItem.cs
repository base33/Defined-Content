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


		//Create Information
		public string DocumentTypeId { get; protected set; }
		public string Name { get; protected set; }
		public string Parent { get; protected set; }
		public ResolutionType ParentType { get; protected set; }
		public List<PropertyDefault> PropertyDefaults { get; set; }

		#endregion

		#region Constructors

		public DefinedContentItem(XElement xml)
		{
			LoadAttributes(xml);
			LoadPropertyDefaults(xml);
		}

		#endregion

		#region Load from XML

		private void LoadAttributes(XElement xml)
		{
			this.Key = xml.Attribute("key").Value;
			this.ResolveType = (ResolutionType)Enum.Parse(typeof(ResolutionType), xml.Attribute("resolveType").Value);
			this.ResolveValue = xml.Attribute("resolveValue").Value;

			this.DocumentTypeId = xml.Attribute("docTypeId").Value;
			this.Name = xml.Attribute("name").Value;
			this.Parent = xml.Attribute("parent").Value;
			this.ParentType = (ResolutionType)Enum.Parse(typeof(ResolutionType), xml.Attribute("parentType").Value);
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

		#region IEquatable members

		public bool Equals(DefinedContentItem other)
		{
			return this.Key == other.Key;
		}

		#endregion
	}
}
