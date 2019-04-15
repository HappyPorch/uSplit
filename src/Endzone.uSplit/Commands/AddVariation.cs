using System;
using System.Linq;
using System.Threading.Tasks;
using Endzone.uSplit.GoogleApi;
using Endzone.uSplit.Models;
using Experiment = Google.Apis.Analytics.v3.Data.Experiment;

namespace Endzone.uSplit.Commands
{
    public class AddVariation : GoogleApiCommand<VariationDetails>
    {
        public string GoogleExperimentId { get; set; }
        public string Name { get; set; }
        public int? NodeId { get; set; }
        
        public AddVariation(AnalyticsAccount config) : base(config)
        {
        }
        
        public override async Task<VariationDetails> ExecuteAsync()
        {
            var service = await GetAnalyticsService();
            var googleExperimentRequest = service.Management.Experiments.Get(account, GoogleExperimentId);
            var googleExperiment = await googleExperimentRequest.ExecuteAsync();

            var urlId = NodeId.HasValue? $"{NodeId}" : "SERVER_SIDE";

            if (googleExperiment.Variations.Any(v => v.Name == Name))
            {
                var baseName = Name + " ";
                var counter = 1;
                do
                {
                    Name = baseName + counter++;
                } while (googleExperiment.Variations.Any(v => v.Name == Name));
            }

            googleExperiment.Variations.Add(new Experiment.VariationsData()
            {
                Name = Name,
                Url = urlId
            });

            var request = service.Management.Experiments.Patch(account, googleExperiment);
            await request.ExecuteAsync();
            return new VariationDetails()
            {
                Name = Name,
                GoogleName = Name,
                NodeId = NodeId
            };
        }
    }
}