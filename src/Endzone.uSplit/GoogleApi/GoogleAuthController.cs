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
            var flow = uSplitAuthorizationCodeFlow.Instance;
            if (await flow.IsConnected(cancellationToken))
            {
                await flow.DeleteTokenAsync(Constants.Google.SystemUserId, cancellationToken);
            }

            return RedirectToAction(nameof(AuthorizeAsync), new
            {
                originalUrl
            });
        }

        public async Task<ActionResult> AuthorizeAsync(string originalUrl, CancellationToken cancellationToken)
        {
            var result = await new AuthorizationCodeMvcApp(this, new uSplitFlowMetadata()).AuthorizeAsync(cancellationToken);

            if (result.Credential == null)
                //no token, lets go to Google
                return new RedirectResult(result.RedirectUri);

            //got a token, we can return back
            return new RedirectResult(originalUrl);  
        }
    }
}
