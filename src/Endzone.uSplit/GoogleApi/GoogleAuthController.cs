using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Google.Apis.Auth.OAuth2.Mvc;
using Umbraco.Web.Mvc;

namespace Endzone.uSplit.GoogleApi
{
    public class GoogleAuthController : UmbracoAuthorizedController
    {
        public async Task<ActionResult> ReauthorizeAsync(string originalUrl, CancellationToken cancellationToken)
        {
            var experiments = new ExperimentsApi();
            if (await experiments.IsConnected(cancellationToken))
            {
                var uSplitGoogleApiAuth = uSplitAuthorizationCodeFlow.Instance;
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
