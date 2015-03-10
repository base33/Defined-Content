using DefinedContent.UI.Helpers;
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

            return TypeConverter.CoreItemToViewModel(item);
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
