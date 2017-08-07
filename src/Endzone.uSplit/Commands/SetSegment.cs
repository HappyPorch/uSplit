using System.Threading.Tasks;
using Endzone.uSplit.GoogleApi;
using Endzone.uSplit.Models;
using OurExperiment = Endzone.uSplit.Models.Experiment;
using Experiment = Google.Apis.Analytics.v3.Data.Experiment;


namespace Endzone.uSplit.Commands
{
    public class SetSegment : GoogleApiCommand<Experiment>
    {
        public string ExperimentId { get; set; }
        public string ProviderKey { get; set; }
        public string Value { get; set; }
        
        public SetSegment(AccountConfig config) : base(config)
        {
        }

        public override async Task<Experiment> ExecuteAsync()
        {
            var service = await GetAnalyticsService();
            var googleExperimentRequest = service.Management.Experiments.Get(Config, ExperimentId);
            var googleExperiment = await googleExperimentRequest.ExecuteAsync();

            var settings = OurExperiment.ParseSettings(googleExperiment.Description);
            settings.SegmentationProviderKey = ProviderKey;
            settings.SegmentationValue = Value;
            googleExperiment.Description = OurExperiment.UpdateSettings(googleExperiment.Description, settings);

            var request = service.Management.Experiments.Patch(Config, googleExperiment);
            return await request.ExecuteAsync();
        }
    }
}
