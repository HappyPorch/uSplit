using System.Runtime.Caching;
using System.Threading.Tasks;
using Endzone.uSplit.GoogleApi;
using Google.Apis.Analytics.v3.Data;

namespace Endzone.uSplit.Commands
{
    public class GetCachedExperiments : GoogleApiCommand<Experiments>
    {
        //TODO: Handle the case if the user has over 1000 experiments.
        public override Task<Experiments> ExecuteAsync()
        {
            var cache = MemoryCache.Default;
            var experiments = cache[Constants.Cache.ExperimentsList] as Experiments;

            return Task.FromResult(experiments);
        }
    }
}