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
			var item = DefinedContent.Cache.GetDefinedContentItem(key);

			return TypeConverter.CoreItemToViewModel(item);
		}

		[HttpPost]
		public bool Save(DefinedContentModel model)
		{
			try
			{
				if (ValidateModel(model).Count == 0)
				{

					var item = TypeConverter.ViewModelToCore(model);
					string filePath = HttpContext.Current.Server.MapPath("~/") + Constants.CONFIG_DIRECTORY;

					//if its not empty, it means this was not created at the root
					if (model.DefinedContentParent != "-1")
					{
						var parent = DefinedContent.Cache.GetDefinedContentItem(model.DefinedContentParent);
						filePath = Path.GetDirectoryName(parent.FilePath);
					}

					filePath = filePath.TrimEnd(new[] { '\\' }) + "\\" + item.Key + "\\" + Constants.CONFIG_FILE_NAME;

					string xml = Serialiser.Serialize<DefinedContentItem>(item).OuterXml;

					System.IO.FileInfo file = new System.IO.FileInfo(filePath);
					file.Directory.Create();

					File.WriteAllText(filePath, xml);

					DefinedContent.Cache.FullRefresh();

					return true;
				}
			}
			catch (Exception) { }

			return false;
		}

		[HttpPost]
		public List<string> ValidateModel(DefinedContentModel model)
		{
			List<string> errors = new List<string>();

			if (string.IsNullOrEmpty(model.Key))
				errors.Add("You must specify a unique Key for this Defined Content Item.");
			else if (DefinedContent.TryGetId(model.Key).HasValue)
				errors.Add("The Key you specified is not unique, Keys must be unique.");

			if (string.IsNullOrEmpty(model.ResolveType))
				errors.Add("You must specify a Resolution so we know how to resolve this Key.");

			if (model.CreateConfig.Enabled)
			{
				if (string.IsNullOrEmpty(model.CreateConfig.ContentTypeAlias))
					errors.Add("In order for creation to work, you must specify the ContentTypeAlias to create.");

				if (string.IsNullOrEmpty(model.CreateConfig.Name))
					errors.Add("In order for creation to work, you must specify the Name of the content item to create.");

				if (string.IsNullOrEmpty(model.DefinedContentParent) && string.IsNullOrEmpty(model.ParentKey))
					errors.Add("In order for creation to work, you must specify a parent node, either by Key, Id or XPath.");

				if (string.IsNullOrEmpty(model.ParentResolveType))
					errors.Add("In order for creation to work, you must specify how we should resolve the parent node.");
			}

			return errors;
		}

		[HttpDelete]
		public void Delete(string key)
		{
			var item = DefinedContent.Cache.GetDefinedContentItem(key);

			File.Delete(item.FilePath);

			DefinedContent.Cache.FullRefresh();
		}
	}
}
