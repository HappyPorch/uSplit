using System.Runtime.Caching;
using System.Threading.Tasks;
using Endzone.uSplit.GoogleApi;
using Endzone.uSplit.Models;

namespace Endzone.uSplit.Commands
{
    public class StartExperiment : GoogleApiCommand<Experiment>
    {
        public string GoogleExperimentId { get; set; }

        public StartExperiment(AccountConfig config) : base(config)
        {
        }
        
        public override async Task<Experiment> ExecuteAsync()
        {
            var service = await GetAnalyticsService();
            var googleExperimentRequest = service.Management.Experiments.Get(Config, GoogleExperimentId);
            var googleExperiment = await googleExperimentRequest.ExecuteAsync();

            googleExperiment.Status = "RUNNING";
            if (string.IsNullOrEmpty(googleExperiment.ObjectiveMetric))
            {
                googleExperiment.ObjectiveMetric = "ga:pageviews";
            }

            var request = service.Management.Experiments.Patch(Config, googleExperiment);
            var experiment = await request.ExecuteAsync();

            var parsedExperiment = new Experiment(experiment);

            //update cache
            var experiments = await new GetCachedExperiments().ExecuteAsync();
            experiments.Add(parsedExperiment);

            return parsedExperiment;
        }
    }
}