using System.Threading.Tasks;
using Endzone.uSplit.GoogleApi;
using Google.Apis.Analytics.v3.Data;

namespace Endzone.uSplit.Commands
{
    public class GetExperiments : GoogleApiCommand<Experiments>
    {
        //TODO: Handle the case if the user has over 1000 experiments.
        public override async Task<Experiments> ExecuteAsync()
        {
            var service = await GetAnalyticsService();
            var list = service.Management.Experiments.List();
            var experiments = await list.ExecuteAsync();
            return experiments;
        }
    }
}