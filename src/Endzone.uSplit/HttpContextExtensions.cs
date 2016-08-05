using System;
using System.Web;
using Umbraco.Web;

namespace Endzone.uSplit
{
    public static class HttpContextExtensions
    {
        private const string HttpContextItemName = "Umbraco.Web.UmbracoContext";
        
        [Obsolete("This method was introduced in Umbraco 7.4. This method and class can safely be removed once support for earlier versions is no longer needed.")]
        public static UmbracoContext GetUmbracoContext(this HttpContextBase http)
        {
            if (http.Items.Contains(HttpContextItemName))
            {
                var umbCtx = http.Items[HttpContextItemName] as UmbracoContext;
                return umbCtx;
            }
            return null;
        }
    }
}
