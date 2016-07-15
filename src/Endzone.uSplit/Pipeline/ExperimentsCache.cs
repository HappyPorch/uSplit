using System;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Timers;
using Endzone.uSplit.Commands;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Web;

namespace Endzone.uSplit.Pipeline
{
    public class ExperimentsCache : ApplicationEventHandler
    {
        public static ExperimentsCache Instance;

        private readonly Logger logger;

        public ExperimentsCache()
        {
            Instance = this;
            logger = Logger.CreateWithDefaultLog4NetConfiguration();
            UpdateExperimentsCacheAsync(); //get the latest data
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
            await UpdateExperimentsCacheAsync();
        }

        public async Task UpdateExperimentsCacheAsync()
        {
            try
            {
                //TODO: check if we are configured, otherwise this will generate errors every now and then
                var cache = MemoryCache.Default;
                var experiments = await new GetExperiments().ExecuteAsync();
                cache[Constants.Cache.ExperimentsList] = experiments.Items.ToList();
            }
            catch (Exception ex)
            {
                logger.WarnWithException(typeof(ExperimentsCache), "Failed to download A/B testing data.", ex);
            }
        }
    }
}