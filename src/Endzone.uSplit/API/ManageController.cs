using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Endzone.uSplit.Commands;
using Endzone.uSplit.Models;
using Umbraco.Core;
using Umbraco.Web.Mvc;
using GoogleExperiment = Google.Apis.Analytics.v3.Data.Experiment;

namespace Endzone.uSplit.API
{
    /// <summary>
    /// uSplit JS API
    /// </summary>
    [PluginController(Constants.PluginName)]
    public class ManageController : BackofficeController
    {
        [HttpGet]
        public async Task<HttpResponseMessage> GetExperimentAsync(string id, string profileId)
        {
            var experiment = await ExecuteAsync(new GetGoogleExperiment(AccountConfig.GetByUniqueId(profileId))
            {
                GoogleExperimentId = id
            });
            var details = await ExecuteAsync(new GetExperimentDetails()
            {
                Experiment = experiment
            });
            return CreateResponse(details);
        }

        [HttpGet]
        public async Task<GoogleExperiment> CreateExperimentAsync(int id, string profileId)
        {
            return await ExecuteAsync(new CreateExperiment(AccountConfig.GetByUniqueId(profileId))
            {
                NodeUnderTestId = id
            });
        }

        [HttpPost]
        public async Task<HttpResponseMessage> AddVariationAsync(string profileId, [FromBody]AddVariationRequest request)
        {
            var variationDetails = await ExecuteAsync(new AddVariation(AccountConfig.GetByUniqueId(profileId))
            {
                GoogleExperimentId = request.ExperimentId,
                NodeId = request.NodeId
            });
            return CreateResponse(variationDetails);
        }

        [HttpDelete]
        public async Task DeleteExperimentAsync(string id, string profileId)
        {
            var accountConfigs = AccountConfig.GetAll().ToList();
            AccountConfig config;
            if ((id.IsNullOrWhiteSpace() || id == "-1") && accountConfigs.Count == 1)
            {
                config = accountConfigs.First();
            }
            else
            {
                config = AccountConfig.GetByUniqueId(profileId);                
            }
            
            
            //TODO: add an option to delete variations (e.g. Umbraco content linked to it)
            await ExecuteAsync(new DeleteExperiment(config)
            {
                GoogleExperimentId = id
            });
        }

        [HttpPost]
        public async Task DeleteVariationAsync(string profileId, [FromBody]DeleteVariationRequest request)
        {
            await ExecuteAsync(new DeleteVariation(AccountConfig.GetByUniqueId(profileId))
            {
                GoogleExperimentId = request.ExperimentId,
                VariationName = request.VariationName
            });
        }

        [HttpPost]
        public async Task<HttpResponseMessage> StartExperimentAsync(string id, string profileId)
        {
            var experiment = await ExecuteAsync(new StartExperiment(AccountConfig.GetByUniqueId(profileId))
            {
                GoogleExperimentId = id
            });
            var details = await ExecuteAsync(new GetExperimentDetails()
            {
                Experiment = experiment
            });
            return CreateResponse(details);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> StopExperimentAsync(string id, string profileId)
        {
            var experiment = await ExecuteAsync(new StopExperiment(AccountConfig.GetByUniqueId(profileId), id));
            var details = await ExecuteAsync(new GetExperimentDetails()
            {
                Experiment = experiment
            });
            return CreateResponse(details);
        }

        [HttpPost]
        public async Task<IHttpActionResult> SetSegmentAsync(string profileId, [FromBody]SetSegmentRequest request)
        {
            await ExecuteAsync(new SetSegment(AccountConfig.GetByUniqueId(profileId)) {
                ExperimentId = request.ExperimentId,
                ProviderKey = request.ProviderKey,
                Value = request.Value
            });
            return Ok();
        }
    }
}