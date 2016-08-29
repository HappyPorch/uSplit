using System;
using System.Web;
using Umbraco.Web;

namespace Endzone.uSplit
{
    [Obsolete("Remove this class one support for Umbraco versions prior to 7.4 is needed.")]
    public static class HttpContextExtensions
    {
        public const string HttpContextItemName = "Umbraco.Web.UmbracoContext";
        
        /// <remarks>
        /// This method masks the same extension method that was introduced
        /// in Umbraco 7.4. This allows uSplit to work with previous version
        /// of Umbraco, as all invocations (under our namespace anyway) bind
        /// to our method. Once support for Umbraco versions prior to 7.4 ends
        /// we can simply remove this method and the code will still compile.
        /// </remarks>
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
