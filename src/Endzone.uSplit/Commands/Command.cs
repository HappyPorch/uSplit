using System.Threading.Tasks;
using Umbraco.Web;

namespace Endzone.uSplit.Commands
{
    /// <summary>
    /// A command that can be executed.
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    /// <remarks>Implementations are meant to have properties with values used for execution.</remarks>
    public abstract class Command<TOut>
    {
        protected UmbracoContext UmbracoContext => UmbracoContext.Current;

        public abstract Task<TOut> ExecuteAsync();
    }
}