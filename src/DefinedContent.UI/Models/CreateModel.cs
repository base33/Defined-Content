using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefinedContent.UI.Models
{
    public class CreateModel
    {
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string ContentTypeAlias { get; set; }
        public IEnumerable<PropertyMapping> PropertyMapping { get; set; }
    }
}
