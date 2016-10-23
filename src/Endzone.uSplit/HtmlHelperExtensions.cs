using System;
using System.Web.Mvc;
using Endzone.uSplit.GoogleApi;
using Endzone.uSplit.Models;

namespace Endzone.uSplit
{
    public static class HtmlHelperExtensions
    {
        public class AbTestingScriptBuilder
        {
            private HtmlHelper helper;

            public AbTestingScriptBuilder(HtmlHelper helper)
            {
                this.helper = helper;
            }

            private VariedContent Init()
            {
                helper.ViewContext.HttpContext.Items[Constants.VariationReportedHttpContextItemsKey] = true;

                var umbracoContext = helper.ViewContext.HttpContext.GetUmbracoContext();
                var request = umbracoContext.PublishedContentRequest;

                return request?.PublishedContent as VariedContent;
            }

            /// <summary>
            /// Renders a series of analytics.js JavaScript method calls that report the
            /// chosen variations to Google Analytics. Nothing is rendered if the current
            /// page is not part of an experiment.
            /// 
            /// This method should be called inside a  script block in your master Layout,
            /// right after where <code>ga('send', 'pageview');</code> usually goes.
            /// 
            /// Calling this method supresses automatic reporting for this request.
            /// </summary>
            public MvcHtmlString AnalyticsJsCode()
            {
                var variedContent = Init();
                if (variedContent == null)
                    return MvcHtmlString.Empty; //this is not a variation, not part of an experiment

                var fragment = ScriptsHelper.AnalyticsJsFragment(variedContent.AppliedVariations);
                return new MvcHtmlString(fragment);
            }

            /// <summary>
            /// Renders a script tag that contains a series of analytics.js JavaScript
            /// method calls reports any chosen variations to Google Analytics. This tag
            /// is only rendered if the current page is part of an experiment.
            /// 
            /// This method should be called after you include your analytics.js script.
            /// 
            /// Calling this method supresses automatic reporting for this request.
            /// </summary>
            public MvcHtmlString AnalyticsJsScriptTag()
            {
                var variedContent = Init();

                if (variedContent == null)
                    return MvcHtmlString.Empty; //this is not a variation, not part of an experiment

                var fragment = ScriptsHelper.AnalyticsJsFragment(variedContent.AppliedVariations, true);
                return new MvcHtmlString(fragment);
            }

            /// <summary>
            /// Renders two script tags that report the chosen variation to Google Analytics.
            /// These tags are only rendered if the current page is part of an experiment.
            /// This method should be called from the head section of your master layout.
            /// 
            /// This method of reporting supports running only a single experiment at a time.
            /// 
            /// Calling this method supresses automatic reporting for this request.
            /// </summary>
            public MvcHtmlString CxApiScriptTags()
            {
                var variedContent = Init();
                if (variedContent == null)
                    return MvcHtmlString.Empty; //this is not a variation, not part of an experiment

                var fragment = ScriptsHelper.cxApiVariations(variedContent.AppliedVariations);
                return new MvcHtmlString(fragment);
            }

            /// <summary>
            /// Leaves it up to you to report the variations to Google Analytics. You will
            /// receive all applicable variations for the current page; zero if there are
            /// experiments defined but the user does not participate in any.
            /// 
            /// Calling this method supresses automatic reporting for this request.
            /// </summary>
            public MvcHtmlString Custom(Func<IPublishedContentVariation[], string> customTemplate)
            {
                var variedContent = Init();
                if (variedContent == null)
                    return MvcHtmlString.Empty; //this is not a variation, not part of an experiment

                var fragment = customTemplate(variedContent.AppliedVariations);
                return new MvcHtmlString(fragment);
            }
        }

        /// <summary>
        /// Use this fluent API to percielsy control how are chosen A/B testing variations
        /// reported to Google Analytics. Find anything missing? Report feature requests at
        /// https://github.com/EndzoneSoftware/uSplit/issues
        /// </summary>
        public static AbTestingScriptBuilder AbTesting(this HtmlHelper helper)
            => new AbTestingScriptBuilder(helper);

        public static MvcHtmlString RenderAbTestingScriptTags(this HtmlHelper helper)
        {
            return AbTesting(helper).AnalyticsJsScriptTag();
        }
    }
}
