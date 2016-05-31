namespace Endzone.uSplit.Models
{
    public class VariationDetails
    {
        public int? NodeId { get; set; }
        /// <summary>
        /// The name of the variation - Node name for uSplit experiments, otherwise Name from Google Experiments.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The name of the variation in Google Experiments - should be unique!
        /// </summary>
        public string GoogleName { get; set; }

        public double? Weight { get; set; }

        public string Status { get; set; }

        public bool? Won { get; set; }
    }
}