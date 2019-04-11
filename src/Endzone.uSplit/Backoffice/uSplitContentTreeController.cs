using System;
using System.Linq;
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
                var nodes = AsyncHelpers.RunSync(() => GetNodesUnderRootAsync(queryStrings));
                return nodes;
            }
            
            var accounts = AnalyticsAccount.GetAll().ToList();
            var account = accounts.FirstOrDefault(x => x.UniqueId == id);
            if (account != null)
            {
                var nodes = AsyncHelpers.RunSync(() => GetNodesForAccountAsync(account, queryStrings));
                return nodes;
            }

            throw new NotSupportedException("Invalid node id");
        }

        private static bool IsRootNode(string id)
        {
            return id == UmbracoConstants.System.Root.ToInvariantString();
        }

        private async Task<TreeNodeCollection> GetNodesUnderRootAsync(FormDataCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();
            var accounts = AnalyticsAccount.GetAll().ToList();
            if (accounts.Count() > 1)
            {
                foreach (var account in accounts)
                {
                    var accountNode = CreateAccountNode(account, queryStrings);
                    nodes.Add(accountNode);
                }
            }
            else if (accounts.Count() == 1)
            {
                return await GetNodesForAccountAsync(accounts[0], queryStrings);
            }

            return nodes;
        }
        
        private async Task<TreeNodeCollection> GetNodesForAccountAsync(AnalyticsAccount config, FormDataCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();
            
            string parentId = config.UniqueId;
            
            if (!await uSplitAuthorizationCodeFlow.GetInstance(config).IsConnected(CancellationToken.None))
            {
                nodes.Add(CreateTreeNode("error", parentId, queryStrings,
                    "ERROR - Google API not connected", "icon-alert"));
            }
            else
            {
                nodes.AddRange(await CreateExperimentNodes(queryStrings, parentId, config));
            }

            return nodes;
        }
        
        private TreeNode CreateAccountNode(AnalyticsAccount config, FormDataCollection queryStrings)
        {
            var name = config.Name;
            if (name.IsNullOrWhiteSpace()) name = config.UniqueId;

            const string icon = Constants.Icons.Account + " color-black";

            var url = $"content/{Constants.Trees.AbTesting}/dashboard/{config.UniqueId}/";
            var node = CreateTreeNode(config.UniqueId, $"{UmbracoConstants.System.Root}", queryStrings, name, icon, url);
            node.HasChildren = true;
            return node;
        }

        private async Task<TreeNodeCollection> CreateExperimentNodes(FormDataCollection queryStrings, string parentId, AnalyticsAccount config)
        {
            var nodes = new TreeNodeCollection();
            
            try
            {
                var experiments = await ExecuteAsync(new GetExperiments(config));
                foreach (var experiment in experiments.Items)
                {
                    var e = new Experiment(experiment);
                    nodes.Add(CreateExperimentNode(e, queryStrings, parentId));
                }
            }
            catch (Exception ex)
            {
                var messages = ex.Message.Split('\n');
                var message = messages.Length > 1 ? messages[1] : ex.Message;
                nodes.Add(CreateTreeNode("error", parentId, queryStrings, message));
            }

            return nodes;
        }
        

        private TreeNode CreateExperimentNode(Experiment experiment, FormDataCollection queryStrings, string parentId)
        {
            var name = experiment.Name;

            if (experiment.PageUnderTest != null)
                name += " (" + experiment.PageUnderTest.Name + ")";

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
            var url = $"content/{Constants.Trees.AbTesting}/experiment/{experiment.GoogleExperiment.Id}?{parentId}";
            var node = CreateTreeNode(experiment.GoogleExperiment.Id, parentId, queryStrings, name, icon, url);
            return node;
        }


        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();

            var accounts = AnalyticsAccount.GetAll().ToList();
            if (IsRootNode(id))
            {
                if (accounts.Count == 1)
                {
                    menu.Items.Add<ActionNew>("Create a new experiment", "profileId", accounts.First().UniqueId);
                }
            }
            else if (accounts.Any(x => x.UniqueId == id)) 
            {
                menu.Items.Add<ActionNew>("Create a new experiment", "profileId", id);
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