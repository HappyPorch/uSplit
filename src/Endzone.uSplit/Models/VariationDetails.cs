namespace Endzone.uSplit.Models
{
    public class VariationDetails
    {
        public int? NodeId { get; set; }
        /// <summary>
        /// The name of the variation - Node name for uSplit experiments, otherwise Name from Google Experiments.
        /// </summary>
        public string Name { get; set; }
    }
}