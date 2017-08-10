using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using Google.Apis.Analytics.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util;
using Google.Apis.Util.Store;
using System.Web.Hosting;
using Endzone.uSplit.Models;
using Google.Apis.Auth.OAuth2.Requests;
using ICSharpCode.SharpZipLib.Zip;

namespace Endzone.uSplit.GoogleApi
{
    public class uSplitAuthorizationCodeFlow : GoogleAuthorizationCodeFlow
    {
        public static uSplitAuthorizationCodeFlow GetInstance(AccountConfig config)
        {
            if (!Instances.ContainsKey(config.UniqueId))
            {
                Instances[config.UniqueId] = new uSplitAuthorizationCodeFlow(config);
            }
            return Instances[config.UniqueId];
        }
        
        public static readonly Dictionary<string, uSplitAuthorizationCodeFlow> Instances = new Dictionary<string, uSplitAuthorizationCodeFlow>();

        private static Initializer CreateFlowInitializer(AccountConfig config)
        {
            return new Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = config.GoogleClientId,
                    ClientSecret = config.GoogleClientSecret
                },
                Scopes = new[] {AnalyticsService.Scope.AnalyticsEdit},
                DataStore = new FileDataStore(GetStoragePath(), true),
                UserDefinedQueryParams = new []{new KeyValuePair<string, string>("testkey", "testvalue"), },
            };
        }

        private static string GetStoragePath()
        {
            var appName = Constants.ApplicationName;
            var storagePath = HostingEnvironment.MapPath($"~/App_Data/TEMP/{appName}/google/auth");
            return storagePath;
        }

        private uSplitAuthorizationCodeFlow(AccountConfig config) : base(CreateFlowInitializer(config))
        {
            
        }

        public override AuthorizationCodeRequestUrl CreateAuthorizationCodeRequest(string redirectUri)
        {
            return new GoogleAuthorizationCodeRequestUrl(new Uri(AuthorizationServerUrl))
            {
                ClientId = ClientSecrets.ClientId,
                Scope = string.Join(" ", Scopes),
                RedirectUri = redirectUri,
                AccessType = "offline", //required to get a useful refresh token
                ApprovalPrompt = "force" 
            };
        }

        /// <summary>
        ///     Indicates whether a valid token exists for the Google API.
        /// </summary>
        public async Task<bool> IsConnected(CancellationToken cancellationToken)
        {
            var token = await LoadTokenAsync(Constants.Google.SystemUserId, cancellationToken);
            return !DoWeHaveUsefulToken(token);
        }

        private bool DoWeHaveUsefulToken(TokenResponse token)
        {
            if (ShouldForceTokenRetrieval() || token == null)
                return true;
            if (token.RefreshToken == null)
                return token.IsExpired(SystemClock.Default);
            return false;
        }
    }
}