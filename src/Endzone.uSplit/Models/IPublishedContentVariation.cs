using Umbraco.Core.Models;

namespace Endzone.uSplit.Models
{
    public interface IPublishedContentVariation
    {
        IPublishedContent Content { get; set; }
        int GoogleVariationId { get; set; }
        string GoogleExperimentId { get; set; }
    }
}