using System;
using System.Threading.Tasks;
using Endzone.uSplit.GoogleApi;
using Endzone.uSplit.Models;
using Experiment = Google.Apis.Analytics.v3.Data.Experiment;

namespace Endzone.uSplit.Commands
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
            var id = $"{node.Name} - {Guid.NewGuid()}";

            googleExperiment.Variations.Add(new Experiment.VariationsData()
            {
                Name = id,
                Url = urlId
            });

            var request = service.Management.Experiments.Patch(googleExperiment);
            await request.ExecuteAsync();
            return new VariationDetails()
            {
                Name = node.Name,
                GoogleName = id,
                NodeId = NodeId
            };
        }

        public string GoogleExperimentId { get; set; }
        public int NodeId { get; set; }
    }
}