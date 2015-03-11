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
            string resolveType = "";
            switch(item.ResolveType)
            {
                case ResolutionType.XPath:
                    resolveType = "xpath";
                    break;
                case ResolutionType.Key:
                    resolveType = "key";
                    break;
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
                DefinedContentParent = item.Parent == "" ? "-1" : item.Parent,

                CreateConfig = new CreateModel()
                {
                    Enabled = item.ItemType == DefinedContentItemType.CreateAndResolve,
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

        public static DefinedContentItem ViewModelToCore(DefinedContentModel model)
        {
            ResolutionType resolveType;

            switch (model.ResolveType)
            {
                case "xpath":
                    resolveType = ResolutionType.XPath;
                    break;
                case "key":
                    resolveType = ResolutionType.Key;
                    break;
                case "contentId":
                    resolveType = ResolutionType.ContentId;
                    break;
                default:
                    resolveType = ResolutionType.Key;
                    break;
            }

            return new DefinedContentItem()
            {
                Key = model.Key,
                Parent = model.ParentKey,
                ParentType = ResolutionType.Key, //TODO: needs to get parent xpath, contentid, or key in the editor
                ResolveType = resolveType,
                ResolveValue = model.ResolveValue,
                ItemType = model.CreateConfig.Enabled 
                    ? DefinedContentItemType.CreateAndResolve 
                    : DefinedContentItemType.Resolve,
                ContentTypeAlias = model.CreateConfig.ContentTypeAlias,
                Name = model.CreateConfig.Name,
                PropertyDefaults = model.CreateConfig.PropertyMapping.Select(p => new PropertyDefault() {  PropertyAlias = p.Alias, Value = p.Value, ValueType = p.IsKey ? PropertyDefaultValueType.Key : PropertyDefaultValueType.StaticValue }).ToList()
            };
        }
    }
}
