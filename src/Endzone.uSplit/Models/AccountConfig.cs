using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Configuration;
using Umbraco.Core;

namespace Endzone.uSplit.Models
{
    public class AnalyticsAccount
    {
        public string Name { get; }
        public string GoogleAccountId { get; }
        public string GoogleWebPropertyId { get; }
        public string GoogleProfileId { get; }

        public string UniqueId => GoogleProfileId;

        public AnalyticsAccount(NameValueCollection settings, string name)
        {
            Name = name;
            GoogleAccountId = GetValue(settings, Constants.AppSettings.GoogleAccountId);
            GoogleWebPropertyId = GetValue(settings, Constants.AppSettings.GoogleWebPropertyId);
            GoogleProfileId = GetValue(settings, Constants.AppSettings.GoogleProfileId);
        }
        
        protected string GetValue(NameValueCollection appSettings, string key)
        {
            return appSettings[GetFullKey(Name, key)];
        }

        protected static string GetFullKey(string name, string key)
        {
            if (name.IsNullOrWhiteSpace())
            {
                return $"{Constants.AppSettings.Prefix}:{key}";
            }
            return $"{Constants.AppSettings.Prefix}:{name}:{key}";
        }

        public static IEnumerable<AnalyticsAccount> GetAll()
        {
            var prefix = Constants.AppSettings.Prefix + ":";
            var keys = WebConfigurationManager.AppSettings.AllKeys;
            var names = new HashSet<string>();
            foreach (var key in keys)
            {
                if (!key.StartsWith(prefix)) continue;
                var parts = key.Split(':');
                if (parts.Length == 2)
                {
                    names.Add(string.Empty);
                }
                else if (parts.Length == 3)
                {
                    names.Add(parts[1]);
                }
            }
            return names.Select(GetByName).Where(x => !string.IsNullOrEmpty(x.GoogleProfileId));
        }
        
        public static AnalyticsAccount GetByName(string name)
        {
            return new AnalyticsAccount(WebConfigurationManager.AppSettings, name);
        }

        public static AnalyticsAccount GetByUniqueId(string uniqueId)
        {
            return GetAll().First(x => x.UniqueId == uniqueId);
        }
    }
}
