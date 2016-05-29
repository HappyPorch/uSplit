using System.Linq;
using System.Threading.Tasks;
using Endzone.uSplit.GoogleApi;
using Endzone.uSplit.Models;

namespace Endzone.uSplit.Commands
{
    public class FindExperiment : GoogleApiCommand<Experiment>
    {
        public int PublishedContentId { get; set; }

        public override async Task<Experiment> ExecuteAsync()
        {
            var service = await GetAnalyticsService();
            var request = service.Management.Experiments.List();
            var experiments = await request.ExecuteAsync();

            var experiment =
                experiments.Items
                    .FirstOrDefault(e => Experiment.ExtractNodeIdFromExperimentName(e.Name) == PublishedContentId);

            if (experiment == null)
                return null;

            var experimentData = new Experiment(experiment);
            return experimentData;
        }
    }
}