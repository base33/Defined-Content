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
		[XmlEnum]
		XPath = 1,
		[XmlEnum]
		ContentId = 2,
		[XmlEnum]
		Key = 3
	}
}
