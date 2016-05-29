﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using Umbraco.Core.Models;
using Umbraco.Web;
using GoogleExperiment = Google.Apis.Analytics.v3.Data.Experiment;

namespace Endzone.uSplit.Models
{
    public class Experiment
    {
        public bool IsUSplitExperiment { get; set; }

        public string Id => GoogleExperiment.Id;

        public IContent PageUnderTest => Variations?[0].VariedContent;

        public GoogleExperiment GoogleExperiment { get; set; }
        public List<Variation> Variations { get; set; }

        public Experiment(GoogleExperiment experiment)
        {
            GoogleExperiment = experiment;
            ParseGoogleData(experiment);
        }

        private void ParseGoogleData(GoogleExperiment experiment)
        {
            var contentService = UmbracoContext.Current.Application.Services.ContentService;
            var id = ExtractNodeIdFromExperimentName(experiment.Name);
            if (!id.HasValue) return;

            var variations = new List<Variation>();
            foreach (var variation in experiment.Variations) 
            {
                int variationNodeId;
                if (!int.TryParse(variation.Url, out variationNodeId))
                    return;

                variations.Add(new Variation
                {
                    VariedContent = contentService.GetById(variationNodeId),
                    GoogleVariation = variation
                });
            }
            
            Variations = variations;
            IsUSplitExperiment = true;
        }

        public static string ConstructExperimentName(int id, string name)
        {
            return $"{Constants.ApplicationName} - {id} - {name}";
        }

        public static int? ExtractNodeIdFromExperimentName(string name)
        {
            var regex = new Regex($@"{Constants.ApplicationName} - (\d+) - .*");
            var matches = regex.Match(name);
            if (matches.Success)
                return int.Parse(matches.Groups[1].Value);

            return null;
        }
    }
}
