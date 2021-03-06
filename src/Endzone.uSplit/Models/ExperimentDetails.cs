using System;
using System.Collections.Generic;

namespace Endzone.uSplit.Models
{
    /// <summary>
    /// The Experiment Model of the API
    /// </summary>
    public class ExperimentDetails
    {
        public string GoogleId { get; set; }
        /// <summary>
        /// The name of the experiment - Node name for uSplit experiments, otherwise Name from Google Experiments.
        /// </summary>
        public string Name { get; set; }
        public string GoogleName { get; set; }
        public bool ServerSide { get; set; }
        public int? NodeId { get; set; }

        public string Status { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// The Google Experiment
        /// </summary>
        public object Debug { get; set; }
        public List<VariationDetails> Variations { get; set; }
        public string Metric { get; set; }

        public string SegmentationProviderKey { get; set; }
        public string SegmentationValue { get; set; }
    }
}