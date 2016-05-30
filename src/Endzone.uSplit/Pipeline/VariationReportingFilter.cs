using System.Web.Mvc;
using Endzone.uSplit.Models;
using Umbraco.Web;

namespace Endzone.uSplit.Pipeline
{
    public class VariationReportingFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var request = filterContext.RequestContext.HttpContext.GetUmbracoContext().PublishedContentRequest;
            var response = filterContext.HttpContext.Response;
            var variedContent = request?.PublishedContent as VariedContent;
            if (response.ContentType == "text/html" && variedContent != null)
            {
                response.Filter = new InjectVariationReport(response.Filter, variedContent);
            }
        }
    }
}