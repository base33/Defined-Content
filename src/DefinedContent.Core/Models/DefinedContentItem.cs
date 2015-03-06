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

		public DefinedContentItem(XElement xml)
		{
			LoadAttributes(xml);
			LoadPropertyDefaults(xml);
		}

		private void LoadAttributes(XElement xml)
		{
			this.Key = xml.Attribute("key").Value;
			this.DocumentTypeId = xml.Attribute("docTypeId").Value;
			this.Name = xml.Attribute("name").Value;
			this.Parent = xml.Attribute("parent").Value;
			this.ParentType = (ParentType)Enum.Parse(typeof(ParentType), xml.Attribute("parentType").Value);
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
	}
}
