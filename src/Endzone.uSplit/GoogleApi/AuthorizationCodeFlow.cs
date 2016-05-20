using System.Web.Configuration;
using Google.Apis.Analytics.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util.Store;

namespace Endzone.uSplit.GoogleApi
{
    public class uSplitAuthorizationCodeFlow : GoogleAuthorizationCodeFlow
    {
        public static readonly uSplitAuthorizationCodeFlow Instance;

        static uSplitAuthorizationCodeFlow()
        {
            Instance = new uSplitAuthorizationCodeFlow();
        }

        private static readonly Initializer FlowInitializer = new Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = WebConfigurationManager.AppSettings[Constants.AppSettings.GoogleClientId],
                ClientSecret = WebConfigurationManager.AppSettings[Constants.AppSettings.GoogleClientSecret],
            },
            Scopes = new[] {AnalyticsService.Scope.AnalyticsEdit},
            DataStore = new FileDataStore($"/TEMP/{Constants.ApplicationAlias}/Analytics.Api.Auth.Store")
        };

        public uSplitAuthorizationCodeFlow() : base(FlowInitializer)
        {
            
        }
    }
}