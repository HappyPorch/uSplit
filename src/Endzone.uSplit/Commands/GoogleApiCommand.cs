using System.Threading;
using System.Threading.Tasks;
using Endzone.uSplit.GoogleApi;
using Endzone.uSplit.Models;
using Google.Apis.Analytics.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;

namespace Endzone.uSplit.Commands
{
    public abstract class GoogleApiCommand<TOut> : Command<TOut>
    {
        protected readonly AnalyticsAccount account;

        protected GoogleApiCommand(AnalyticsAccount account)
        {
            this.account = account;
        }

        protected async Task<AnalyticsService> GetAnalyticsService()
        {
            return new AnalyticsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = await GetCredential(),
                ApplicationName = Constants.ApplicationName
            });
        }

        protected async Task<ICredential> GetCredential()
        {
            var uSplitGoogleApiAuth = uSplitAuthorizationCodeFlow.GetInstance(account);
            var token = await uSplitGoogleApiAuth.LoadTokenAsync(Constants.Google.SystemUserId, CancellationToken.None);
            return new UserCredential(uSplitGoogleApiAuth, Constants.Google.SystemUserId, token);
        }
    }
}