using Umbraco.Core.Models;

namespace Endzone.uSplit.Models
{
    public class Variation
    {
        public IContent VariedContent { get; set; }
        public Google.Apis.Analytics.v3.Data.Experiment.VariationsData GoogleVariation { get; set; }
    }
}