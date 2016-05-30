using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Endzone.uSplit.GoogleApi;
using Google.Apis.Analytics.v3;
using Google.Apis.Analytics.v3.Data;
using Experiment = Endzone.uSplit.Models.Experiment;
using GoogleExperiment = Google.Apis.Analytics.v3.Data.Experiment;

namespace Endzone.uSplit.Commands
{
    public class FindExperiment : GoogleApiCommand<GoogleExperiment>
    {
        public int PublishedContentId { get; set; }

        public override async Task<GoogleExperiment> ExecuteAsync()
        {

            //var service = await GetAnalyticsService();
            //var request = service.Management.Experiments.List();
            //var experiments = await request.ExecuteAsync();

            //TODO delete this class
            var cache = MemoryCache.Default;
            var experiments = cache[Constants.Cache.ExperimentsList] as Experiments;

            var experiment =
                experiments?.Items
                    .FirstOrDefault(e => Experiment.ExtractNodeIdFromExperimentName(e.Name) == PublishedContentId);

            return experiment;
        }
    }
}