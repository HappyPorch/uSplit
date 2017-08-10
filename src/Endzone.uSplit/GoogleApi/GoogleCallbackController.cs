using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Endzone.uSplit.Models;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Auth.OAuth2.Mvc.Controllers;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Web;
using Umbraco.Web.Mvc;

namespace Endzone.uSplit.GoogleApi
{
    [UmbracoAuthorize]
    public class GoogleCallbackController : AuthCallbackController
    {
        private uSplitFlowMetadata _flowData;

        public override Task<ActionResult> IndexAsync(AuthorizationCodeResponseUrl authorizationCode, CancellationToken taskCancellationToken)
        {
            var stateWithoutRandomToken = authorizationCode.State.Substring(0, authorizationCode.State.Length - AuthorizationCodeWebApp.StateRandomLength);
            var uri = new Uri(stateWithoutRandomToken);
            var profileId = uri.ParseQueryString().Get("profileId");
            var config = AccountConfig.GetByUniqueId(profileId);
            _flowData = new uSplitFlowMetadata(config);
            return base.IndexAsync(authorizationCode, taskCancellationToken);
        }

        protected override FlowMetadata FlowData => _flowData;

    }
}
