using System;
using System.Threading.Tasks;
using System.Timers;
using Endzone.uSplit.Commands;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace Endzone.uSplit.Pipeline
{
    /// <remarks>
    /// Runs outside of a HTTP request, cannot use many ASP.NET or Umbraco helpers / contexts
    /// </remarks>
    public class ExperimentsUpdater : ApplicationEventHandler
    {
        public static ExperimentsUpdater Instance;

        private readonly Logger logger;

        public ExperimentsUpdater()
        {
            Instance = this;
            logger = Logger.CreateWithDefaultLog4NetConfiguration();
            UpdateExperimentsCacheAsync(); //get the latest data
        }

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            var cacheTimer = new Timer();
            cacheTimer.Elapsed += CacheTimer_Elapsed;
            cacheTimer.Interval = Constants.Cache.ExperimentsRefreshInterval.TotalMilliseconds;
            cacheTimer.Enabled = true;
            cacheTimer.Start();
        }

        private async void CacheTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await UpdateExperimentsCacheAsync();
        }

        public async Task UpdateExperimentsCacheAsync()
        {
            logger.Info(typeof(ExperimentsUpdater), "Updating experiments data from Google Analytics.");
            try
            {
                //TODO: check if we are configured, otherwise this will generate errors every now and then
                var cache = ApplicationContext.Current.ApplicationCache.RuntimeCache;
                var experiments = await new GetExperiments().ExecuteAsync();
                cache.InsertCacheItem(Constants.Cache.RawExperimentData, () => experiments, Constants.Cache.ExperimentsRefreshInterval);
                cache.ClearCacheItem(Constants.Cache.ParsedExperiments);
            }
            catch (Exception ex)
            {
                logger.Error(typeof(ExperimentsUpdater), "Failed to download A/B testing data.", ex);
            }
        }
    }
}