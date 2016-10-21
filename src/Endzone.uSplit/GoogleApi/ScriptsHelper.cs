using System.Collections.Generic;
using System.Text;
using Endzone.uSplit.Models;

namespace Endzone.uSplit.GoogleApi
{
    public static class ScriptsHelper
    {
        public static string ReportVariations(IEnumerable<IPublishedContentVariation> variations)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<script src=\"//www.google-analytics.com/cx/api.js\"></script>");
            sb.AppendLine("<script>");
            foreach (var variation in variations)
            {
                sb.AppendLine($"  cxApi.setChosenVariation({variation.GoogleVariationId},'{variation.GoogleExperimentId}');");
            }
            sb.AppendLine("</script>");
            return sb.ToString();
        }
    }
}
