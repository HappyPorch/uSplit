using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Examine.LuceneEngine.SearchCriteria;
using umbraco.presentation.umbraco.translation;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Endzone.uSplit.API
{
    /// <summary>
    /// uSplit JS API
    /// </summary>
    [PluginController("uSplitPersonalisationGroups")]
    public class SegmentsController : BackofficeController
    {
        //[HttpGet]
        //public async Task<HttpResponseMessage> GetAsync()
        //{
        //    var segments = Umbraco.TypedContentAtRoot().SelectMany(c => c.DescendantsOrSelf().Where(d => d.DocumentTypeAlias == Zone.UmbracoPersonalisationGroups.AppConstants.))
        //    return CreateResponse(details);
        //}

        //[HttpGet]
        //public async Task<HttpResponseMessage> GetPicker()
        //{
        //    var segments = Umbraco.TypedContentAtRoot().SelectMany(c => c.DescendantsOrSelf().Where(d => d.DocumentTypeAlias == Zone.UmbracoPersonalisationGroups.AppConstants.))
        //    return CreateResponse(details);
        //}
    }
}