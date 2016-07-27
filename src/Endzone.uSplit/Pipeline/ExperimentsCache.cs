using System;
using System.Linq;
using System.Runtime.Caching;
using System.Timers;
using Endzone.uSplit.Commands;
using Endzone.uSplit.Models;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace Endzone.uSplit.Pipeline
{
    public class ExperimentsCache : ApplicationEventHandler
    {
        private readonly Logger logger;

        public ExperimentsCache()
        {
            logger = Logger.CreateWithDefaultLog4NetConfiguration();
            CacheTimer_Elapsed(null, null); //get the latest data
        }

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            var cacheTimer = new Timer();
            cacheTimer.Elapsed += CacheTimer_Elapsed;
            cacheTimer.Interval = TimeSpan.FromHours(1).TotalMilliseconds;
            cacheTimer.Enabled = true;
            cacheTimer.Start();
        }

        private async void CacheTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                //TODO: check if we are configured, otherwise this will generate errors every now and then
                var cache = MemoryCache.Default;
                var experiments = await new GetExperiments().ExecuteAsync();
                cache[Constants.Cache.ExperimentsList] = experiments.Items.Select(i => new Experiment(i)).ToList();
            }
            catch (Exception ex)
            {
                logger.WarnWithException(typeof(ExperimentsCache), "Failed to download A/B testing data.", ex);
            }
        }
    }
}