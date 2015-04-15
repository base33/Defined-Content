using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using umbraco.BusinessLogic.Actions;
using Umbraco.Core;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Trees;
using RealUmbraco = Umbraco;

namespace DefinedContent.UI
{
    [Tree("settings", "definedContent", "Defined Content", "icon-directions", "icon-directions", true, 3)]
    public class DefinedContentTreeController : TreeController
    {
        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            // check if we're rendering the root node's children
            var items = id == RealUmbraco.Core.Constants.System.Root.ToInvariantString()
                ? DefinedContent.Cache.GetRootDefinedContentItems()
                : DefinedContent.Cache.GetDefinedContentItem(id).Children;
            
            // empty tree
            //var tree = new TreeNodeCollection()
            //{
            //    CreateTreeNode("Bobs Something", id, new FormDataCollection("somequerystring=2"), "Bob's News Root", "icon-anchor")
            //};

            var tree = new TreeNodeCollection();
            foreach (var item in items)
            {
                tree.Add(CreateTreeNode(item.Key, item.Key, null, item.Key, "icon-anchor", item.Children.Any()));
            }

            // but if we wanted to add nodes - 
            /*  var tree = new TreeNodeCollection
            {
                CreateTreeNode("1", id, queryStrings, "My Node 1"), 
                CreateTreeNode("2", id, queryStrings, "My Node 2"), 
                CreateTreeNode("3", id, queryStrings, "My Node 3")
            };*/
            return tree;
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            // create my menu item "Import"
            var menu = new MenuItemCollection();

            //// duplicate this section for more than one icon
            var m = new MenuItem("create", "Create");
            m.Icon = "add";
            m.NavigateToRoute("/settings/definedContent/create/" + id);
            m.AdditionalData.Add("DefinedContentParent", id == "-1" ? "" : id);
            menu.Items.Add(m);

            if (id != "-1")
            {
                menu.Items.Add<ActionDelete>("Delete", additionalData: new Dictionary<string, object> { { "id", id } });
            }

            menu.Items.Add<ActionRefresh>("Refresh");
            return menu;
        }
    }
}
