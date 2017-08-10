using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Endzone.uSplit.Models;
using Endzone.uSplit.Pipeline;
using Google.Apis.Auth.OAuth2.Mvc;
using Umbraco.Web.Mvc;

namespace Endzone.uSplit.GoogleApi
{
    public class GoogleAuthController : UmbracoAuthorizedController
    {
        public async Task<ActionResult> ReauthorizeAsync(string originalUrl, string profileId, CancellationToken cancellationToken)
        {
            var config = AccountConfig.GetByUniqueId(profileId);
            var flow = uSplitAuthorizationCodeFlow.GetInstance(config);
            if (await flow.IsConnected(cancellationToken))
            {
                await flow.DeleteTokenAsync(Constants.Google.SystemUserId, cancellationToken);
            }

            return RedirectToAction(nameof(AuthorizeAsync), new
            {
                originalUrl,
                profileId
            });
        }

        public async Task<ActionResult> AuthorizeAsync(string originalUrl, string profileId, CancellationToken cancellationToken)
        {
            var config = AccountConfig.GetByUniqueId(profileId);
            var result = await new AuthorizationCodeMvcApp(this, new uSplitFlowMetadata(config)).AuthorizeAsync(cancellationToken);

            if (result.Credential == null)
                //no token, lets go to Google
                return new RedirectResult(result.RedirectUri);

            //refresh the experiments cache
            await ExperimentsUpdater.Instance.UpdateExperimentsCacheAsync();

            //got a token, we can return back
            return new RedirectResult(originalUrl);  
        }
    }
}
