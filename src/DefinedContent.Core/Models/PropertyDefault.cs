using DefinedContent.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DefinedContent.Models
{
	public class PropertyDefault
	{
		public string PropertyAlias { get; protected set; }
		
		public PropertyDefaultValueType ValueType { get; set; }
		public string Value { get; set; }

		public PropertyDefault(XElement propertyDefaultXml)
		{
			this.PropertyAlias = propertyDefaultXml.Attribute("propertyAlias").Value;
			this.Value = propertyDefaultXml.Attribute("value").Value;
			this.ValueType = (PropertyDefaultValueType)Enum.Parse(typeof(PropertyDefaultValueType), propertyDefaultXml.Attribute("valueType").Value);
		}
	}
}
