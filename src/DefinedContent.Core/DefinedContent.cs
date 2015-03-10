using DefinedContent.Enums;
using DefinedContent.Models;
using DefinedContent.Models.Cache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
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

		#region Properties

		protected List<DefinedContentItem> ContentItems { get; set; }
		protected Dictionary<string, CacheItem> KeyToNodeIdCache { get; set; }

		protected List<DefinedContentItem> AwaitingResolution { get; set; }
		protected List<DefinedContentItem> CreatedItems { get; set; }

		UmbracoHelper _umbraco;
		IContentService _contentService;

		#endregion

		#region Constructors

		protected DefinedContent()
		{
			_umbraco = new UmbracoHelper(UmbracoContext.Current);
			_contentService = UmbracoContext.Current.Application.Services.ContentService;

			FullRefresh();
		}

		#endregion

		#region Public Static Methods

		public static int Id(string key)
		{
			return Current.GetId(key);
		}

		public static int Id(string key, int currentPage)
		{
			return Current.GetId(key, currentPage);
		}

		public static IPublishedContent TypedContent(string key)
		{
			return Current.GetTypedContent(key);
		}

		#endregion

		#region Public Methods

		public int GetId(string key)
		{
			if (!this.KeyToNodeIdCache.ContainsKey(key))
				throw new Exception("Unknown key " + key);

			return this.KeyToNodeIdCache[key].ResolveId();
		}

		public int GetId(string key, int currentPageId)
		{
			if (!this.KeyToNodeIdCache.ContainsKey(key))
				throw new Exception("Unknown key " + key);

			return this.KeyToNodeIdCache[key].ResolveId(currentPageId);
		}

		public int? TryGetId(string key)
		{
			if (!this.KeyToNodeIdCache.ContainsKey(key))
				return null;

			return this.KeyToNodeIdCache[key].ResolveId();
		}

		public DefinedContentItem GetDefinedContentItem(string key)
		{
			var item = (from ci in this.ContentItems
						where ci.Key == key
						select ci).FirstOrDefault();

			if (item == null)
				throw new Exception("Unknown key " + key);

			return item;
		}

		public IPublishedContent GetTypedContent(string key)
		{
			int id = GetId(key);

			return _umbraco.TypedContent(id);
		}

		/// <summary>
		/// Reloads XML configs, ensures all content exists and rebuilds the defined content cache
		/// </summary>
		public void FullRefresh(UmbracoContext umbracoContext = null)
		{
			if (umbracoContext != null)
				_umbraco = new UmbracoHelper(umbracoContext);

			this.ContentItems = new List<DefinedContentItem>();
			this.KeyToNodeIdCache = new Dictionary<string, CacheItem>();
			this.AwaitingResolution = new List<DefinedContentItem>();
			this.CreatedItems = new List<DefinedContentItem>();

			LoadXmlConfigs();
			BuildCache(this.ContentItems);
			SetPropertyDefaults();
		}

		public void AddStaticItemToCache(DefinedContentItem item, int resolvedNodeId)
		{
			if (this.KeyToNodeIdCache.ContainsKey(item.Key))
				throw new Exception("Duplicate key detected " + item.Key);

			this.KeyToNodeIdCache.Add(item.Key, new StaticCacheItem(item, resolvedNodeId));
		}

		public void AddRelativeItemToCache(DefinedContentItem item)
		{
			if (this.KeyToNodeIdCache.ContainsKey(item.Key))
				throw new Exception("Duplicate key detected " + item.Key);

			this.KeyToNodeIdCache.Add(item.Key, new RelativeCacheItem(item));
		}

		#endregion

		#region Load from XML

		/// <summary>
		/// Loads XML files into memory
		/// </summary>
		protected void LoadXmlConfigs()
		{
			string xmlConfigDirectoryPath = AppDomain.CurrentDomain.BaseDirectory + Constants.CONFIG_DIRECTORY;

			DirectoryInfo configDirectory = new DirectoryInfo(xmlConfigDirectoryPath);

			RecursivelyLoadDirectory(configDirectory);
		}

		private void RecursivelyLoadDirectory(DirectoryInfo configDirectory, DefinedContentItem parent = null)
		{
			var configFile = new FileInfo(configDirectory + "\\" + Constants.CONFIG_FILE_NAME);

			XmlSerializer serializer = new XmlSerializer(typeof(DefinedContentItem));
			using (FileStream fs = System.IO.File.OpenRead(configFile.FullName))
			{
				DefinedContentItem item = (DefinedContentItem)serializer.Deserialize(fs);
				item.FilePath = configFile.FullName;

				if (parent == null)
					this.ContentItems.Add(item);
				else
					parent.Children.Add(item);


				var subDirs = configDirectory.GetDirectories();
				foreach (var subDir in subDirs)
				{
					RecursivelyLoadDirectory(subDir, item);
				}
			}
		}

		#endregion

		#region Build Cache

		/// <summary>
		/// Builds the defined content cache from already loaded XML configs.
		/// </summary>
		protected void BuildCache(List<DefinedContentItem> contentItems)
		{
			for (int i = 0; i < contentItems.Count; i++)
			{
				ResolveNodeId(contentItems[i]);

				BuildCache(contentItems[i].Children);
			}

			while (this.AwaitingResolution.Count > 0)
			{
				BuildCache(this.AwaitingResolution);
			}
		}

		/// <summary>
		/// Determines the correct method for resolve the parent node and adds the first matched child node id to the cache.
		/// </summary>
		/// <param name="item">Defined Content Item to match</param>
		private void ResolveNodeId(DefinedContentItem item)
		{
			int? nodeId = null;

			switch (item.ResolveType)
			{
				case ResolutionType.ContentId:
					nodeId = ResolveItemById(item);
					break;
				case ResolutionType.Key:
					nodeId = ResolveItemByKey(item);
					break;
				case ResolutionType.XPath:
					nodeId = ResolveItemByXPath(item);
					break;
				default:
					throw new InvalidOperationException("Unknown parentType in Key " + item.Key);
			}

			if (nodeId.HasValue)
				AddStaticItemToCache(item, nodeId.Value);
		}

		/// <summary>
		/// Checks a node id exists and adds it to the cache.
		/// </summary>
		/// <param name="item"></param>
		private int? ResolveItemById(DefinedContentItem item, bool errorOnNotExists = true)
		{
			IPublishedContent resolvedNode = _umbraco.TypedContent(item.ResolveValue);

			if (resolvedNode == null)
			{
				if (errorOnNotExists)
					throw new Exception("Cannot resolve node id for key " + item.Key + ". Invalid node id " + item.ResolveValue);

				return null;
			}

			return resolvedNode.Id;
		}

		/// <summary>
		/// Resolves a node id by looking up another key
		/// </summary>
		/// <param name="item">Defined Content Item to match</param>
		private int? ResolveItemByKey(DefinedContentItem item)
		{
			int? nodeId = TryGetId(item.ResolveValue);

			if (!nodeId.HasValue)
			{
				if (!this.AwaitingResolution.Contains(item))
					this.AwaitingResolution.Add(item);

				return null;
			}

			if (this.AwaitingResolution.Contains(item))
				this.AwaitingResolution.Remove(item);

			return this.KeyToNodeIdCache[item.ResolveValue].ResolveId();
		}

		/// <summary>
		/// Resolves a parent using XPath and adds the first matched child node id to the cache.
		/// </summary>
		/// <param name="item">Defined Content Item to match</param>
		private int? ResolveItemByXPath(DefinedContentItem item)
		{
			if (item.ResolveValue.Contains("$currentPage"))
			{
				AddRelativeItemToCache(item);
				return null;
			}

			int? resolvedNode = XPathResolver.ResolveStatic(item.ResolveValue, false);

			if (!resolvedNode.HasValue)
				return CreateItem(item);
			else
				return resolvedNode.Value;
		}

		#endregion

		#region Create Content

		private int? CreateItem(DefinedContentItem item)
		{
			if (item.CanCreate())
			{
				this.CreatedItems.Add(item);

				int? parentId = ResolveParent(item);

				if (!parentId.HasValue)
				//Can't create until the parent has been created / resolved.
				{
					this.AwaitingResolution.Add(item);
					return null;
				}

				if (this.AwaitingResolution.Contains(item))
					this.AwaitingResolution.Remove(item);

				IContent createdContent = _contentService.CreateContentWithIdentity(item.Name, parentId.Value, item.ContentTypeAlias);
				this.CreatedItems.Add(item);

				return createdContent.Id;
			}

			throw new Exception("Not enough information provided to create item with key. Need docTypeId, name, parent and ParentType." + item.Key);
		}

		private int? ResolveParent(DefinedContentItem item)
		{
			switch (item.ParentType)
			{
				case ResolutionType.ContentId:
					return ResolveParentById(item);
				case ResolutionType.Key:
					return ResolveParentByKey(item);
				case ResolutionType.XPath:
					return ResolveParentByXPath(item);
			}

			throw new Exception("Unknown ResolutionType " + item.ParentType.Value.ToString());
		}

		private int? ResolveParentById(DefinedContentItem item, bool errorOnNotExists = true)
		{
			IPublishedContent resolvedNode = _umbraco.TypedContent(item.Parent);

			if (resolvedNode == null)
			{
				if (errorOnNotExists)
					throw new Exception("Cannot resolve parent node id for key " + item.Key + ". Invalid node id " + item.Parent);

				return null;
			}

			return resolvedNode.Id;
		}

		private int? ResolveParentByKey(DefinedContentItem item)
		{
			throw new NotImplementedException();
		}

		private int? ResolveParentByXPath(DefinedContentItem item)
		{
			throw new NotImplementedException();
		}

		#region Set Property Defaults

		private void SetPropertyDefaults()
		{
			foreach (DefinedContentItem item in this.CreatedItems)
			{
				SetPropertyDefaults(item);
			}
		}

		private void SetPropertyDefaults(DefinedContentItem item)
		{

		}

		#endregion

		#endregion
	}
}
