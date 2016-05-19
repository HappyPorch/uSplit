using System.Threading.Tasks;
using System.Web.Configuration;
using Google.Apis.Analytics.v3;

namespace Endzone.uSplit.Google
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


    }
}
