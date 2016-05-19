using System.Web.Mvc;
using System.Web.Routing;
using umbraco;
using Umbraco.Core;
using Umbraco.Web;

namespace Endzone.uSplit
{
    public class RouteConfig : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            RouteTable.Routes.MapRoute(
                name: Constants.Google.AuthRouteName,
                url: Constants.Google.AuthUrl,
                defaults: new
                {
                    controller = "GoogleAuth",
                    action = "IndexAsync",
                });

            var route = RouteTable.Routes.MapRoute(
                name: "uSplit.Experiments",
                url: $"{Constants.UmbracoPath}/backoffice/{Constants.ApplicationName}/{{controller}}/{{action}}",
                defaults: new
                {
                    controller = "Experiments",
                    action = "IndexAsync",
                    area = Constants.ApplicationAlias
                });

            route.DataTokens.Add("area", Constants.ApplicationAlias);
            //route.DataTokens.Add(Core.Constants.Web.UmbracoDataToken, umbracoTokenValue); //ensure the umbraco token is set
        }
    }
}