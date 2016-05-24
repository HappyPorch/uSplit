using System;
using System.Collections.Generic;

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


    public class ExperimentDetails
    {
        public string GoogleId { get; set; }
        public int? NodeId { get; set; }
        /// <summary>
        /// The name of the experiment - Node name for uSplit experiments, otherwise Name from Google Experiments.
        /// </summary>
        public string Name { get; set; }
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

    }
}