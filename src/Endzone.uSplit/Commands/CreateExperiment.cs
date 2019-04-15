using System;
using System.Threading.Tasks;
using Endzone.uSplit.GoogleApi;
using Endzone.uSplit.Models;
using GoogleExperiment = Google.Apis.Analytics.v3.Data.Experiment;

namespace Endzone.uSplit.Commands
{
    public class CreateExperiment : GoogleApiCommand<GoogleExperiment>
    {
        private const string CreationStatus = "DRAFT";

        public bool ServerSide { get; }
        public int NodeUnderTestId { get; }
        public string Name { get; }

        public CreateExperiment(AnalyticsAccount account, int nodeUnderTestId) 
            : this (account, null, false, nodeUnderTestId) { }

        public CreateExperiment(AnalyticsAccount account, string name, bool serverSide = true, int? nodeUnderTest = null) 
            : base(account)
        {
            if (!serverSide && nodeUnderTest < 1)
                throw new InvalidOperationException("A valid node ID must be specificed for front-end tests");

            if (serverSide && nodeUnderTest.HasValue)
                throw new InvalidOperationException("A server side experiment cannot specify a node ID");

            if (string.IsNullOrEmpty(name))
            {
                if (serverSide)
                {
                    throw new InvalidOperationException("Experiment name is required for server-side experiments");
                }
                else
                {
                    if (!ServerSide && string.IsNullOrEmpty(name))
                    {
                        var node = UmbracoContext.Application.Services.ContentService.GetById(NodeUnderTestId);
                        name = node.Name;
                    }
                }
            }

            ServerSide = serverSide;
            NodeUnderTestId = nodeUnderTest ?? -1;
            Name = name;
        }

        public override async Task<GoogleExperiment> ExecuteAsync()
        {
            var service = await GetAnalyticsService();
            var request = service.Management.Experiments.Insert(account, CreateExperimentRecord());
            var response = await request.ExecuteAsync();
            return response;
        }

        private GoogleExperiment CreateExperimentRecord()
        {
            return new GoogleExperiment()
            {
                Name = Experiment.ConstructExperimentName(NodeUnderTestId, Name),
                Status = CreationStatus,
                Variations = new[] {new GoogleExperiment.VariationsData()
                {
                    Name = Constants.Google.OriginalVariationName,
                    Url = ServerSide ? "SERVER_SIDE" : $"{NodeUnderTestId}"
                }}
            };
        }
    }
}
