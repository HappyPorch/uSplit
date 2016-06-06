using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Endzone.uSplit.Models;
using Umbraco.Core;

namespace Endzone.uSplit.Commands
{
    public class GetExperimentDetails : Command<ExperimentDetails>
    {
        public Experiment Experiment { get; set; }

        public override async Task<ExperimentDetails> ExecuteAsync()
        {
            var variations = Experiment.Variations;
            var variationsDetails = new List<VariationDetails>(variations?.Count ?? 0);
            foreach (var variation in variations ?? Enumerable.Empty<Variation>())
            {
                variationsDetails.Add(new VariationDetails()
                {
                    Name = Experiment.IsUSplitExperiment ? variation.VariedContent.Name : variation.GoogleVariation.Name,
                    GoogleName = variation.GoogleVariation.Name,
                    NodeId = variation.VariedContent?.Id,
                    Weight = variation.GoogleVariation.Weight,
                    Status = variation.GoogleVariation.Status,
                    Won = variation.GoogleVariation.Won
                });
            }

            return new ExperimentDetails()
            {
                Name = Experiment.IsUSplitExperiment ? Experiment.PageUnderTest.Name : Experiment.GoogleExperiment.Name,
                GoogleName = Experiment.GoogleExperiment.Name,
                NodeId = Experiment.PageUnderTest?.Id,
                GoogleId = Experiment.GoogleExperiment.Id,
                Status = Experiment.GoogleExperiment.Status,
                Metric = ExtractMetricName(Experiment.GoogleExperiment.ObjectiveMetric),
                Created = Experiment.GoogleExperiment.Created,
                Updated = Experiment.GoogleExperiment.Updated,
                StartTime = Experiment.GoogleExperiment.StartTime,
                EndTime = Experiment.GoogleExperiment.EndTime,
                Variations = variationsDetails,
                Debug = Experiment.GoogleExperiment,
                MissingLicense = !LicenseHelper.HasValidLicense() && !LicenseHelper.IsCoveredInFreeTrial(Experiment.GoogleExperiment)
            };
        }

        private static string ExtractMetricName(string googleMetric)
        {
            if (string.IsNullOrEmpty(googleMetric))
                return googleMetric;

            var metric = googleMetric;
            if (metric.StartsWith("ga:"))
                metric = metric.Remove(0, 3);

            if (metric[0].IsLowerCase())
                metric = char.ToUpperInvariant(metric[0]) + metric.Substring(1);

            //separate words
            metric = Regex.Replace(metric, @"([^A-Z\s])([A-Z])", "$1 $2");

            return metric;
        }
    }
}