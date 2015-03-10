using DefinedContent.Models;
using DefinedContent.UI.Helpers;
using DefinedContent.UI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Serialization;
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
            var item = TypeConverter.ViewModelToCore(model);

            string xml = Serialiser.Serialize<DefinedContentItem>(item).OuterXml;
        }

        [HttpDelete]
        public void Delete(string key)
        {
            
        }
    }
}
