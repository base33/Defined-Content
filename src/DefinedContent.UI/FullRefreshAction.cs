using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using umbraco.interfaces;

namespace DefinedContent.UI
{
    public class FullRefreshAction : IAction
    {

        public string Alias
        {
            get { return "Full Refresh"; }
        }

        public bool CanBePermissionAssigned
        {
            get { return false; }
        }

        public string Icon
        {
            get { return "globe"; }
        }

        public string JsFunctionName
        {
            get { return null; }
        }

        public string JsSource
        {
            get { return null; }
        }

        public char Letter
        {
            get { return 'f'; }
        }

        public bool ShowInNotifier
        {
            get { return false; }
        }
    }
}
