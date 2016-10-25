using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Models;
using GoogleExperiment = Google.Apis.Analytics.v3.Data.Experiment;

namespace Endzone.uSplit.Models
{
    public class Experiment : IExperiment
    {
        public const string DescriptionSeparator = "/**** uSplit configuration, please do not modify ****/";
        /// <summary>
        /// Indicates whether this experiment was created by uSplit
        /// </summary>
        public bool IsUSplitExperiment => PageUnderTest != null;
        /// <summary>
        /// The Google Experiment ID
        /// </summary>
        public string Id => GoogleExperiment.Id;
        public IContent PageUnderTest => Variations?[0].VariedContent;
        public bool IsRunning => GoogleExperiment.Status == "RUNNING";
        public GoogleExperiment GoogleExperiment { get; set; }
        public List<Variation> Variations { get; set; }
        public ExperimentSettings Settings { get; set; }

        public Experiment(GoogleExperiment experiment)
        {
            GoogleExperiment = experiment;
            ParseGoogleData(experiment);
        }

        private void ParseGoogleData(GoogleExperiment experiment)
        {
            Settings = ParseSettings(experiment.Description);
            Variations = ParseVariations(experiment);
        }

        private static List<Variation> ParseVariations(GoogleExperiment experiment)
        {
            //we might not be executing this in the scope of a http request
            var contentService = ApplicationContext.Current.Services.ContentService;
            var variations = new List<Variation>();
            foreach (var variation in experiment.Variations)
            {
                IContent content = null;
                int variationNodeId;
                if (!int.TryParse(variation.Url, out variationNodeId))
                    content = contentService.GetById(variationNodeId);

                variations.Add(new Variation
                {
                    VariedContent = content,
                    GoogleVariation = variation,
                });
            }
            return variations;
        }

        public static ExperimentSettings ParseSettings(string description)
        {
            var source = description ?? string.Empty;
            var separatorPosition = source.IndexOf(DescriptionSeparator, StringComparison.InvariantCultureIgnoreCase);
            if (separatorPosition > -1)
                source = source.Substring(separatorPosition);
            return JsonConvert.DeserializeObject<ExperimentSettings>(source) ?? new ExperimentSettings();
        }

        public static string UpdateSettings(string description, ExperimentSettings settings)
        {
            var userText = description ?? string.Empty;
            var separatorPosition = userText.IndexOf(DescriptionSeparator, StringComparison.InvariantCultureIgnoreCase);
            if (separatorPosition > -1)
                userText = userText.Substring(0, separatorPosition);
            var serializedConfig = JsonConvert.SerializeObject(settings);
            return $"{userText}\n{DescriptionSeparator}\n{serializedConfig}";
        }

        public static string ConstructExperimentName(int id, string name)
        {
            return $"{Constants.ApplicationName} - {id} - {name}";
        }

        public static int? ExtractNodeIdFromExperimentName(string name)
        {
            var regex = new Regex($@"{Constants.ApplicationName} - (\d+)(- .?) - .*");
            var matches = regex.Match(name);
            if (matches.Success)
                return int.Parse(matches.Groups[1].Value);

            return null;
        }
    }
}
