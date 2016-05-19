using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Endzone.uSplit.API
{

    //TODO: implement a JSON API as opposed to MVC
    /// <summary>
    /// uSplit JS API
    /// </summary>
    [PluginController(Constants.PluginName)]
    public class ManageController : UmbracoAuthorizedApiController
    {
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