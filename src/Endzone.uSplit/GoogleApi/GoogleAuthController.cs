using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Google.Apis.Analytics.v3;
using Google.Apis.Analytics.v3.Data;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Services;
using Google.Apis.Util;
using Umbraco.Web.Mvc;

namespace Endzone.uSplit.GoogleApi
{
    public class GoogleAuthController : UmbracoAuthorizedController
    {
        public async Task<ActionResult> ReauthorizeAsync(string originalUrl, CancellationToken cancellationToken)
        {
            var uSplitGoogleApiAuth = uSplitAuthorizationCodeFlow.Instance;
            var token = await uSplitGoogleApiAuth.LoadTokenAsync(Constants.Google.SystemUserId, cancellationToken);
            var tokenValid = token != null && !token.IsExpired(SystemClock.Default);
            if (tokenValid)
            {
                await uSplitGoogleApiAuth.DeleteTokenAsync(Constants.Google.SystemUserId, cancellationToken);
            }

            return RedirectToAction(nameof(AuthorizeAsync), new
            {
                originalUrl
            });
        }

        public async Task<ActionResult> AuthorizeAsync(string originalUrl, CancellationToken cancellationToken)
        {
            var result = await new AuthorizationCodeMvcApp(this, new uSplitFlowMetadata()).AuthorizeAsync(cancellationToken);

            if (result.Credential != null)
            {
                return new RedirectResult(originalUrl);
            }
            else
            {
                return new RedirectResult(result.RedirectUri);
            }
        }
    }
}
