using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Logging;
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
        public bool IsUSplitExperiment { get; }
        /// <summary>
        /// The Google Experiment ID
        /// </summary>
        public string Id => GoogleExperiment.Id;
        public string Name { get;  }
        public bool ServerSide { get; }
        public IContent PageUnderTest => Variations?.FirstOrDefault()?.VariedContent;
        public bool IsRunning => GoogleExperiment.Status == "RUNNING";
        public GoogleExperiment GoogleExperiment { get; }
        public List<Variation> Variations { get; }
        public ExperimentConfiguration Configuration { get; }

        public Experiment(GoogleExperiment experiment)
        {
            GoogleExperiment = experiment;
            if (TryParseUSplitExperimentName(experiment.Name, out var nodeId, out var name))
            {
                IsUSplitExperiment = true;
                Name = name;
                ServerSide = nodeId == -1;
                Configuration = ParseSettings(experiment.Description);
            }
            else
            {
                Name = experiment.Name;
            }
            Variations = ParseVariations(experiment, IsUSplitExperiment && !ServerSide);
        }

        private static List<Variation> ParseVariations(GoogleExperiment experiment, bool isContentExperiment)
        {
            //we might not be executing this in the scope of a http request
            var contentService = ApplicationContext.Current.Services.ContentService;
            var variations = new List<Variation>();
            foreach (var variation in experiment.Variations)
            {
                IContent content = null;
                if (isContentExperiment && int.TryParse(variation.Url, out int variationNodeId))
                    content = contentService.GetById(variationNodeId);

                variations.Add(new Variation
                {
                    Name = variation.Name,
                    VariedContent = content,
                    GoogleVariation = variation,
                });
            }
            return variations;
        }

        public static ExperimentConfiguration ParseSettings(string description)
        {
            var source = description ?? string.Empty;
            var separatorPosition = source.IndexOf(DescriptionSeparator, StringComparison.InvariantCultureIgnoreCase);
            if (separatorPosition > -1)
            {
                source = source.Substring(separatorPosition);
                try
                {
                    var settings = JsonConvert.DeserializeObject<ExperimentConfiguration>(source);
                    return settings ?? new ExperimentConfiguration();
                }
                catch (JsonReaderException e)
                {
                    LogHelper.Error<Experiment>("Parsing segmentation settings for experiment failed. Will use default settings.", e);
                }
            }
            return new ExperimentConfiguration();
        }

        public static string UpdateSettings(string description, ExperimentConfiguration settings)
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

        public static bool TryParseUSplitExperimentName(string originalName, out int nodeId, out string name)
        {
            var regex = new Regex($@"{Constants.ApplicationName} - (\-?\d+) - (.+)");
            var matches = regex.Match(originalName);
            if (matches.Success)
            {
                nodeId = int.Parse(matches.Groups[1].Value);
                name = matches.Groups[2].Value;
                return true;
            }

            nodeId = 0;
            name = null;
            return false;
        }
    }
}
