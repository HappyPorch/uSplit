using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Endzone.uSplit.Google;
using Google.Apis.Analytics.v3;
using Google.Apis.Analytics.v3.Data;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Services;
using Umbraco.Web.Mvc;

namespace Endzone.uSplit.Controllers
{
    public class ExperimentsController : UmbracoAuthorizedController
    {

        public async Task<ActionResult> IndexAsync(CancellationToken cancellationToken)
        {
            var result = await new AuthorizationCodeMvcApp(this, new GoogleAuthFlowMetadata()).AuthorizeAsync(cancellationToken);

            if (result.Credential != null)
            {
                var service = new AnalyticsService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = result.Credential,
                    ApplicationName = Constants.ApplicationName
                });

                // YOUR CODE SHOULD BE HERE..
                // SAMPLE CODE:
                var list = service.Management.Experiments.List();
                Experiments experiments = await list.ExecuteAsync(cancellationToken);
                ViewBag.Experiments = experiments;
                return View();
            }
            else
            {
                return new RedirectResult(result.RedirectUri);
            }
        }

    }
}
