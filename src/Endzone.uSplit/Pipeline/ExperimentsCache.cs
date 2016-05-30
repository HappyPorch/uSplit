using System;
using System.Runtime.Caching;
using System.Timers;
using Endzone.uSplit.Commands;
using Umbraco.Core;

namespace Endzone.uSplit.Pipeline
{
    public class ExperimentsCache : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            var cacheTimer = new Timer();
            cacheTimer.Elapsed += CacheTimer_Elapsed;
            cacheTimer.Interval = TimeSpan.FromSeconds(30).TotalMilliseconds;
            cacheTimer.Enabled = true;
            cacheTimer.Start();
        }

        private async void CacheTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var cache = MemoryCache.Default;
            cache[Constants.Cache.ExperimentsList] = await new GetExperiments().ExecuteAsync();
        }
    }
}