using System.Web.Configuration;
using Google.Apis.Analytics.v3;

namespace Endzone.uSplit.GoogleApi
{
    public static class GoogleAnalyticsExtensions
    {
        public static ManagementResource.ExperimentsResource.ListRequest List(
            this ManagementResource.ExperimentsResource experiments)
        {
            var accountId = WebConfigurationManager.AppSettings[Constants.AppSettings.GoogleAccountId];
            var webPropertyId = WebConfigurationManager.AppSettings[Constants.AppSettings.GoogleWebPropertyId];
            var profileId = WebConfigurationManager.AppSettings[Constants.AppSettings.GoogleProfileId];
            return experiments.List(accountId, webPropertyId, profileId);
        }

        public static ManagementResource.ExperimentsResource.GetRequest Get(this ManagementResource.ExperimentsResource experiments, string experimentId)
        {
            var accountId = WebConfigurationManager.AppSettings[Constants.AppSettings.GoogleAccountId];
            var webPropertyId = WebConfigurationManager.AppSettings[Constants.AppSettings.GoogleWebPropertyId];
            var profileId = WebConfigurationManager.AppSettings[Constants.AppSettings.GoogleProfileId];
            return experiments.Get(accountId, webPropertyId, profileId, experimentId);
        }

    }
}
