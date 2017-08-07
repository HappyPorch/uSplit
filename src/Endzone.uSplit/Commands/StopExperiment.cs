using System.Threading.Tasks;
using Endzone.uSplit.GoogleApi;
using Endzone.uSplit.Models;

namespace Endzone.uSplit.Commands
{
    public class StopExperiment : GoogleApiCommand<Experiment>
    {
        private readonly string _googleExperimentId;

        public StopExperiment(AccountConfig config, string googleExperimentId) : base(config)
        {
            _googleExperimentId = googleExperimentId;
        }
        
        public override async Task<Experiment> ExecuteAsync()
        {
            var service = await GetAnalyticsService();
            var googleExperimentRequest = service.Management.Experiments.Get(Config, _googleExperimentId);
            var googleExperiment = await googleExperimentRequest.ExecuteAsync();

            googleExperiment.Status = "ENDED";

            var request = service.Management.Experiments.Patch(Config, googleExperiment);
            var experiment = await request.ExecuteAsync();

            var parsedExperiment = new Experiment(experiment);

            //update cache
            var experiments = await new GetCachedExperiments().ExecuteAsync();
            var index = experiments.FindIndex(e => e.Id == experiment.Id);
            if (index >= 0)
            {
                experiments[index] = parsedExperiment;
            }
            
            return parsedExperiment;
        }
    }
}