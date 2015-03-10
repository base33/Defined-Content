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
	public class PropertyDefault
	{
		#region Properties

		[XmlAttribute]
		public string PropertyAlias { get; set; }

		[XmlAttribute]
		public PropertyDefaultValueType ValueType { get; set; }

		[XmlAttribute]
		public string Value { get; set; }

		#endregion

		#region Constructors

		public PropertyDefault()
		{

		}

		public PropertyDefault(XElement propertyDefaultXml)
		{
			//LoadAttributes(propertyDefaultXml);
		}

		#endregion

		//#region Load from XML

		//private void LoadAttributes(XElement propertyDefaultXml)
		//{
		//	this.PropertyAlias = propertyDefaultXml.Attribute("propertyAlias").Value;
		//	this.Value = propertyDefaultXml.Attribute("value").Value;
		//	this.ValueType = (PropertyDefaultValueType)Enum.Parse(typeof(PropertyDefaultValueType), propertyDefaultXml.Attribute("valueType").Value);
		//}

		//#endregion
	}
}
