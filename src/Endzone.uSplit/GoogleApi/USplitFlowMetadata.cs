using System.Web.Mvc;
using Endzone.uSplit.Models;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Mvc;

namespace Endzone.uSplit.GoogleApi
{
    public class uSplitFlowMetadata : FlowMetadata
    {
        private readonly AccountConfig _config;

        public uSplitFlowMetadata(AccountConfig config)
        {
            _config = config;
            AuthCallback = "/" + Constants.Google.CallbackUrl;
        }

        public override string GetUserId(Controller controller) => Constants.Google.SystemUserId;

        public override string AuthCallback { get; }

        public override IAuthorizationCodeFlow Flow => uSplitAuthorizationCodeFlow.GetInstance(_config);
    }
}