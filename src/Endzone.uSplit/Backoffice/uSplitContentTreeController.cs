using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using ClientDependency.Core;
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
            
            var accounts = AccountConfig.GetAll().ToList();
            var account = accounts.FirstOrDefault(x => x.GoogleProfileId == id);
            if (account != null)
            {
                var nodes = AsyncHelpers.RunSync(() => GetTreeNodesForAccountAsync(queryStrings, account));
                return nodes;
            }

            throw new NotSupportedException("Invalid node id");
        }

        private static bool IsRootNode(string id)
        {
            return id == UmbracoConstants.System.Root.ToInvariantString();
        }

        private async Task<TreeNodeCollection> GetTreeNodesAsync(FormDataCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();
            var accounts = AccountConfig.GetAll().ToList();
            if (accounts.Count() > 1)
            {
                foreach (var account in AccountConfig.GetAll())
                {
                    var accountNode = CreateAccountNode(account, queryStrings);
                    nodes.Add(accountNode);
                }
            } else if (accounts.Count() == 1)
            {
                return await GetTreeNodesForAccountAsync(queryStrings, accounts[0]);
            }

            return nodes;
        }
        
        private async Task<TreeNodeCollection> GetTreeNodesForAccountAsync(FormDataCollection queryStrings, AccountConfig config)
        {
            var nodes = new TreeNodeCollection();
            
            string parentId = config.GoogleProfileId;
            
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
        
        private TreeNode CreateAccountNode(AccountConfig config, FormDataCollection queryStrings)
        {
            var name = config.Name;
            if (name.IsNullOrWhiteSpace()) name = config.GoogleProfileId;

            const string icon = Constants.Icons.Account + " color-black";

            var url = $"content/{Constants.Trees.AbTesting}/dashboard/{config.GoogleProfileId}/";
            var node = CreateTreeNode(config.GoogleProfileId, $"{UmbracoConstants.System.Root}", queryStrings, name, icon, url);
            node.HasChildren = true;
            return node;
        }

        private async Task<TreeNodeCollection> CreateExperimentNodes(FormDataCollection queryStrings, string parentId, AccountConfig config)
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
            var name = experiment.GoogleExperiment.Name;

            if (experiment.PageUnderTest != null)
                name = experiment.PageUnderTest.Name;

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

            var accounts = AccountConfig.GetAll().ToList();
            if (IsRootNode(id))
            {
                if (accounts.Count == 1)
                {
                    menu.Items.Add<ActionNew>("Create a new experiment", "profileId", accounts.First().GoogleProfileId);
                }
            }
            else if (accounts.Any(x => x.GoogleProfileId == id)) 
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