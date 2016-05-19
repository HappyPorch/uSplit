using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Auth.OAuth2.Mvc.Controllers;
using Umbraco.Web.Mvc;

namespace Endzone.uSplit.Google
{
    [UmbracoAuthorize]
    public class GoogleAuthController : AuthCallbackController
    {
        protected override FlowMetadata FlowData => new GoogleAuthFlowMetadata();
    }
}
