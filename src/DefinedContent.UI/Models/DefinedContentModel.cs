using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefinedContent.UI.Models
{
    public class DefinedContentModel
    {
        public string Key { get; set; }
        public string ResolveType { get; set; }
        public string ResolveValue { get; set; }

        public string DefinedContentParent { get; set; } //the physical parent - where to save

        public CreateModel CreateConfig { get; set; }

        public string ParentKey { get; set; }
    }
}
