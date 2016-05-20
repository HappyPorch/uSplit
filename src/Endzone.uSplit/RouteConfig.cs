using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;

namespace Endzone.uSplit
{
    public class RouteConfig : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            RouteTable.Routes.MapRoute(
                name: "uSplit.GoogleApiAuth",
                url: Constants.Google.BaseUrl);
        }
    }
}