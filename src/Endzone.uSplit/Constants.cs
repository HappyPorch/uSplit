using umbraco;

namespace Endzone.uSplit
{
    public static class Constants
    {
        public const string PluginName = "uSplit";
        public const string ApplicationName = "uSplit";
        public const string ApplicationAlias = "usplit";

        public static class Icons
        {
            public const string Split = "icon-split-alt";
        }

        public static class Trees
        {
            public const string AbTesting = "abtesting";
        }

        public static class AppSettings
        {
            public const string GoogleClientId      = "uSplit:googleClientId";
            public const string GoogleClientSecret  = "uSplit:googleClientSecret";
            public const string GoogleAccountId     = "uSplit:accountId";
            public const string GoogleWebPropertyId = "uSplit:webPropertyId";
            public const string GoogleProfileId     = "uSplit:profileId";
        }

        public static class Google
        {
            public static string BaseUrl = $"{UmbracoPath}/backoffice/{ApplicationAlias}/{{controller}}/{{action}}";
            public static readonly string CallbackUrl = $"{UmbracoPath}/backoffice/{ApplicationAlias}/GoogleCallback/IndexAsync";
            public static readonly string AuthUrl = $"{UmbracoPath}/backoffice/{ApplicationAlias}/GoogleAuth/ReauthorizeAsync";
            public const string SystemUserId = "googleApiAuth";
        }

        public static string UmbracoPath => GlobalSettings.Path.TrimStart('/');
    }
}
