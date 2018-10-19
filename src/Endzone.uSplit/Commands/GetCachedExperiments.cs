using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Analytics.v3.Data;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Experiment = Endzone.uSplit.Models.Experiment;
using GoogleExperiment = Google.Apis.Analytics.v3.Data.Experiment;

namespace Endzone.uSplit.Commands
{
    public class GetCachedExperiments : Command<List<Experiment>>
    {
        private IRuntimeCacheProvider Cache => ApplicationContext.Current.ApplicationCache.RuntimeCache;

        public override Task<List<Experiment>> ExecuteAsync()
        {
            var experiments = Cache.GetCacheItem<List<Experiment>>(Constants.Cache.ParsedExperiments, ParseRawExperiments, Constants.Cache.ExperimentsRefreshInterval);
            return Task.FromResult(experiments);
        }

        private List<Experiment> ParseRawExperiments()
        {
            var experiments = new List<Experiment>();
            var rawData = Cache.GetCacheItem<List<GoogleExperiment>>(Constants.Cache.RawExperimentData);
            if (rawData != null)
            {
                experiments.AddRange(rawData.Select(i => new Experiment(i)));
            }
            return experiments;
        }
    }
}