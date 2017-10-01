using System;
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
            public const string Block = "icon-block";
            public const string Play = "icon-play";
            public const string Autofill = "icon-autofill";
            public const string FlagAlt = "icon-flag-alt";
            public const string Account = "icon-keyhole";
        }

        public static class Trees
        {
            public const string AbTesting = "abtesting";
        }

        public static class AppSettings
        {
            public const string Prefix              = "uSplit";
            public const string GoogleClientId      = "uSplit:googleClientId";
            public const string GoogleClientSecret  = "uSplit:googleClientSecret";
            public const string GoogleAccountId     = "accountId";
            public const string GoogleWebPropertyId = "webPropertyId";
            public const string GoogleProfileId     = "profileId";
        }
        
        public static class Google
        {
            public static readonly string BaseUrl = $"{UmbracoPath}/backoffice/{ApplicationAlias}/{{controller}}/{{action}}";
            public static readonly string CallbackUrl = $"{UmbracoPath}/backoffice/{ApplicationAlias}/GoogleCallback/IndexAsync";
            public const string SystemUserId = "googleApiAuth";
            public static string OriginalVariationName = "ORIGINAL";
        }

        public static class Cache
        {
            public static string RawExperimentData = "uSplitRawExperimentData";
            public static string ParsedExperiments = "uSplitParsedExperiments";
            public static TimeSpan ExperimentsRefreshInterval = TimeSpan.FromMinutes(30);
        }

        public static class Cookies
        {
            public static string CookieVariationName = "uSplitExperiment";
        }

        public static string UmbracoPath => GlobalSettings.Path.TrimStart('/');
        public const string VariationReportedHttpContextItemsKey = "uSplit:VariationReported";
    }
}
