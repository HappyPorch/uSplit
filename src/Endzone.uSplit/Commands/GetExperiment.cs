using System.Threading.Tasks;
using Endzone.uSplit.GoogleApi;
using Endzone.uSplit.Models;

namespace Endzone.uSplit.Commands
{
    public class GetExperiment : GoogleApiCommand<Experiment>
    {
        public string GoogleExperimentId { get; set; }

        public override async Task<Experiment> ExecuteAsync()
        {
            var service = await GetAnalyticsService();
            var request = service.Management.Experiments.Get(GoogleExperimentId);
            var googleExperiment = await request.ExecuteAsync();
            var experiment = new Experiment(googleExperiment);
            return experiment;
        }
    }
}