using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Auth.OAuth2.Mvc.Controllers;
using Umbraco.Web.Mvc;

namespace Endzone.uSplit.GoogleApi
{
    [UmbracoAuthorize]
    public class GoogleCallbackController : AuthCallbackController
    {
        protected override FlowMetadata FlowData => new uSplitFlowMetadata();

    }
}
