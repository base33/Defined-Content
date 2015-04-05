using DefinedContent.Enums;
using DefinedContent.Models;
using DefinedContent.Models.Cache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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

		public static DefinedContent Cache { get; private set; }

		static DefinedContent()
		{
			Cache = new DefinedContent();
		}

		#endregion

		#region Properties

		protected List<DefinedContentItem> ContentItems { get; set; }
		protected Dictionary<string, CacheItem> KeyToNodeIdCache { get; set; }

		protected List<DefinedContentItem> AwaitingResolution { get; set; }
		protected List<DefinedContentItem> CreatedItems { get; set; }

		UmbracoHelper _umbraco;
		IContentService _contentService;
		string _configDirectory;

		#endregion

		#region Constructors

		public DefinedContent(string configDirectory = "")
		{
			_umbraco = new UmbracoHelper(UmbracoContext.Current);
			_contentService = UmbracoContext.Current.Application.Services.ContentService;

			if (string.IsNullOrEmpty(configDirectory))
				_configDirectory = HttpContext.Current.Server.MapPath("~/") + Constants.CONFIG_DIRECTORY;
			else
				_configDirectory = configDirectory;

			FullRefresh();
		}

		#endregion

		#region Public Static Methods

		public static int Id(string key)
		{
			return Cache.GetId(key);
		}

		public static int Id(string key, int currentPageId)
		{
			return Cache.GetId(key, currentPageId);
		}

		public static int? TryGetId(string key)
		{
			return Cache.AttemptGetId(key);
		}

		public static int? TryGetId(string key, int currentPageId)
		{
			return Cache.AttemptGetId(key, currentPageId);
		}

		public static IPublishedContent TypedContent(string key)
		{
			return Cache.GetTypedContent(key);
		}

		public static IPublishedContent TypedContent(string key, int currentPageId)
		{
			return Cache.GetTypedContent(key, currentPageId);
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

		public int? AttemptGetId(string key)
		{
			if (!this.KeyToNodeIdCache.ContainsKey(key))
				return null;

			return this.KeyToNodeIdCache[key].ResolveId();
		}

		public int? AttemptGetId(string key, int currentPageId)
		{
			if (!this.KeyToNodeIdCache.ContainsKey(key))
				return null;

			return this.KeyToNodeIdCache[key].ResolveId(currentPageId);
		}

		public DefinedContentItem GetDefinedContentItem(string key)
		{
            var contentItemsFlat = new List<DefinedContentItem>();
            PopulateList(contentItemsFlat, this.ContentItems.First());

            var item = (from ci in contentItemsFlat
						where ci.Key == key
						select ci).FirstOrDefault();

			if (item == null)
				throw new Exception("Unknown key " + key);

			return item;
        }

        void PopulateList(List<DefinedContentItem> source, DefinedContentItem current)
        {
            source.Add(current);

            foreach (var item in current.Children)
            {
                PopulateList(source, item);
            }
        }

		public IPublishedContent GetTypedContent(string key)
		{
			int id = GetId(key);

			return _umbraco.TypedContent(id);
		}

		public IPublishedContent GetTypedContent(string key, int currentPageId)
		{
			int id = GetId(key, currentPageId);

			return _umbraco.TypedContent(id);
		}

        /// <summary>
        /// Returns all defined content items that have no parents
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DefinedContentItem> GetRootDefinedContentItems()
        {
			return ContentItems;
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
			while (this.AwaitingResolution.Count > 0)
			{
				BuildCache(this.AwaitingResolution);
			}
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
			DirectoryInfo configDirectory = new DirectoryInfo(_configDirectory);

			RecursivelyLoadDirectory(configDirectory);
		}

		private void RecursivelyLoadDirectory(DirectoryInfo configDirectory, DefinedContentItem parent = null)
		{
			var configFile = new FileInfo(configDirectory.FullName + "\\" + Constants.CONFIG_FILE_NAME);

			string configFilePath = configDirectory.FullName + "\\" + Constants.CONFIG_FILE_NAME;
			DefinedContentItem item = null; 

			if (System.IO.File.Exists(configFilePath))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(DefinedContentItem));

				using (FileStream fs = System.IO.File.OpenRead(configFilePath))
				{
					item = (DefinedContentItem)serializer.Deserialize(fs);
					item.FilePath = configFilePath;

					if (parent == null)
						this.ContentItems.Add(item);
					else
						parent.Children.Add(item);
				}
			}

			var subDirs = configDirectory.GetDirectories();
			foreach (var subDir in subDirs)
			{
				RecursivelyLoadDirectory(subDir, item);
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
				DefinedContentItem item = contentItems[i];

				ResolveNodeId(item);

				if (item.Children.Count > 0)
					BuildCache(item.Children);
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
			int? nodeId = AttemptGetId(item.ResolveValue);

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
			return AttemptGetId(item.Parent);
		}

		private int? ResolveParentByXPath(DefinedContentItem item)
		{
			return XPathResolver.ResolveStatic(item.Parent);
		}

		#region Set Property Defaults

		private void SetPropertyDefaults()
		{
			foreach (DefinedContentItem item in this.CreatedItems)
			{
				IContent contentItem = _contentService.GetById(GetId(item.Key));

				SetPropertyDefaults(item, contentItem);

				_contentService.SaveAndPublishWithStatus(contentItem);
			}
		}

		private void SetPropertyDefaults(DefinedContentItem item, IContent contentItem)
		{
			foreach (PropertyDefault property in item.PropertyDefaults)
			{
				string propertyValue = "";

				switch (property.ValueType)
				{
					case PropertyDefaultValueType.Key:
						propertyValue = GetId(property.Value).ToString();
						break;
					case PropertyDefaultValueType.StaticValue:
						propertyValue = property.Value;
						break;
					default:
						throw new Exception("Unknown property default value type for property " + property.PropertyAlias + " on key " + item.Key);
				}

				contentItem.SetValue(property.PropertyAlias, propertyValue);
			}
		}

		#endregion

		#endregion
	}
}
