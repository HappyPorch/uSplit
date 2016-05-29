using System;
using System.Threading.Tasks;
using Endzone.uSplit.Commands;
using Endzone.uSplit.Models;
using umbraco;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Task = System.Threading.Tasks.Task;

namespace Endzone.uSplit.Pipeline
{
    public class ExperimentsPipeline : ApplicationEventHandler
    {
        protected async Task<TOut> ExecuteAsync<TOut>(Command<TOut> command)
        {
            return await command.ExecuteAsync();
        }

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            PublishedContentRequest.Prepared += PublishedContentRequestOnPrepared;
        }

        private void PublishedContentRequestOnPrepared(object sender, EventArgs eventArgs)
        {
            var request = sender as PublishedContentRequest;
            var task = SetUpExperiment(request);
            task.RunSynchronously();
            task.Wait();
        }

        private async Task SetUpExperiment(PublishedContentRequest request)
        {
            if (request.PublishedContent == null)
                return;

            var experiment = await ExecuteAsync(new FindExperiment()
            {
                PublishedContentId = request.PublishedContent.Id
            });

            if (experiment == null)
                return;

            var variation = experiment.Variations.GetRandom();
            //TODO: change VariedContent
            var variationContentId = variation.VariedContent.Id;
            var helper = new UmbracoHelper(UmbracoContext.Current);
            var variationPage = helper.TypedContent(variationContentId);

            request.PublishedContent = new VariedContent(request.PublishedContent, variationPage, 0);
        }
    }
}
