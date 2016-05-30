using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Endzone.uSplit.GoogleApi;
using Google.Apis.Analytics.v3.Data;
using Umbraco.Core;
using Experiment = Endzone.uSplit.Models.Experiment;

namespace Endzone.uSplit.Commands
{
    public class StopExperiment : GoogleApiCommand<Experiment>
    {
        public override async Task<Experiment> ExecuteAsync()
        {
            var service = await GetAnalyticsService();
            var googleExperimentRequest = service.Management.Experiments.Get(GoogleExperimentId);
            var googleExperiment = await googleExperimentRequest.ExecuteAsync();

            googleExperiment.Status = "ENDED";

            var request = service.Management.Experiments.Patch(googleExperiment);
            var experiment = await request.ExecuteAsync();

            var cache = MemoryCache.Default;
            var experiments = await new GetCachedExperiments().ExecuteAsync();
            var index = experiments.Items.FindIndex(e => e.Id == experiment.Id);
            if (index >= 0)
            {
                experiments.Items[index] = experiment;
            }
            
            return new Experiment(experiment);
        }

        public string GoogleExperimentId { get; set; }
    }
}