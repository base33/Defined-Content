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
            return new DefinedContentModel()
            {
                Key = item.Key,
                ResolveType = GetViewResolveType(item.ResolveType),
                ResolveValue = item.ResolveValue,
                ParentKey = item.Parent,
                ParentResolveType = GetViewResolveType(item.ParentType.Value),
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

        private static string GetViewResolveType(ResolutionType type)
        {
            switch (type)
            {
                case ResolutionType.XPath:
                    return "xpath";
                case ResolutionType.Key:
                    return "key";
                case ResolutionType.ContentId:
                    return "contentId";
            }
            return "key";
        }

        public static DefinedContentItem ViewModelToCore(DefinedContentModel model)
        {
            return new DefinedContentItem()
            {
                Key = model.Key,
                Parent = model.ParentKey,
                ParentType = GetCoreResolutionType(model.ParentResolveType), //TODO: needs to get parent xpath, contentid, or key in the editor
                ResolveType = GetCoreResolutionType(model.ResolveType),
                ResolveValue = model.ResolveValue,
                ItemType = model.CreateConfig.Enabled 
                    ? DefinedContentItemType.CreateAndResolve 
                    : DefinedContentItemType.Resolve,
                ContentTypeAlias = model.CreateConfig.ContentTypeAlias,
                Name = model.CreateConfig.Name,
                PropertyDefaults = model.CreateConfig.PropertyMapping.Select(p => new PropertyDefault() {  PropertyAlias = p.Alias, Value = p.Value, ValueType = p.IsKey ? PropertyDefaultValueType.Key : PropertyDefaultValueType.StaticValue }).ToList()
            };
        }

        private static ResolutionType GetCoreResolutionType(string type)
        {
            switch (type)
            {
                case "xpath":
                    return ResolutionType.XPath;
                case "key":
                    return ResolutionType.Key;
                case "contentId":
                    return ResolutionType.ContentId;
                default:
                    return ResolutionType.Key;
            }
        }
    }
}
