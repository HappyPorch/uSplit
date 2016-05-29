using System.Linq;
using System.Threading.Tasks;
using Endzone.uSplit.GoogleApi;
using Endzone.uSplit.Models;
using GoogleExperiment = Google.Apis.Analytics.v3.Data.Experiment;

namespace Endzone.uSplit.Commands
{
    public class FindExperiment : GoogleApiCommand<GoogleExperiment>
    {
        public int PublishedContentId { get; set; }

        public override async Task<GoogleExperiment> ExecuteAsync()
        {
            var service = await GetAnalyticsService();
            var request = service.Management.Experiments.List();
            var experiments = await request.ExecuteAsync();

            var experiment =
                experiments.Items
                    .FirstOrDefault(e => Experiment.ExtractNodeIdFromExperimentName(e.Name) == PublishedContentId);

            return experiment;
        }
    }
}