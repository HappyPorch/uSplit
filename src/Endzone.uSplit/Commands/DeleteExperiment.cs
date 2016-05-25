using System.Threading.Tasks;
using Endzone.uSplit.GoogleApi;

namespace Endzone.uSplit.Commands
{
    public class DeleteExperiment : GoogleApiCommand<object>
    {
        public string GoogleExperimentId { get; set; }
        public override async Task<object> ExecuteAsync()
        {
            var service = await GetAnalyticsService();
            var request = service.Management.Experiments.Delete(GoogleExperimentId);
            var response = await request.ExecuteAsync();
            return response;
        }
    }
}