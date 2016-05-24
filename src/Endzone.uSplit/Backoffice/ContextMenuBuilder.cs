using System.Web.Http;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Web.Trees;

namespace Endzone.uSplit.Backoffice
{
    public class ContextMenuBuilder : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            //register the event listener:
            TreeControllerBase.TreeNodesRendering += TreeControllerBase_TreeNodesRendering;
            TreeControllerBase.MenuRendering += TreeControllerBaseOnMenuRendering;
        }

        /// <summary>
        /// Here we can manipulate nodes of other trees
        /// </summary>
        void TreeControllerBase_TreeNodesRendering(TreeControllerBase sender, TreeNodesRenderingEventArgs e)
        {
            //TODO: Generate A/B testing related context menu items for the Cotnent nodes

            //ex below deletes some nodes 
            //if (sender.TreeAlias == "content")
            //{
            //    e.Nodes.RemoveAll(t => t.Name.EndsWith("[VARIATION]"));
            //}
        }

        /// <summary>
        /// Generates a private tree on the content section
        /// </summary>
        private void TreeControllerBaseOnMenuRendering(TreeControllerBase sender, MenuRenderingEventArgs menuRenderingEventArgs)
        {
            //TODO: build our tree
            //TODO: include the files in the plugin

            //if (sender.TreeAlias == "content")
            //{
            //    var menuItem = new MenuItem("abTesting", "A/B Testing")
            //    {
            //        SeperatorBefore = true,
            //        Icon = "split-alt"
            //    };
            //    menuItem.LaunchDialogView("/App_Plugins/uSplit/backoffice/dialog/variations.html", "A/B Testing");
            //    menuRenderingEventArgs.Menu.Items.Add(menuItem);
            //}
        }
    }
}