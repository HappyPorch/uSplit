using System.Linq;
using System.Threading.Tasks;
using Endzone.uSplit.Commands;
using Endzone.uSplit.GoogleApi;
using Endzone.uSplit.Models;
using umbraco.NodeFactory;
using Experiment = Google.Apis.Analytics.v3.Data.Experiment;

namespace Endzone.uSplit.API
{
    public class AddVariation : GoogleApiCommand<VariationDetails>
    {
        public override async Task<VariationDetails> ExecuteAsync()
        {
            var service = await GetAnalyticsService();
            var googleExperimentRequest = service.Management.Experiments.Get(GoogleExperimentId);
            var googleExperiment = await googleExperimentRequest.ExecuteAsync();

            var node = UmbracoContext.Application.Services.ContentService.GetById(NodeId);
            var urlId = $"{NodeId}";

            googleExperiment.Variations.Add(new Experiment.VariationsData()
            {
                Name = node.Name,
                Url = urlId
            });

            var request = service.Management.Experiments.Patch(googleExperiment);
            var response = await request.ExecuteAsync();
            var variation = response.Variations.First(v => v.Url == urlId);
            return new VariationDetails()
            {
                Name = variation.Name,
                NodeId = NodeId
            };
        }

        public string GoogleExperimentId { get; set; }
        public int NodeId { get; set; }
    }
}