using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using Google.Apis.Analytics.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util;
using Google.Apis.Util.Store;
using System.Web.Hosting;

namespace Endzone.uSplit.GoogleApi
{
    public class uSplitAuthorizationCodeFlow : GoogleAuthorizationCodeFlow
    {
        public static readonly uSplitAuthorizationCodeFlow Instance;

        private static readonly Initializer FlowInitializer = new Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = WebConfigurationManager.AppSettings[Constants.AppSettings.GoogleClientId],
                ClientSecret = WebConfigurationManager.AppSettings[Constants.AppSettings.GoogleClientSecret]
            },
            Scopes = new[] {AnalyticsService.Scope.AnalyticsEdit},
            DataStore = new FileDataStore(GetStoragePath(), true)
        };

        private static string GetStoragePath()
        {
            var appName = Constants.ApplicationName;
            var storagePath = HostingEnvironment.MapPath($"~/App_Data/TEMP/{appName}/google/auth");
            return storagePath;
        }

        static uSplitAuthorizationCodeFlow()
        {
            Instance = new uSplitAuthorizationCodeFlow();
        }

        public uSplitAuthorizationCodeFlow() : base(FlowInitializer)
        {
        }

        /// <summary>
        ///     Indicates whether a valid token exists for the Google API.
        /// </summary>
        public async Task<bool> IsConnected(CancellationToken cancellationToken)
        {
            var token = await LoadTokenAsync(Constants.Google.SystemUserId, cancellationToken);
            return !DoWeHaveUsefulToken(token);
        }

        private bool DoWeHaveUsefulToken(TokenResponse token)
        {
            if (Instance.ShouldForceTokenRetrieval() || token == null)
                return true;
            if (token.RefreshToken == null)
                return token.IsExpired(SystemClock.Default);
            return false;
        }
    }
}