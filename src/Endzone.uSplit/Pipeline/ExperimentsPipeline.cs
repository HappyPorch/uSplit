using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Endzone.uSplit.Commands;
using Endzone.uSplit.Models;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Experiment = Endzone.uSplit.Models.Experiment;

namespace Endzone.uSplit.Pipeline
{
    public class ExperimentsPipeline : ApplicationEventHandler
    {
        private readonly Random random;
        private readonly Logger logger;

        public ExperimentsPipeline()
        {
            random = new Random();
            logger = Logger.CreateWithDefaultLog4NetConfiguration();
        }

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            PublishedContentRequest.Prepared += PublishedContentRequestOnPrepared;
            GlobalFilters.Filters.Add(new VariationReportingActionFilterAttribute());
        }

        private void PublishedContentRequestOnPrepared(object sender, EventArgs eventArgs)
        {
            logger.Debug(GetType(), "uSplit is processing a request");
            var request = sender as PublishedContentRequest;
            var originalContent = request?.PublishedContent;
            var originalTemplateAlias = request?.TemplateAlias;
            try
            {
                Process(request);
            }
            catch (Exception e)
            {
                logger.Error(GetType(), "Exception has been thrown when uSplit processed the request", e);
                if (request != null)
                {
                    if (request.PublishedContent != originalContent)
                        request.PublishedContent = originalContent;

                    if (request.TemplateAlias != originalTemplateAlias)
                        request.TrySetTemplate(originalTemplateAlias);
                }
            }
        }

        private void Process(PublishedContentRequest request)
        {
            //Are we rendering a published content (sanity check)?
            if (request?.PublishedContent == null)
                return;

            //Are there any experiments running
            var experiments = new GetApplicableCachedExperiments
            {
                ContentId = request.PublishedContent.Id
            }.ExecuteAsync().Result;

            //variations of the same content will be applied in the order the experiments were created,
            //if they override the same property variation of the newest experiment wins
            var variationsToApply = new List<IPublishedContentVariation>();

            foreach (var experiment in experiments)
            {
                var experimentId = experiment.GoogleExperiment.Id;

                //Has the user been previously exposed to this experiment?
                var assignedVariationId = GetAssignedVariation(request, experiment.Id);
                if (assignedVariationId != null)
                {
                    if (ShouldApplyVariation(experiment, assignedVariationId.Value))
                    {
                        var variationContent = GetVariationContent(experiment, assignedVariationId.Value);
                        variationsToApply.Add(new PublishedContentVariation(variationContent, experimentId, assignedVariationId.Value));
                    }
                }
                //Should the user be included in the experiment?
                else if (ShouldVisitorParticipate(experiment))
                {
                    //Choose a variation for the user
                    var variationId = SelectVariation(experiment);
                    AssignVariationToUser(request, experiment.Id, variationId);
                    if (ShouldApplyVariation(experiment, variationId))
                    {
                        var variationContent = GetVariationContent(experiment, variationId);
                        variationsToApply.Add(new PublishedContentVariation(variationContent, experimentId, variationId));
                    }
                }
                else
                {
                    AssignVariationToUser(request, experiment.Id, -1);
                }
            }

            //the visitor is excluded or we didn't find the content needed for variations, just show original
            if (variationsToApply.Count <= 0)
                return;

            var variedContent = new VariedContent(request.PublishedContent, variationsToApply.ToArray());
            request.PublishedContent = variedContent;
            request.SetIsInitialPublishedContent();

            //Reset the published content now we have set the initial content
            request.PublishedContent = PublishedContentModelFactoryResolver.Current.Factory.CreateModel(variedContent);

            request.TrySetTemplate(variedContent.GetTemplateAlias());
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

        private bool ShouldApplyVariation(Experiment experiment, int variationId)
        {
            if (variationId == -1)
                return false; //user is excluded 

            var variation = experiment.Variations[variationId];
            if (!variation.IsActive)
                return false;

            return true;
        }

        private IPublishedContent GetVariationContent(Experiment experiment, int variationId)
        {
            if (experiment.ServerSide)
                return null;

            var variation = experiment.Variations[variationId];
            var pageId = variation.VariedContent.Id;
            var helper = new UmbracoHelper(UmbracoContext.Current);
            var variationPage = helper.TypedContent(pageId);

            if (variationPage == null)
            {
                logger.Warn(GetType(), $"Cannot find the content (id {pageId}) for variation #{variationId} of {experiment.Id} experiment! " +
                    $"This might occur on testing environments if you use live Google Analytics account and in such cases can be ignored. " +
                    $"Alternatively the variation page might have been deleted or unpublished, check the experiment and make sure all variations have mathcing content.");
            }

            return variationPage;
        }

        private int? GetAssignedVariation(PublishedContentRequest request, string experimentId)
        {
            //get a cookie
            var cookie = request.RoutingContext.UmbracoContext.HttpContext.Request.Cookies.Get(Constants.Cookies.CookieVariationName + experimentId);
            if (cookie != null)
            {
                if (int.TryParse(cookie.Value, out var variationId))
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
