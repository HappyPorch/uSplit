using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Endzone.uSplit.GoogleApi;
using Google.Apis.Analytics.v3.Data;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Endzone.uSplit.API
{
    /// <summary>
    /// uSplit JS API
    /// </summary>
    [PluginController(Constants.PluginName)]
    public class ManageController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public async Task<Experiment> GetExperimentAsync(string id)
        {
            var experimentsApi = new ExperimentsApi();
            return await experimentsApi.GetExperimentAsync(id);
        }

        [HttpGet]
        public object CreateVariation()
        {
            throw new NotImplementedException();

            //try
            //{
            //    var content = Services.ContentService.CreateContent("new", -1, "LandingPage");
            //    Services.ContentService.Save(content);
            //    return new
            //    {
            //        ID = content.Id,

            //    };
            //}
            //catch (Exception e)
            //{
            //    return e;
            //}

        }

        [HttpGet]
        public async Task<object> ListVariations(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}