using System.Collections.Generic;
using Umbraco.Core.Models;

namespace Endzone.uSplit.Models
{
    public interface IExperiment
    {
        /// <summary>
        /// Indicates whether this experiment was created by uSplit
        /// </summary>
        bool IsUSplitExperiment { get; }

        /// <summary>
        /// The Google Experiment ID
        /// </summary>
        string Id { get; }
        string Name { get; }

        IContent PageUnderTest { get; }
        Google.Apis.Analytics.v3.Data.Experiment GoogleExperiment { get; }
        List<Variation> Variations { get; }
    }
}