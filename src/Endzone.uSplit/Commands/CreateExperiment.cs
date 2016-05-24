using System.Threading.Tasks;
using Endzone.uSplit.GoogleApi;
using Endzone.uSplit.Models;
using Umbraco.Core.Models;
using GoogleExperiment = Google.Apis.Analytics.v3.Data.Experiment;

namespace Endzone.uSplit.Commands
{
    public class CreateExperiment : GoogleApiCommand<GoogleExperiment>
    {
        private const string CreationStatus = "DRAFT";
        public int NodeUnderTestId { get; set; }

        public override async Task<GoogleExperiment> ExecuteAsync()
        {
            var node = UmbracoContext.Application.Services.ContentService.GetById(NodeUnderTestId);
            var service = await GetAnalyticsService();
            var request = service.Management.Experiments.Insert(CreateNewExperiment(node));
            var response = await request.ExecuteAsync();
            return response;
        }

        private GoogleExperiment CreateNewExperiment(IContent node)
        {
            return new GoogleExperiment()
            {
                Name = Experiment.ConstructExperimentName(NodeUnderTestId, node.Name),
                Status = CreationStatus,
                Variations = new[] {new GoogleExperiment.VariationsData()
                {
                    Name = Constants.Google.OriginalVariationName,
                    Url = $"{NodeUnderTestId}"
                }}
            };
        }
    }
}
