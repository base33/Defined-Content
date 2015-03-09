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

		#region Properties

		protected List<DefinedContentItem> ContentItems { get; set; }
		protected Dictionary<string, int> KeyToNodeIdCache { get; set; }

		protected List<DefinedContentItem> AwaitingResolution { get; set; }

		protected UmbracoHelper Umbraco { get; set; }

		#endregion

		#region Constructors

		protected DefinedContent()
		{
			Umbraco = new UmbracoHelper(UmbracoContext.Current);

			FullRefresh();
		}

		#endregion

		#region Public Methods

		public int GetId(string key)
		{
			if (!this.KeyToNodeIdCache.ContainsKey(key))
				throw new Exception("Unknown key " + key);

			return this.KeyToNodeIdCache[key];
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

		/// <summary>
		/// Reloads XML configs, ensures all content exists and rebuilds the defined content cache
		/// </summary>
		public void FullRefresh(UmbracoContext umbracoContext = null)
		{
			if (umbracoContext != null)
				Umbraco = new UmbracoHelper(umbracoContext);

			this.ContentItems = new List<DefinedContentItem>();
			this.AwaitingResolution = new List<DefinedContentItem>();
			this.KeyToNodeIdCache = new Dictionary<string, int>();

			LoadXmlConfigs();
			CreateContent();
			BuildCache(this.ContentItems);
		}

		/// <summary>
		/// Ensures duplicate 
		/// </summary>
		/// <param name="item"></param>
		/// <param name="resolvedNodeId"></param>
		public void AddToCache(DefinedContentItem item, int resolvedNodeId)
		{
			if (this.KeyToNodeIdCache.ContainsKey(item.Key))
				throw new Exception("Duplicate key detected " + item.Key);

			this.KeyToNodeIdCache.Add(item.Key, resolvedNodeId);
		}

		public int? TryGetId(string key)
		{
			if (!this.KeyToNodeIdCache.ContainsKey(key))
				return null;

			return this.KeyToNodeIdCache[key];
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

		private void RecursivelyLoadDirectory(DirectoryInfo configDirectory)
		{
			var xmlFiles = configDirectory.GetFiles("*" + Constants.CONFIG_FILE_EXTENSION);

			foreach (var xmlFile in xmlFiles)
			{
				this.ContentItems.Add(new DefinedContentItem(xmlFile.FullName));
			}

			var subDirs = configDirectory.GetDirectories();

			foreach (var subDir in subDirs)
			{
				RecursivelyLoadDirectory(subDir);
			}
		}

		#endregion

		#region Create Content

		/// <summary>
		/// Ensures all defined content exists 
		/// </summary>
		protected void CreateContent()
		{

		}

		#endregion

		#region Build Cache

		/// <summary>
		/// Builds the defined content cache from already loaded XML configs.
		/// </summary>
		protected void BuildCache(List<DefinedContentItem> contentItems)
		{
			for(int i = 0; i < contentItems.Count; i++)
			{
				ResolveNodeId(contentItems[i]);
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
			switch (item.ResolveType)
			{
				case ResolutionType.ContentId:
					ResolveNodeById(item);
					break;
				case ResolutionType.Key:
					ResolveNodeByKey(item);
					break;
				case ResolutionType.XPath:
					ResolveNodeByXPath(item);
					break;
				default:
					throw new InvalidOperationException("Unknown parentType in Key " + item.Key);
			}
		}

		/// <summary>
		/// Checks a node id exists and adds it to the cache.
		/// </summary>
		/// <param name="item"></param>
		private void ResolveNodeById(DefinedContentItem item)
		{
			IPublishedContent resolvedNode = Umbraco.TypedContent(item.ResolveValue);

			if (resolvedNode == null)
				throw new Exception("Cannot resolve node id for key " + item.Key + ". Invalid node id " + item.ResolveValue);

			AddToCache(item, resolvedNode.Id);
		}

		/// <summary>
		/// Resolves a node id by looking up another key
		/// </summary>
		/// <param name="item">Defined Content Item to match</param>
		private void ResolveNodeByKey(DefinedContentItem item)
		{
			int? nodeId = TryGetId(item.ResolveValue);

			if (!nodeId.HasValue)
			{
				if (!this.AwaitingResolution.Contains(item))
					this.AwaitingResolution.Add(item);

				return;
			}

			if (this.AwaitingResolution.Contains(item))
				this.AwaitingResolution.Remove(item);

			AddToCache(item, this.KeyToNodeIdCache[item.ResolveValue]);
		}

		/// <summary>
		/// Resolves a parent using XPath and adds the first matched child node id to the cache.
		/// </summary>
		/// <param name="item">Defined Content Item to match</param>
		private void ResolveNodeByXPath(DefinedContentItem item)
		{
			var resolvedNode = Umbraco.TypedContentAtXPath(item.ResolveValue);

			if (resolvedNode == null)
				throw new Exception("Cannot resolve node id for key " + item.Key + ". XPath failed to return anything");

			AddToCache(item, resolvedNode.First().Id);
		}

		#endregion
	}
}
