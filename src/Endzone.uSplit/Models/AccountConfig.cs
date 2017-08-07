using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Configuration;
using Umbraco.Core;

namespace Endzone.uSplit.Models
{
    public class AccountConfig
    {
        public string Name { get; }
        public string GoogleClientId { get; }
        public string GoogleClientSecret { get; }
        public string GoogleAccountId { get; }
        public string GoogleWebPropertyId { get; }
        public string GoogleProfileId { get; }

        public AccountConfig(NameValueCollection settings, string name)
        {
            Name = name;
            GoogleClientId = GetValue(settings, Constants.AppSettings.GoogleClientId);
            GoogleClientSecret = GetValue(settings, Constants.AppSettings.GoogleClientSecret);
            GoogleAccountId = GetValue(settings, Constants.AppSettings.GoogleAccountId);
            GoogleWebPropertyId = GetValue(settings, Constants.AppSettings.GoogleWebPropertyId);
            GoogleProfileId = GetValue(settings, Constants.AppSettings.GoogleProfileId);
        }

        public string GetValue(NameValueCollection appSettings, string key)
        {
            return appSettings[GetFullKey(Name, key)];
        }

        public static IEnumerable<AccountConfig> GetAll()
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
            return names.Select(Get);
        }

        public static AccountConfig Get(string name)
        {
            return new AccountConfig(WebConfigurationManager.AppSettings, name);
        }

        public static string GetFullKey(string name, string key)
        {
            if (name.IsNullOrWhiteSpace())
            {
                return $"{Constants.AppSettings.Prefix}:{key}";
            }
            return $"{Constants.AppSettings.Prefix}:{name}:{key}";
        }

        public static AccountConfig GetByProfileId(string profileId)
        {
            return GetAll().First(x => x.GoogleProfileId == profileId);
        }
    }
}
