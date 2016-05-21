using System;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using Endzone.uSplit.GoogleApi;
using Umbraco.Core;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;
using UmbracoConstants = Umbraco.Core.Constants;

namespace Endzone.uSplit
{

    /// <summary>
    /// Generates a private tree on the content section
    /// </summary>
    [PluginController(Constants.PluginName)]
    [Tree(UmbracoConstants.Applications.Content, Constants.Trees.AbTesting, "A/B testing")]
    public class USplitContentTreeController : TreeController
    {
        protected override TreeNode CreateRootNode(FormDataCollection queryStrings)
        {
            var node = base.CreateRootNode(queryStrings);

            //node.RoutePath = $"content/{Constants.Trees.AbTesting}/Experiments/IndexAsync/mvc";
            node.RoutePath = $"content/{Constants.Trees.AbTesting}/View/0";
            node.Icon = Constants.Icons.Split;
            node.HasChildren = true;

            return node;
        }

        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            if (id == UmbracoConstants.System.Root.ToInvariantString())
            {
                var nodes = AsyncHelpers.RunSync(() => GetTreeNodesAsync(queryStrings));
                return nodes;
            }

            throw new NotSupportedException("We do not have any children at the moment");
        }

        private async Task<TreeNodeCollection> GetTreeNodesAsync(FormDataCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();

            var experimentsApi = new ExperimentsApi();
            if (!await experimentsApi.IsConnected(CancellationToken.None))
            {
                nodes.Add(CreateTreeNode("error", queryStrings, "ERROR - Google API not connected", "icon-alert"));
            }
            else
            {
                var experiments = await experimentsApi.GetExperiments();
                foreach (var experiment in experiments.Items)
                {
                    nodes.Add(CreateTreeNode(experiment.Id, queryStrings, experiment.Name, Constants.Icons.Split));
                }
            }

            return nodes;
        }

        private TreeNode CreateTreeNode(string id, FormDataCollection queryStrings, string title, string icon)
        {
            var url = $"{Constants.ApplicationAlias}/{Constants.Trees.AbTesting}/{id}/view";
            return CreateTreeNode(id, UmbracoConstants.System.Root.ToInvariantString(), queryStrings, title, icon, false, url);
        }


        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();
            //menu.DefaultMenuAlias = ActionAudit.Instance.Alias;
            //menu.Items.Add<ActionNew>("Create");
            return menu;
        }

    }
}