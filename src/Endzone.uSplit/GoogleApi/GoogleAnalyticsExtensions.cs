using Endzone.uSplit.Models;
using Google.Apis.Analytics.v3;
using GoogleExperiment = Google.Apis.Analytics.v3.Data.Experiment;

namespace Endzone.uSplit.GoogleApi
{
    public static class GoogleAnalyticsExtensions
    {
        public static ManagementResource.ExperimentsResource.ListRequest List(
            this ManagementResource.ExperimentsResource experiments, AccountConfig config)
        {
            var accountId = config.GoogleAccountId;
            var webPropertyId = config.GoogleWebPropertyId;
            var profileId = config.GoogleProfileId;
            return experiments.List(accountId, webPropertyId, profileId);
        }

        public static ManagementResource.ExperimentsResource.GetRequest Get(this ManagementResource.ExperimentsResource experiments, AccountConfig config, string experimentId)
        {
            var accountId = config.GoogleAccountId;
            var webPropertyId = config.GoogleWebPropertyId;
            var profileId = config.GoogleProfileId;
            return experiments.Get(accountId, webPropertyId, profileId, experimentId);
        }

        public static ManagementResource.ExperimentsResource.InsertRequest Insert(this ManagementResource.ExperimentsResource experiments, AccountConfig config, GoogleExperiment experiment)
        {
            var accountId = config.GoogleAccountId;
            var webPropertyId = config.GoogleWebPropertyId;
            var profileId = config.GoogleProfileId;
            return experiments.Insert(experiment, accountId, webPropertyId, profileId);
        }

        public static ManagementResource.ExperimentsResource.PatchRequest Patch(this ManagementResource.ExperimentsResource experiments, AccountConfig config, GoogleExperiment experiment)
        {
            var accountId = config.GoogleAccountId;
            var webPropertyId = config.GoogleWebPropertyId;
            var profileId = config.GoogleProfileId;
            return experiments.Patch(experiment, accountId, webPropertyId, profileId, experiment.Id);
        }

        public static ManagementResource.ExperimentsResource.DeleteRequest Delete(this ManagementResource.ExperimentsResource experiments, AccountConfig config, string experimentId)
        {
            var accountId = config.GoogleAccountId;
            var webPropertyId = config.GoogleWebPropertyId;
            var profileId = config.GoogleProfileId;
            return experiments.Delete(accountId, webPropertyId, profileId, experimentId);
        }
    }
}
