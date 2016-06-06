using System.Threading.Tasks;
using Endzone.uSplit.GoogleApi;
using Umbraco.Core;

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

            //update cache
            var experiments = await new GetCachedExperiments().ExecuteAsync();
            var index = experiments.FindIndex(e => e.Id == GoogleExperimentId);
            if (index >= 0)
            {
                experiments.RemoveAt(index);
            }

            return response;
        }
    }
}