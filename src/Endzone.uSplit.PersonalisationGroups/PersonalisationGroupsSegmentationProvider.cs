using System;
using System.Configuration;
using System.Linq;
using System.Web;
using Umbraco.Web;
using Zone.UmbracoPersonalisationGroups;

namespace Endzone.uSplit.PersonalisationGroups
{
    public class PersonalisationGroupsSegmentationProvider : ISegmentationProvider
    {
        public string Name => "Personalisation Groups";
        public string ProviderKey => "PG";

        public string AngularViewPath => "/App_Plugins/uSplit.PersonalisationGroups/backoffice/abtesting/segments.html";

        /// <summary>
        /// Adapted IsMatch from https://github.com/AndyButland/UmbracoPersonalisationGroups/blob/master/Zone.UmbracoPersonalisationGroups/ExtensionMethods/PublishedContentExtensions.cs
        /// </summary>
        public bool VisitorInSegment(string segmentKey)
        {
            var helper = new UmbracoHelper(UmbracoContext.Current);
            var segment = helper.TypedContent(segmentKey);
            if (segment == null)
            {
                return false; //something went wrong, we didn't find the segment!
            }

            var definition = segment.GetPropertyValue<PersonalisationGroupDefinition>(AppConstants.PersonalisationGroupDefinitionPropertyAlias);
            if (IsStickyMatch(definition, segment.Id))
            {
                return true;
            }

            var matchCount = PersonalisationGroupMatcher.CountMatchingDefinitionDetails(definition);

            // If matching any and matched at least one, or matching all and matched all - we've matched one of the definitions 
            // associated with a selected personalisation group
            if ((definition.Match == PersonalisationGroupDefinitionMatch.Any && matchCount > 0) ||
                (definition.Match == PersonalisationGroupDefinitionMatch.All && matchCount == definition.Details.Count()))
            {
                MakeStickyMatch(definition, segment.Id);
                return true;
            }


            // If we've got here, we haven't found a match
            return false;
        }

        public string SegmentName(string segmentKey)
        {
            var helper = new UmbracoHelper(UmbracoContext.Current);
            var segment = helper.TypedContent(segmentKey);
            return segment == null ? "Segment not found" : segment.Name;
        }

        #region Copied from https://github.com/AndyButland/UmbracoPersonalisationGroups/blob/master/Zone.UmbracoPersonalisationGroups/ExtensionMethods/PublishedContentExtensions.cs

        private static bool IsStickyMatch(PersonalisationGroupDefinition definition, int groupNodeId)
        {
            if (definition.Duration == PersonalisationGroupDefinitionDuration.Page)
            {
                return false;
            }

            var httpContext = HttpContext.Current;
            var key = GetCookieKeyForMatchedGroups(definition.Duration);
            var cookie = httpContext.Request.Cookies[key];
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                return IsGroupMatched(cookie.Value, groupNodeId);
            }

            return false;
        }

        /// <summary>
        /// Makes a matched group sticky for the visitor via a cookie setting according to group definition
        /// </summary>
        /// <param name="definition">Matched group definition</param>
        /// <param name="groupNodeId">Id of the matched groups node</param>
        private static void MakeStickyMatch(PersonalisationGroupDefinition definition, int groupNodeId)
        {
            if (definition.Duration == PersonalisationGroupDefinitionDuration.Page)
            {
                return;
            }

            var httpContext = HttpContext.Current;
            var key = GetCookieKeyForMatchedGroups(definition.Duration);
            var cookie = httpContext.Request.Cookies[key];
            if (cookie != null)
            {
                cookie.Value = AppendGroupNodeId(cookie.Value, groupNodeId);
            }
            else
            {
                cookie = new HttpCookie(key)
                {
                    Value = groupNodeId.ToString(),
                    HttpOnly = true,
                };
            }

            if (definition.Duration == PersonalisationGroupDefinitionDuration.Visitor)
            {
                int cookieExpiryInDays;
                if (!int.TryParse(ConfigurationManager.AppSettings[AppConstants.ConfigKeys.PersistentMatchedGroupsCookieExpiryInDays], out cookieExpiryInDays))
                {
                    cookieExpiryInDays = AppConstants.DefaultMatchedGroupsTrackingCookieExpiryInDays;
                }

                cookie.Expires = DateTime.Now.AddDays(cookieExpiryInDays);
            }

            httpContext.Response.Cookies.Add(cookie);
        }

        private static string GetCookieKeyForMatchedGroups(PersonalisationGroupDefinitionDuration duration)
        {
            string defaultKey, appSettingKey;
            switch (duration)
            {
                case PersonalisationGroupDefinitionDuration.Session:
                    defaultKey = "sessionMatchedGroups";
                    appSettingKey = AppConstants.ConfigKeys.CookieKeyForSessionMatchedGroups;
                    break;
                case PersonalisationGroupDefinitionDuration.Visitor:
                    defaultKey = "persistentMatchedGroups";
                    appSettingKey = AppConstants.ConfigKeys.CookieKeyForPersistentMatchedGroups;
                    break;
                default:
                    throw new InvalidOperationException("Only session and visitor personalisation groups can be tracked.");
            }

            // First check if key defined in config
            var cookieKey = ConfigurationManager.AppSettings[appSettingKey];
            if (string.IsNullOrEmpty(cookieKey))
            {
                // If not, use the convention key
                cookieKey = defaultKey;
            }

            return cookieKey;
        }

        /// <summary>
        /// Adds a matched group to the cookie for sticky groups
        /// </summary>
        /// <param name="matchedGroupIds">Existing cookie value of matched group node Ids</param>
        /// <param name="groupNodeId">Id of the matched groups node</param>
        /// <returns>Updated cookie value</returns>
        private static string AppendGroupNodeId(string matchedGroupIds, int groupNodeId)
        {
            // Shouldn't exist as we don't try to append an already sticky group match, but just to be sure
            if (!IsGroupMatched(matchedGroupIds, groupNodeId))
            {
                matchedGroupIds = matchedGroupIds + "," + groupNodeId;
            }

            return matchedGroupIds;
        }

        /// <summary>
        /// Checks if group is matched in tracking cookie value
        /// </summary>
        /// <param name="matchedGroupIds">Existing cookie value of matched group node Ids</param>
        /// <param name="groupNodeId">Id of the matched groups node</param>
        /// <returns>True if matched</returns>
        private static bool IsGroupMatched(string matchedGroupIds, int groupNodeId)
        {
            return matchedGroupIds
                .Split(',')
                .Any(x => int.Parse(x) == groupNodeId);
        }

        #endregion
    
    }
}
