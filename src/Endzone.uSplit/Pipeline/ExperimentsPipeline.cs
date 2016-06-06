using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Endzone.uSplit.Commands;
using Endzone.uSplit.Models;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Experiment = Endzone.uSplit.Models.Experiment;

namespace Endzone.uSplit.Pipeline
{
    public class ExperimentsPipeline : ApplicationEventHandler
    {
        private readonly Random random;

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
            GlobalFilters.Filters.Add(new VariationReportingFilterAttribute());
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
            var experiments = new GetCachedExperiments().ExecuteAsync().Result;

            var googleExperiment =
                experiments?
                    .FirstOrDefault(e => Experiment.ExtractNodeIdFromExperimentName(e.Name) == request.PublishedContent.Id);

            if (googleExperiment == null)
                return;

            var experiment = new Experiment(googleExperiment);
            if (!IsValidExperiment(experiment))
                return;

            //set the experiment we are running
            HttpContext.Current.Items[Constants.HttpContextExperimentKey] = experiment;

            //check for a license
            if (!LicenseHelper.HasValidLicense() && !LicenseHelper.IsCoveredInFreeTrial(googleExperiment))
                return;

            //Has the user been previously exposed to this experiment?
            var variationId = GetAssignedVariation(request, experiment.Id);
            if (variationId != null)
            {
                ShowVariation(request, experiment, variationId.Value);
                return;
            }

            //Should the user be included in the experiment?
            if (!ShouldVisitorParticipate(experiment))
            {
                ShowVariation(request, experiment, -1);
                return;
            }

            //Choose a variation for the user
            variationId = SelectVariation(experiment);
            ShowVariation(request, experiment, variationId.Value);

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
            AssignVariationToUser(request,experiment.Id, variationId);

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

        private int? GetAssignedVariation(PublishedContentRequest request, string experimentId)
        {
            //get a cookie
            var cookie = request.RoutingContext.UmbracoContext.HttpContext.Request.Cookies.Get(Constants.Cookies.CookieVariationName + experimentId);
            if (cookie != null)
            {
                int variationId = 0;
                if (int.TryParse(cookie.Value, out variationId))
                {
                    return variationId;
                }
            }
            return null;
        }

        private void AssignVariationToUser(PublishedContentRequest request, string experimentId, int variationId)
        {
            //set a cookie
            var cookie = new HttpCookie(Constants.Cookies.CookieVariationName + experimentId)
            {
                Expires = DateTime.Now.AddDays(30),
                Value = variationId.ToString()
            };
            request.RoutingContext.UmbracoContext.HttpContext.Response.Cookies.Set(cookie);
        }
    }
}
