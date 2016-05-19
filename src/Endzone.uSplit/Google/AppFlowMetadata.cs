using System.Web.Configuration;
using System.Web.Mvc;
using Google.Apis.Analytics.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Util.Store;
using umbraco;

namespace Endzone.uSplit.Google
{ 
    public class GoogleAuthFlowMetadata : FlowMetadata
    {
        private static readonly IAuthorizationCodeFlow flow;

        public GoogleAuthFlowMetadata()
        {
            AuthCallback = "/" + Constants.Google.AuthUrl;
        }

        static GoogleAuthFlowMetadata()
        {
            flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = WebConfigurationManager.AppSettings[Constants.AppSettings.GoogleClientId],
                    ClientSecret = WebConfigurationManager.AppSettings[Constants.AppSettings.GoogleClientSecret],
                },
                Scopes = new[] { AnalyticsService.Scope.AnalyticsEdit },
                DataStore = new FileDataStore($"/TEMP/{Constants.ApplicationAlias}/Analytics.Api.Auth.Store")
            });
        }

        public override string GetUserId(Controller controller) => controller.HttpContext.User.Identity.Name + "6";

        public override string AuthCallback { get; }

        public override IAuthorizationCodeFlow Flow => flow;
    }
}