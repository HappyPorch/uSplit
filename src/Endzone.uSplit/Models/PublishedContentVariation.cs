using Umbraco.Core.Models;

namespace Endzone.uSplit.Models
{
    public class PublishedContentVariation : IPublishedContentVariation
    {
        public PublishedContentVariation(IPublishedContent content, string googleExperimentId, int googleVariationId)
        {
            Content = content;
            GoogleVariationId = googleVariationId;
            GoogleExperimentId = googleExperimentId;
        }

        public IPublishedContent Content { get; set; }
        public int GoogleVariationId { get; set; }
        public string GoogleExperimentId { get; set; }
    }
}