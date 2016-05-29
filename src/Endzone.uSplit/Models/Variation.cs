using Umbraco.Core.Models;
using GoogleVariation = Google.Apis.Analytics.v3.Data.Experiment.VariationsData;

namespace Endzone.uSplit.Models
{
    public class Variation
    {
        public IContent VariedContent { get; set; }
        public GoogleVariation GoogleVariation { get; set; }
    }
}