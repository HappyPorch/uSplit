using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Endzone.uSplit.Commands;
using Endzone.uSplit.GoogleApi;
using Endzone.uSplit.Models;
using Endzone.uSplit.Pipeline;
using Umbraco.Web.Mvc;

namespace Endzone.uSplit.API
{
    [PluginController(Constants.PluginName)]
    public class ConfigurationController : BackofficeController
    {
        [HttpGet]
        public async Task<bool> Status(string profileId, CancellationToken cancellationToken)
        {
            var config = AccountConfig.GetByUniqueId(profileId);
            return await uSplitAuthorizationCodeFlow.GetInstance(config).IsConnected(cancellationToken);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> CheckAccess(CancellationToken cancellationToken)
        {
            var response = new List<object>();
            foreach (var config in AccountConfig.GetAll()) {
                bool hasAccess = false;
                bool isConnected = false;
                string error = string.Empty;

                try
                {
                    hasAccess = await new CheckAccess(config).ExecuteAsync();
                    isConnected = await uSplitAuthorizationCodeFlow.GetInstance(config).IsConnected(cancellationToken);
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
                response.Add(new
                {
                    Name = config.Name,
                    HasAccess = hasAccess,
                    IsConnected = isConnected,
                    Error = error,
                    ProfileId = config.UniqueId,
                });
            }
            return CreateResponse(response);
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