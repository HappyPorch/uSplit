using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using Google.Apis.Analytics.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util;
using Google.Apis.Util.Store;

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
            DataStore = new FileDataStore($"/TEMP/{Constants.ApplicationAlias}/Analytics.Api.Auth.Store")
        };

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