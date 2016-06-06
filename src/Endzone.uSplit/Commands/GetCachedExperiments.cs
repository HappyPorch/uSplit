using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Endzone.uSplit.GoogleApi;
using Google.Apis.Analytics.v3.Data;

namespace Endzone.uSplit.Commands
{
    public class GetCachedExperiments : GoogleApiCommand<List<Experiment>>
    {
        public override Task<List<Experiment>> ExecuteAsync()
        {
            var cache = MemoryCache.Default;
            var experiments = cache[Constants.Cache.ExperimentsList] as List<Experiment>;
            if (experiments == null)
            {
                experiments = new List<Experiment>();
                //TODO: fix a race condition if the cache update code updates the cache before this line
                cache[Constants.Cache.ExperimentsList] = experiments;
            }

            return Task.FromResult(experiments);
        }
    }
}