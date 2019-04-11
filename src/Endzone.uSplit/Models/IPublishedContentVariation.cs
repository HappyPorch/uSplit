using Umbraco.Core.Models;

namespace Endzone.uSplit.Models
{
    public interface IPublishedContentVariation
    {
        /// <summary>
        /// Might be null for server-side variation
        /// </summary>
        IPublishedContent Content { get; set; }

        /// <summary>
        /// The 0-based index of chosen variation
        /// </summary>
        int GoogleVariationId { get; set; }
        string GoogleExperimentId { get; set; }
    }
}