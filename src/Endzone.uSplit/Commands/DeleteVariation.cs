using System;
using System.Linq;
using System.Threading.Tasks;
using Endzone.uSplit.GoogleApi;
using Endzone.uSplit.Models;
using Experiment = Google.Apis.Analytics.v3.Data.Experiment;

namespace Endzone.uSplit.Commands
{
    public class DeleteVariation : GoogleApiCommand<Experiment>
    {
        public string GoogleExperimentId { get; set; }
        public string VariationName { get; set; }
        
        public DeleteVariation(AnalyticsAccount config) : base(config)
        {
        }
        
        public override async Task<Experiment> ExecuteAsync()
        {
            var service = await GetAnalyticsService();
            var googleExperimentRequest = service.Management.Experiments.Get(account, GoogleExperimentId);
            var googleExperiment = await googleExperimentRequest.ExecuteAsync();

            var variation = googleExperiment.Variations.FirstOrDefault(v => v.Name.Equals(VariationName));
            if (variation == null)
                throw new InvalidOperationException($"Variation {VariationName} not found on experiment {GoogleExperimentId}.");

            googleExperiment.Variations.Remove(variation);

            var request = service.Management.Experiments.Patch(account, googleExperiment);
            var response = await request.ExecuteAsync();

            return response;
        }
    }
}