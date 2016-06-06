using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Endzone.uSplit.Commands;
using Endzone.uSplit.GoogleApi;
using Umbraco.Web.Mvc;

namespace Endzone.uSplit.API
{
    [PluginController(Constants.PluginName)]
    public class ConfigurationController : BackofficeController
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

        [HttpGet]
        public HttpResponseMessage License()
        {
            return CreateResponse(new
            {
                HasLicense = LicenseHelper.HasValidLicense(),
                LicenseHelper.FreeTrialExperimentDurationInDays
            });
        }
    }
}