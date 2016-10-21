using System;
using System.Web.Mvc;
using Endzone.uSplit.Models;
using Umbraco.Core.Logging;

namespace Endzone.uSplit.Pipeline
{
    public class VariationReportingActionFilterAttribute : ActionFilterAttribute
    {
        private readonly Logger logger;

        private const string FilteringDoesntWork =
            "uSplit cannot insert a required JS fragment (to report chosen A/B testing variation) to your page " +
            "automatically. Add a call to @Html.RenderAbTestingScriptTags() into your master layout " +
            "to manually include this JS snippet and suppress this error.";

        public VariationReportingActionFilterAttribute()
        {
            logger = Logger.CreateWithDefaultLog4NetConfiguration();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            try
            {
                //ignore child actions, cannot use filter with them
                if (filterContext.IsChildAction)
                    return;

                var httpContext = filterContext.HttpContext;
                var umbracoContext = httpContext.GetUmbracoContext();
                var request = umbracoContext?.PublishedContentRequest;
                if (request == null)
                    return; //not a request for an umbraco page

                var response = httpContext.Response;
                if (response.ContentType != "text/html")
                    return; //we only know how to report from JavaScript, so we need to be serving an HTML page

                var variedContent = request.PublishedContent as VariedContent;
                if (variedContent == null)
                    return; //this is not a variation, not part of an experiment

                Func<bool> scriptsNeeded = () => Equals(filterContext.HttpContext.Items[Constants.VariationReportedHttpContextItemsKey], true);

                response.Filter = new VariationReportingHttpResponseFilter(response.Filter, variedContent, scriptsNeeded);
            }
            catch (Exception e)
            {
                //There are instances where filtering simply doesn't work
                //https://github.com/EndzoneSoftware/uSplit/issues/4

                logger.Error(GetType(), "An exception has been throw when uSplit tried to report a variation to Google Analytics." +
                                        "This may happen due to incompatibility with thrid party libraries or custom code.", e);

                if (!filterContext.HttpContext.Items.Contains(Constants.VariationReportedHttpContextItemsKey))
                    filterContext.HttpContext.Items[Constants.VariationReportedHttpContextItemsKey] = false;

            }
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var variationReported = (bool) (filterContext.HttpContext.Items[Constants.VariationReportedHttpContextItemsKey] ?? true);

            if (!variationReported)
                throw new NotSupportedException(FilteringDoesntWork);
        }
    }
}