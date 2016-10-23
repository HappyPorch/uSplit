using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Endzone.uSplit.Models;

namespace Endzone.uSplit.GoogleApi
{
    public static class ScriptsHelper
    {
        /// <summary>
        /// Returns a series of analytics.js <code>ga</code> method calls for that report all experiments.
        /// These calls can be optionally redenred inside their own script tag.
        /// Should be rendered after the analytics.js code, for instance where <code>ga('send', 'pageview');</code> is usually called.
        /// </summary>
        public static string AnalyticsJsFragment(IEnumerable<IPublishedContentVariation> variations, bool renderScriptTags = false)
        {
            var sb = new StringBuilder();
            if (renderScriptTags)
            {
                sb.AppendLine("<script>");
            }
            foreach (var variation in variations)
            {
                sb.AppendLine($"ga('set', 'expId', '{variation.GoogleExperimentId}');");
                sb.AppendLine($"ga('set', 'expVar', '{variation.GoogleVariationId}');");
                sb.AppendLine($"ga('send', 'event', 'experiment', 'view');");
                //send custom event to transmit the experiment details with
            }
            if (renderScriptTags)
            {
                sb.AppendLine("</script>");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Includes the www.google-analytics.com/cx/api.js script and sets the variation 
        /// </summary>
        public static string cxApiVariations(IPublishedContentVariation[] variations)
        {
            if (variations.Length > 1)
                //we would need to send a custom event between each setChosenVariation call,
                //but need to know how first - which library is it using? could take a lambda as a param
                throw new NotSupportedException("You may only report a single experiment via the cxApi at the moment.");
            
            var sb = new StringBuilder();
            sb.AppendLine("<script src=\"//www.google-analytics.com/cx/api.js\"></script>");
            sb.AppendLine("<script>");

            foreach (var variation in variations)
            {
                sb.AppendLine($"  cxApi.setChosenVariation({variation.GoogleVariationId},'{variation.GoogleExperimentId}');");
                //todo: send custom event so that we can support multiple of these
            }

            sb.AppendLine("</script>");
            return sb.ToString();
        }
    }
}
