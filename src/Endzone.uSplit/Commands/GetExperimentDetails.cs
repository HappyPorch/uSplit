using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Endzone.uSplit.Models;

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
                    NodeId = variation.VariedContent?.Id
                });
            }

            return new ExperimentDetails()
            {
                Name = Experiment.IsUSplitExperiment ? Experiment.PageUnderTest.Name : Experiment.GoogleExperiment.Name,
                GoogleName = Experiment.GoogleExperiment.Name,
                NodeId = Experiment.PageUnderTest?.Id,
                GoogleId = Experiment.GoogleExperiment.Id,
                Status = Experiment.GoogleExperiment.Status,
                Created = Experiment.GoogleExperiment.Created,
                Updated = Experiment.GoogleExperiment.Updated,
                StartTime = Experiment.GoogleExperiment.StartTime,
                EndTime = Experiment.GoogleExperiment.EndTime,
                Variations = variationsDetails,
                Debug = Experiment.GoogleExperiment
            };
        }
    }
}