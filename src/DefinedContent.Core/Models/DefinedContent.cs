using DefinedContent.Enums;
using DefinedContent.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace DefinedContent
{
	public class DefinedContent
	{
		#region Singleton

		public static DefinedContent Current { get; private set; }

		static DefinedContent()
		{
			Current = new DefinedContent();
		}

		#endregion

		protected List<DefinedContentItem> ContentItems { get; set; }
		protected Dictionary<string, int> KeyToNodeIdCache { get; set; }

		protected UmbracoHelper Umbraco { get; set; }

		protected DefinedContent()
		{
			Umbraco = new UmbracoHelper(UmbracoContext.Current);

			FullRefresh();
		}

		#region Public Static Methods

		public static int GetId(string key)
		{
			return Current.KeyToNodeIdCache[key];
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Reloads XML configs, ensures all content exists and rebuilds the defined content cache
		/// </summary>
		public void FullRefresh(UmbracoContext umbracoContext = null)
		{
			if (umbracoContext != null)
				Umbraco = new UmbracoHelper(umbracoContext);

			this.ContentItems = new List<DefinedContentItem>();
			this.KeyToNodeIdCache = new Dictionary<string, int>();

			LoadXmlConfigs();
			CreateContent();
			BuildCache();
		}

		/// <summary>
		/// Loads XML files into memory
		/// </summary>
		protected void LoadXmlConfigs()
		{
			string xmlConfigDirectoryPath = AppDomain.CurrentDomain.BaseDirectory + Constants.CONFIG_DIRECTORY;

			DirectoryInfo configDirectory = new DirectoryInfo(xmlConfigDirectoryPath);

			RecursivelyLoadDirectory(configDirectory);
		}

		/// <summary>
		/// Ensures all defined content exists 
		/// </summary>
		protected void CreateContent()
		{

		}

		/// <summary>
		/// Builds the defined content cache from already loaded XML configs.
		/// </summary>
		protected void BuildCache()
		{
			foreach (DefinedContentItem item in this.ContentItems)
			{
				ResolveNodeId(item);
			}
		}

		#endregion

		#region Load from XML

		private void RecursivelyLoadDirectory(DirectoryInfo configDirectory)
		{
			var xmlFiles = configDirectory.GetFiles("*.xml");

			foreach (var xmlFile in xmlFiles)
			{
				LoadFromXmlFile(xmlFile);
			}

			var subDirs = configDirectory.GetDirectories();

			foreach (var subDir in subDirs)
			{
				RecursivelyLoadDirectory(subDir);
			}
		}

		private void LoadFromXmlFile(FileInfo xmlFile)
		{
			XElement xml = XElement.Load(xmlFile.FullName);

			foreach (XElement contentItemXml in xml.Descendants())
			{
				this.ContentItems.Add(new DefinedContentItem(contentItemXml));
			}
		}

		#endregion

		#region NodeId Resolution

		private void ResolveNodeId(DefinedContentItem item)
		{
			switch (item.ParentType)
			{
				case ParentType.ContentId:
					ResolveByContentId(item);
					break;
				case ParentType.Key:
					ResolveByParentKey(item);
					break;
				case ParentType.XPath:
					break;
				default:
					throw new InvalidOperationException("Unknown parentType in Key " + item.Key);
			}
		}

		private void ResolveByContentId(DefinedContentItem item)
		{
			IPublishedContent resolvedNode;
			IEnumerable<IPublishedContent> possibilities;

			if (item.Parent == "-1")
			{
				possibilities = Umbraco.TypedContentAtRoot();
			}
			else
			{
				IPublishedContent parent = Umbraco.TypedContent(item.Parent);
				possibilities = parent.Children;
			}

			resolvedNode = possibilities.Where(x => x.DocumentTypeAlias == item.DocumentTypeId && x.Name == item.Name).FirstOrDefault();

			if (resolvedNode == null)
				throw new Exception("Cannot resolve node id for key" + item.Key);

			this.KeyToNodeIdCache.Add(item.Key, resolvedNode.Id);
		}

		private void ResolveByParentKey(DefinedContentItem item)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
