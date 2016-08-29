using System.Web.Mvc;
using Endzone.uSplit.GoogleApi;
using Endzone.uSplit.Models;

namespace Endzone.uSplit
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Renders two script tags that report the chosen variation to Google Analytics.
        /// These tags are only rendered if the current page is part of an experiment.
        /// This method should be called from the head section of your master layout.
        /// </summary>
        /// <returns>
        /// Two script tags that report variation to Google Analytics, or nothing when
        /// page is not rendered as part of an experiment.
        /// </returns>
        public static MvcHtmlString RenderAbTestingScriptTags(this HtmlHelper helper)
        {
            helper.ViewContext.HttpContext.Items[Constants.VariationReportedHttpContextItemsKey] = true;

            var umbracoContext = helper.ViewContext.HttpContext.GetUmbracoContext();
            var request = umbracoContext.PublishedContentRequest;

            var variedContent = request?.PublishedContent as VariedContent;
            if (variedContent == null)
                return MvcHtmlString.Empty; //this is not a variation, not part of an experiment

            var fragment = VariationReportingJsFragmentGenerator.GetHtml(variedContent.Experiment.Id, variedContent.VariationId);
            return new MvcHtmlString(fragment);
        }
    }
}
