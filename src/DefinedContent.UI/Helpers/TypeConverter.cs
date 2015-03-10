using DefinedContent.Enums;
using DefinedContent.Models;
using DefinedContent.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefinedContent.UI.Helpers
{
    public class TypeConverter
    {
        public static DefinedContentModel CoreItemToViewModel(DefinedContentItem item)
        {
            string resolveType = "":
            switch(item.ResolveType)
            {
                case ResolutionType.XPath:
                    resolveType = "xpath";
                    break;
                case ResolutionType.Key:
                    resolveType = "key";
                case ResolutionType.ContentId:
                    resolveType = "contentId";
                    break;
            }

            return new DefinedContentModel()
            {
                Key = item.Key,
                ResolveType = resolveType,
                ResolveValue = item.ResolveValue,
                ParentKey = item.Parent,

                CreateConfig = new CreateModel()
                {
                    Enabled = true, //TODO: item.CanCreate,
                    Name = item.Name,
                    ContentTypeAlias = item.ContentTypeAlias,
                    PropertyMapping = item.PropertyDefaults.Select(p => new PropertyMapping
                    {
                        Alias = p.PropertyAlias,
                        Value = p.Value,
                        IsKey = p.ValueType == PropertyDefaultValueType.Key
                    })
                }
            };
        }
    }
}
