using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Endzone.uSplit.GoogleApi;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Endzone.uSplit.API
{
    //TODO: Rename to configuration controller
    [PluginController(Constants.PluginName)]
    public class GoogleApiController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public async Task<bool> Status(CancellationToken cancellationToken)
        {
            return await uSplitAuthorizationCodeFlow.Instance.IsConnected(cancellationToken);
        }
    }
}