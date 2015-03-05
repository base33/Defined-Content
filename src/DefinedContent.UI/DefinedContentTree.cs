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

namespace DefinedContent.UI
{
    [Tree("settings", "definedContent", "Defined Content", "icon-directions", "icon-directions", true, 3)]
    public class DefinedContentTreeController : TreeController
    {
        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            // check if we're rendering the root node's children
            if (id == Constants.System.Root.ToInvariantString())
            {
                // empty tree
                var tree = new TreeNodeCollection()
                {
                    CreateTreeNode("1", id, new FormDataCollection("somequerystring=2"), "Bob's News Root", "icon-anchor")
                };

                // but if we wanted to add nodes - 
                /*  var tree = new TreeNodeCollection
                {
                    CreateTreeNode("1", id, queryStrings, "My Node 1"), 
                    CreateTreeNode("2", id, queryStrings, "My Node 2"), 
                    CreateTreeNode("3", id, queryStrings, "My Node 3")
                };*/
                return tree;
            }
            // this tree doesn't support rendering more than 1 level
            throw new NotSupportedException();
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            // create my menu item "Import"
            var menu = new MenuItemCollection();

            //// duplicate this section for more than one icon
            var m = new MenuItem("create", "Create");
            m.Icon = "add";
            m.AdditionalData.Add("parent", id);
            menu.Items.Add(m);

            if (id != "-1")
                menu.Items.Add<ActionDelete>("Delete");

            return menu;
        }
    }
}
