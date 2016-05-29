using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Endzone.uSplit.Commands;
using Endzone.uSplit.GoogleApi;
using Google.Apis.Analytics.v3.Data;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Endzone.uSplit.API
{
    //TODO: Rename to configuration controller
    [PluginController(Constants.PluginName)]
    public class GoogleApiController : BackofficeController
    {
        [HttpGet]
        public async Task<bool> Status(CancellationToken cancellationToken)
        {
            return await uSplitAuthorizationCodeFlow.Instance.IsConnected(cancellationToken);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> CheckAccess(CancellationToken cancellationToken)
        {
            bool hasAccess = false;
            string error = string.Empty;

            try
            {
                hasAccess = await new CheckAccess().ExecuteAsync();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return CreateResponse(new { hasAccess = hasAccess, Error = error });
        }
    }
}