using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessor.Common.Extensions;

namespace Endzone.uSplit.Pipeline
{
    public static class Segmentation
    {
        private static Dictionary<string, ISegmentationProvider> providers;

        static Segmentation()
        {
            var providerType = typeof(ISegmentationProvider);
            var instances = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetLoadableTypes())
                .Where(type => providerType.IsAssignableFrom(type) && type.IsClass)
                .Select(x => Activator.CreateInstance(x) as ISegmentationProvider);

            providers = instances.ToDictionary(provider => provider.ProviderKey);
        }

        public static ISegmentationProvider[] Providers => providers.Values.ToArray();

        /// <summary>
        /// Returns a provider for the given key, or null when no prodiver found
        /// </summary>
        public static ISegmentationProvider GetByKey(string key)
        {
            if (key == null)
                return null;

            ISegmentationProvider provider;
            return providers.TryGetValue(key, out provider) ? provider : null;
        }
    }
}
