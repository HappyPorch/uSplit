using System.Threading.Tasks;
using Endzone.uSplit.GoogleApi;
using Google.Apis.Analytics.v3.Data;

namespace Endzone.uSplit.Commands
{
    public class CheckAccess : GoogleApiCommand<bool>
    {
        //TODO: Handle the case if the user has over 1000 experiments.
        public override async Task<bool> ExecuteAsync()
        {
            var service = await GetAnalyticsService();
            var list = service.Management.Experiments.List();
            list.MaxResults = 0;
            var experiments = await list.ExecuteAsync();
            return true;
        }
    }
}