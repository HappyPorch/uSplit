using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Analytics.v3.Data;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Experiment = Endzone.uSplit.Models.Experiment;

namespace Endzone.uSplit.Commands
{
    public class GetApplicableCachedExperiments : Command<IEnumerable<Experiment>>
    {
        private IRuntimeCacheProvider Cache => ApplicationContext.Current.ApplicationCache.RuntimeCache;

        public int ContentId { get; set; }


        public override Task<IEnumerable<Experiment>> ExecuteAsync()
        {
            var experiments = Cache.GetCacheItem<List<Experiment>>(Constants.Cache.ParsedExperiments, ParseRawExperiments, Constants.Cache.ExperimentsRefreshInterval);
            if (experiments == null)
                return Task.FromResult(Enumerable.Empty<Experiment>());

            return Task.FromResult(from experiment in experiments
                where experiment.IsUSplitExperiment && experiment.IsRunning
                where experiment.PageUnderTest.Id == ContentId
                select experiment);
        }



        private List<Experiment> ParseRawExperiments()
        {
            var experiments = new List<Experiment>();
            var rawData = Cache.GetCacheItem<Experiments>(Constants.Cache.RawExperimentData);
            if (rawData != null)
            {
                experiments.AddRange(rawData.Items.Select(i => new Experiment(i)));
            }
            return experiments;
        }
    }
}