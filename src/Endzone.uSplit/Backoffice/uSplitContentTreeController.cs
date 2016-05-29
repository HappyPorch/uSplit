using System;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using Endzone.uSplit.Commands;
using Endzone.uSplit.GoogleApi;
using Endzone.uSplit.Models;
using umbraco.BusinessLogic.Actions;
using Umbraco.Core;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;
using UmbracoConstants = Umbraco.Core.Constants;

namespace Endzone.uSplit.Backoffice
{

    /// <summary>
    /// Generates a private tree on the content section
    /// </summary>
    [PluginController(Constants.PluginName)]
    [Tree(UmbracoConstants.Applications.Content, Constants.Trees.AbTesting, "A/B testing")]
    public class USplitContentTreeController : uSplitTreeController
    {
        protected override TreeNode CreateRootNode(FormDataCollection queryStrings)
        {
            var node = base.CreateRootNode(queryStrings);

            node.RoutePath = $"content/{Constants.Trees.AbTesting}/dashboard/hello";
            node.Icon = Constants.Icons.Split;
            node.HasChildren = true;

            return node;
        }

        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            if (IsRootNode(id))
            {
                var nodes = AsyncHelpers.RunSync(() => GetTreeNodesAsync(queryStrings));
                return nodes;
            }

            throw new NotSupportedException("We do not have any children at the moment");
        }

        private static bool IsRootNode(string id)
        {
            return id == UmbracoConstants.System.Root.ToInvariantString();
        }

        private async Task<TreeNodeCollection> GetTreeNodesAsync(FormDataCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();

            if (!await uSplitAuthorizationCodeFlow.Instance.IsConnected(CancellationToken.None))
            {
                nodes.Add(CreateTreeNode("error", $"{UmbracoConstants.System.Root}", queryStrings, "ERROR - Google API not connected", "icon-alert"));
            }
            else
            {
                var experiments = await ExecuteAsync(new GetExperiments());
                foreach (var experiment in experiments.Items)
                {
                    var e = new Experiment(experiment);
                    nodes.Add(CreateExperimentNode(e, queryStrings));
                }
            }

            return nodes;
        }

        private TreeNode CreateExperimentNode(Experiment experiment, FormDataCollection queryStrings)
        {
            var name = experiment.GoogleExperiment.Name;
            var id = Experiment.ExtractNodeIdFromExperimentName(name);
            if (id.HasValue)
                name = Services.ContentService.GetById(id.Value).Name;
            string icon;
            switch (experiment.GoogleExperiment.Status)
            {
                case "DRAFT":
                case "READY_TO_RUN":
                    icon = Constants.Icons.Autofill + " color-yellow";
                    break;
                case "RUNNING":
                    icon = Constants.Icons.Play + " color-green";
                    break;
                case "ENDED":
                    icon = Constants.Icons.FlagAlt + " color-red";
                    break;
                default:
                    icon = Constants.Icons.Block + " color-red";
                    break;
            }
            var url = $"content/{Constants.Trees.AbTesting}/experiment/{experiment.GoogleExperiment.Id}";
            return CreateTreeNode(experiment.GoogleExperiment.Id, $"{UmbracoConstants.System.Root}", queryStrings, name, icon, url);
        }


        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();

            if (IsRootNode(id))
            {
                menu.Items.Add<ActionNew>("Create a new experiment");
            }
            else //experiment node
            {
                //TODO: Nice-to-haves
                //menu.Items.Add<ActionPublish>("Start this experiment");
                //menu.Items.Add<ActionDisable>("Stop this experiment");
                menu.Items.Add<ActionDelete>("Delete this experiment");
            }

            return menu;
        }

    }
}