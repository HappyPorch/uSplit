using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Analytics.v3;
using Google.Apis.Analytics.v3.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.Util;

namespace Endzone.uSplit.GoogleApi
{
    public class ExperimentsApi
    {
        private bool DoWeHaveUsefulToken(TokenResponse token)
        {
            if (uSplitAuthorizationCodeFlow.Instance.ShouldForceTokenRetrieval() || token == null)
                return true;
            if (token.RefreshToken == null)
                return token.IsExpired(SystemClock.Default);
            return false;
        }

        /// <summary>
        /// Indicates whether a valid token exists for the Google API.
        /// </summary>
        public async Task<bool> IsConnected(CancellationToken cancellationToken)
        {
            var uSplitGoogleApiAuth = uSplitAuthorizationCodeFlow.Instance;
            var token = await uSplitGoogleApiAuth.LoadTokenAsync(Constants.Google.SystemUserId, cancellationToken);
            return !DoWeHaveUsefulToken(token);
        }

        public async Task<Experiments> GetExperiments()
        {
            var service = await GetAnalyticsService();
            var list = service.Management.Experiments.List();
            var experiments = await list.ExecuteAsync();
            return experiments;
        }

        private async Task<ICredential> GetCredential()
        {
            var uSplitGoogleApiAuth = uSplitAuthorizationCodeFlow.Instance;
            var token = await uSplitGoogleApiAuth.LoadTokenAsync(Constants.Google.SystemUserId, CancellationToken.None);
            return new UserCredential(uSplitAuthorizationCodeFlow.Instance, Constants.Google.SystemUserId, token);
        }

        private async Task<AnalyticsService> GetAnalyticsService()
        {
            return new AnalyticsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = await GetCredential(),
                ApplicationName = Constants.ApplicationName
            });
        }
    }
}
