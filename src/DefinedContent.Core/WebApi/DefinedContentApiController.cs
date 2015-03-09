﻿using DefinedContent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Web.WebApi;

namespace DefinedContent.WebApi
{
	public class DefinedContentApiController : UmbracoApiController
	{
		[HttpGet]
		public bool FullRefresh()
		{
			try
			{
				DefinedContent.Current.FullRefresh();

				return true;
			}
			catch
			{
				return false;
			}
		}

		[HttpGet]
		public int GetId(string key)
		{
			return DefinedContent.Current.GetId(key);
		}

		[HttpGet]
		public DefinedContentItem GetDefinedContentItem(string key)
		{
			return DefinedContent.Current.GetDefinedContentItem(key);
		}
	}
}