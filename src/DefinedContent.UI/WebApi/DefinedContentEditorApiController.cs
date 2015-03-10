using DefinedContent.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Web.WebApi;

namespace DefinedContent.UI.WebApi
{
    public class DefinedContentEditorApiController : UmbracoApiController
    {
        [HttpGet]
        public DefinedContentModel Get(string key)
        {
            var item = DefinedContent.Current.GetDefinedContentItem(key);

            

            return new DefinedContentModel()
            {
                Key = "Forum Root",
                ResolveType = "xpath",
                ResolveValue = "$currentPage/Ancestor-or-self [@nodeName = 'Forum']",

                CreateConfig = new CreateModel()
                {
                    Enabled = true,
                    Name = "Forum",
                    ContentTypeAlias = "Forum",
                    PropertyMapping = new List<PropertyMapping>
                    {
                        new PropertyMapping { Alias = "postsPerPage", Value = "10" },
                        new PropertyMapping { Alias = "allowSubscriptions", Value = "true" }
                    }
                }
            };
        }

        [HttpPost]
        public void Save(DefinedContentModel model)
        {
            
        }

        [HttpDelete]
        public void Delete(string key)
        {
            
        }
    }
}
