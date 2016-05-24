using System.Threading.Tasks;
using Endzone.uSplit.Commands;
using Umbraco.Web.Trees;

namespace Endzone.uSplit.Backoffice
{
    // ReSharper disable once InconsistentNaming
    public abstract class uSplitTreeController : TreeController
    {
        protected async Task<TOut> ExecuteAsync<TOut>(Command<TOut> command)
        {
            return await command.ExecuteAsync();
        }
    }
}