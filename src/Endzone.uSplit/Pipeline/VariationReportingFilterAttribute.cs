using System.Web;
using System.Web.Mvc;
using Endzone.uSplit.Models;
using Umbraco.Web;
using Umbraco.Web.Routing;

namespace Endzone.uSplit.Pipeline
{
    public class VariationReportingFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //ignore child actions, cannot use filter with them
            if (filterContext.IsChildAction)
                return;

            //are we running an experiment?
            if (!HttpContext.Current.Items.Contains(Constants.HttpContextExperimentKey))
                return;

            var umbracoContext = filterContext.HttpContext.GetUmbracoContext();
            var request = umbracoContext.PublishedContentRequest;
            var response = filterContext.HttpContext.Response;

            //check the license
            var experiment = (Experiment)HttpContext.Current.Items[Constants.HttpContextExperimentKey];
            if (!LicenseHelper.HasValidLicense() && !LicenseHelper.IsCoveredInFreeTrial(experiment.GoogleExperiment))
            {
                response.Filter = new InjectExpiredLicenseWarning(response.Filter);
                return;
            }

            //report the variation to google
            var variedContent = request?.PublishedContent as VariedContent;
            if (response.ContentType == "text/html" && variedContent != null)
            {
                response.Filter = new InjectVariationReport(response.Filter, variedContent);
            }
        }
    }
}