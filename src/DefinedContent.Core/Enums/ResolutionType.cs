using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DefinedContent.Enums
{
	public enum ResolutionType : int
	{
		[XmlEnum(Name="XPath")]
		XPath = 1,
		[XmlEnum(Name="ContentId")]
		ContentId = 2,
		[XmlEnum(Name="Key")]
		Key = 3
	}
}
