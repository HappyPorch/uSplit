using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Endzone.uSplit.Commands;
using Endzone.uSplit.GoogleApi;
using Endzone.uSplit.Pipeline;
using ImageProcessor.Common.Extensions;
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
        public async Task<HttpResponseMessage> GetSegmentationProviders()
        {
            var providers = Segmentation.Providers;
            return CreateResponse(from provider in providers
                select new
                {
                    provider.Name,
                    provider.ProviderKey,
                    provider.AngularViewPath
                });
        }
    }
}