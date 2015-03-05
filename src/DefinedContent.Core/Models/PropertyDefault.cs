using DefinedContent.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefinedContent.Models
{
	public class PropertyDefault
	{
		public string PropertyAlias { get; protected set; }
		
		public PropertyDefaultValueType ValueType { get; set; }
		public string Value { get; set; }
	}
}
