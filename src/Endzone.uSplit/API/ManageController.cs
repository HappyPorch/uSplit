using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Endzone.uSplit.Commands;
using Endzone.uSplit.Models;
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
        public async Task<HttpResponseMessage> GetExperimentAsync(string id)
        {
            var experiment = await ExecuteAsync(new GetGoogleExperiment()
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
        public async Task<GoogleExperiment> CreateExperimentAsync(int id)
        {
            return await ExecuteAsync(new CreateExperiment()
            {
                NodeUnderTestId = id
            });
        }

        [HttpPost]
        public async Task<HttpResponseMessage> AddVariationAsync([FromBody]AddVariationRequest request)
        {
            var variationDetails = await ExecuteAsync(new AddVariation()
            {
                GoogleExperimentId = request.ExperimentId,
                NodeId = request.NodeId
            });
            return CreateResponse(variationDetails);
        }

        [HttpDelete]
        public async Task DeleteExperimentAsync(string id)
        {
            //TODO: add an option to delete variations (e.g. Umbraco content linked to it)
            await ExecuteAsync(new DeleteExperiment()
            {
                GoogleExperimentId = id
            });
        }

        [HttpPost]
        public async Task DeleteVariationAsync([FromBody]DeleteVariationRequest request)
        {
            await ExecuteAsync(new DeleteVariation()
            {
                GoogleExperimentId = request.ExperimentId,
                VariationName = request.VariationName
            });
        }

        [HttpPost]
        public async Task<HttpResponseMessage> StartExperimentAsync(string id)
        {
            var experiment = await ExecuteAsync(new StartExperiment()
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
        public async Task<HttpResponseMessage> StopExperimentAsync(string id)
        {
            var experiment = await ExecuteAsync(new StopExperiment()
            {
                GoogleExperimentId = id
            });
            var details = await ExecuteAsync(new GetExperimentDetails()
            {
                Experiment = experiment
            });
            return CreateResponse(details);
        }
    }
}