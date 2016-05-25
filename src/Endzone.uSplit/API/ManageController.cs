using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
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
            var experiment = await ExecuteAsync(new GetExperiment()
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

        public class AddVariationRequest
        {
            public string ExperimentId { get; set; }
            public int NodeId { get; set; }
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
            //TODO: add an option to delete variations
            await ExecuteAsync(new DeleteExperiment()
            {
                GoogleExperimentId = id
            });
        }

    }
}