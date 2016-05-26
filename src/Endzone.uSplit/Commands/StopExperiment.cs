using System.Threading.Tasks;
using Endzone.uSplit.GoogleApi;
using Endzone.uSplit.Models;

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
            return new Experiment(experiment);
        }

        public string GoogleExperimentId { get; set; }
    }
}