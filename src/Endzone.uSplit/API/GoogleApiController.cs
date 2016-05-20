using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Endzone.uSplit.GoogleApi;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Util;
using Google.Apis.Util.Store;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Endzone.uSplit.API
{
    [PluginController(Constants.PluginName)]
    public class GoogleApiController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public async Task<bool> Status(CancellationToken cancellationToken)
        {
            var uSplitGoogleApiAuth = uSplitAuthorizationCodeFlow.Instance;
            var token = await uSplitGoogleApiAuth.LoadTokenAsync(Constants.Google.SystemUserId, cancellationToken);
            return token != null && !token.IsExpired(SystemClock.Default);
        }
    }
}