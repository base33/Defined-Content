using DefinedContent.Enums;
using DefinedContent.Models;
using DefinedContent.UI.Helpers;
using DefinedContent.UI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
            string filePath = HttpContext.Current.Server.MapPath("~/") + Constants.CONFIG_DIRECTORY;

            //if its not empty, it means this was not created at the root
            if (model.DefinedContentParent != "-1")
            {
                var parent = DefinedContent.Current.GetDefinedContentItem(model.DefinedContentParent);
                filePath = Path.GetDirectoryName(parent.FilePath);
            }

            filePath = filePath.TrimEnd(new [] { '\\' }) + "\\" + item.Key + "\\" + Constants.CONFIG_FILE_NAME;

            string xml = Serialiser.Serialize<DefinedContentItem>(item).OuterXml;

            System.IO.FileInfo file = new System.IO.FileInfo(filePath);
            file.Directory.Create();

            File.WriteAllText(filePath, xml);

            try
            {
                DefinedContent.Current.FullRefresh();
            }
            catch (Exception) { }
        }

        [HttpDelete]
        public void Delete(string key)
        {
            
        }
    }
}
