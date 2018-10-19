using System.Threading.Tasks;
using Endzone.uSplit.GoogleApi;
using Endzone.uSplit.Models;

namespace Endzone.uSplit.Commands
{
    public class GetGoogleExperiment : GoogleApiCommand<Experiment>
    {
        public string GoogleExperimentId { get; set; }
        
        public GetGoogleExperiment(AccountConfig config) : base(config)
        {
        }

        public override async Task<Experiment> ExecuteAsync()
        {
            var service = await GetAnalyticsService();
            var request = service.Management.Experiments.Get(Config, GoogleExperimentId);
            var googleExperiment = await request.ExecuteAsync();
            var experiment = new Experiment(googleExperiment);
            return experiment;
        }
    }
}