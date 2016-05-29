using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Endzone.uSplit.Commands;
using Endzone.uSplit.Models;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Task = System.Threading.Tasks.Task;

namespace Endzone.uSplit.Pipeline
{
    public class ExperimentsPipeline : ApplicationEventHandler
    {
        private Random random;

        public ExperimentsPipeline()
        {
            random = new Random();
        }

        protected async Task<TOut> ExecuteAsync<TOut>(Command<TOut> command)
        {
            return await command.ExecuteAsync();
        }

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            PublishedContentRequest.Prepared += PublishedContentRequestOnPrepared;
            GlobalFilters.Filters.Add(new VariationReportingFilter());
        }

        private void PublishedContentRequestOnPrepared(object sender, EventArgs eventArgs)
        {
            var request = sender as PublishedContentRequest;
            SetUpExperiment(request);
        }

        private void SetUpExperiment(PublishedContentRequest request)
        {
            //Are we rendering a page?
            if (request.PublishedContent == null)
                return;

            //Is there an experiment running?
            var findExperimentCommand = new FindExperiment()
            {
                PublishedContentId = request.PublishedContent.Id
            };
            var googleExperiment = Task.Run(async () => await ExecuteAsync(findExperimentCommand)).Result;
            if (googleExperiment == null)
                return;

            var experiment = new Experiment(googleExperiment);
            if (!IsValidExperiment(experiment))
                return;

            //Has the user been previously exposed to this experiment?
            var variationId = GetAssignedVariation(experiment.Id);
            if (variationId != null)
            {
                ShowVariation(request, experiment, variationId.Value);
            }

            //Should the user be included in the experiment?
            if (!ShouldVisitorParticipate(experiment))
            {
                ShowVariation(request, experiment, -1);
            }

            //Choose a variation for the user
            variationId = SelectVariation(experiment);
            ShowVariation(request, experiment, variationId.Value);

            //TODO:Send Experiment Data
        }

        private int SelectVariation(Experiment experiment)
        {
            var shot = random.NextDouble();
            var total = 0d;

            for (int i = 0; i < experiment.Variations.Count; i++)
            {
                var variation = experiment.Variations[i];
                total += variation.GoogleVariation.Weight ?? 0;

                if (shot < total)
                    return i;
            }

            return 0;
        }

        private bool ShouldVisitorParticipate(Experiment experiment)
        {
            var coverage = experiment.GoogleExperiment.TrafficCoverage ?? 1;
            var r = random.NextDouble();
            return r <= coverage;
        }

        private void ShowVariation(PublishedContentRequest request, Experiment experiment, int variationId)
        {
            AssignVariationToUser(experiment.Id, variationId);

            if (variationId == -1) 
                return; //user is excluded 

            var variation = experiment.Variations[variationId];
            if (!variation.IsActive)
                return;

            var variationUmbracoId = variation.VariedContent.Id;
            var helper = new UmbracoHelper(UmbracoContext.Current);
            var variationPage = helper.TypedContent(variationUmbracoId);

            request.PublishedContent = new VariedContent(request.PublishedContent, variationPage, experiment, variationId);
            request.TrySetTemplate(request.PublishedContent.GetTemplateAlias());
        }

        private bool IsValidExperiment(Experiment experiment)
        {
            return experiment.IsUSplitExperiment && experiment.GoogleExperiment.Status == "RUNNING";
        }

        private int? GetAssignedVariation(string experimentId)
        {
            //TODO: check the cookie
            return null;
        }

        private void AssignVariationToUser(string experimentId, int variationId)
        {
            //TODO: set a cookie
        }
    }

    public class VariationReportingFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var request = filterContext.RequestContext.HttpContext.GetUmbracoContext().PublishedContentRequest;
            var response = filterContext.HttpContext.Response;
            var variedContent = request?.PublishedContent as VariedContent;
            if (response.ContentType == "text/html" && variedContent != null)
            {
                response.Filter = new InjectVariationReport(response.Filter, variedContent);
            }
        }
    }

    public class InjectVariationReport : MemoryStream
    {
        private readonly Stream outputStream;
        private readonly Func<string, string> filter;
        public InjectVariationReport(Stream outputStream, VariedContent content)
        {
            this.outputStream = outputStream;
            var js = "<script src=\"//www.google-analytics.com/cx/api.js\"></script>\n" +
                     "<script>\n" +
                     $"cxApi.setChosenVariation({content.VariationId},'{content.Experiment.Id}');\n" +
                     "</script>\n";
            filter = s => Regex.Replace(s, @"<head>", $"<head>\n{js}", RegexOptions.IgnoreCase);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            // capture the data and convert to string 
            var s = Encoding.UTF8.GetString(buffer);

            // filter the string
            s = filter(s);

            // write the data to stream 
            var outdata = Encoding.UTF8.GetBytes(s);
            outputStream.Write(outdata, 0, outdata.GetLength(0));
        }

        public override void Close()
        {
            outputStream.Close();
            base.Close();
        }
    }
}
