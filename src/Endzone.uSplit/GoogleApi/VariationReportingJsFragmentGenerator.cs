namespace Endzone.uSplit.GoogleApi
{
    public static class VariationReportingJsFragmentGenerator
    {
        public static string GetHtml(string experimentId, int variationId)
        {
            var html = "<script src=\"//www.google-analytics.com/cx/api.js\"></script>\n" +
                "<script>\n" +
                    $"cxApi.setChosenVariation({variationId},'{experimentId}');\n" +
                "</script>\n";
            return html;
        }
    }
}
