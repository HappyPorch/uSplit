using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Endzone.uSplit.Models;
using Endzone.uSplit.Pipeline;
using Moq;
using NUnit.Framework;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Models;
using Umbraco.Tests.TestHelpers;
using Umbraco.Web;
using Umbraco.Web.Routing;

namespace Endzone.uSplit.Tests
{
    [TestFixture]
    public class VariationReportingFilterAttributeTests : BaseRoutingTest
    {
        private UmbracoContext umbracoContext;
        private RoutingContext routingContext;
        private HttpContextBase httpContext;
        private IUmbracoSettingsSection settings;
        private VariedContent variedContent;
        private Mock<IExperiment> experimentMock;
        private Mock<HttpResponseBase> responseMock;
        private HttpResponseBase response;
        private HtmlHelper htmlHelper;
        private Mock<ActionExecutedContext> actionExecutedMock;
        private Mock<ResultExecutedContext> resultExecutedMock;
        private Mock<IPublishedContentVariation> publishedContentVariationMock;

        public override void Initialize()
        {
            base.Initialize();

            settings = SettingsForTests.GenerateMockSettings();
            SettingsForTests.ConfigureSettings(settings);

            var routeData = new RouteData();
            routeData.Values["controller"] = "DummyTestController";
            routeData.Values["action"] = "Index";

            routingContext = GetRoutingContext("http://localhost", -1, routeData, umbracoSettings: settings);
            umbracoContext = routingContext.UmbracoContext;

            httpContext = umbracoContext.HttpContext;
            var httpContextMock = Mock.Get(httpContext);
            httpContextMock.Setup(x => x.Items).Returns(
                new ListDictionary()
                {
                    { HttpContextExtensions.HttpContextItemName, umbracoContext}
                }
            );

            response = httpContext.Response;
            responseMock = Mock.Get(response);
            responseMock.Setup(r => r.ContentType).Returns("text/html");
            responseMock.SetupProperty(r => r.Filter, Stream.Null);

            var experimentsPipeline = new ExperimentsPipeline();
            experimentsPipeline.OnApplicationStarted(null, ApplicationContext);

            var originalMock = new Mock<IPublishedContent>();
            originalMock.Setup(c => c.Properties).Returns(new List<IPublishedProperty>());
            experimentMock = new Mock<IExperiment>();
            experimentMock.Setup(e => e.Id).Returns("TESTPERIMENT");

            publishedContentVariationMock = new Mock<IPublishedContentVariation>();
            publishedContentVariationMock.SetupAllProperties();
            publishedContentVariationMock.SetupProperty(p => p.Content, originalMock.Object);

            variedContent = new VariedContent(originalMock.Object, new [] { publishedContentVariationMock.Object });

            var docRequest = new PublishedContentRequest(
                new Uri("/", UriKind.Relative),
                routingContext,
                settings.WebRouting,
                s => Enumerable.Empty<string>())
            {
                PublishedContent = variedContent
            };
            docRequest.SetIsInitialPublishedContent();

            umbracoContext.PublishedContentRequest = docRequest;

            var viewContext = new Mock<ViewContext>();
            viewContext.Setup(c => c.HttpContext).Returns(httpContext);

            actionExecutedMock = new Mock<ActionExecutedContext>(MockBehavior.Strict);
            actionExecutedMock.Setup(f => f.IsChildAction).Returns(false);
            actionExecutedMock.Setup(f => f.HttpContext).Returns(httpContext);

            resultExecutedMock = new Mock<ResultExecutedContext>(MockBehavior.Strict);
            resultExecutedMock.Setup(f => f.HttpContext).Returns(httpContext);

            htmlHelper = new HtmlHelper(viewContext.Object, Mock.Of<IViewDataContainer>());
        }

        [Test]
        public void NormalRequest_SetsFilter()
        {
            var attribute = new VariationReportingActionFilterAttribute();
            attribute.OnActionExecuted(actionExecutedMock.Object);
            responseMock.VerifySet(r => r.Filter, Times.Once());
        }

        [Test]
        public void NormalRequest_AutomaticallyReported_ReportedOnce()
        {
            var attribute = new VariationReportingActionFilterAttribute();
            attribute.OnActionExecuted(actionExecutedMock.Object);
            ExecuteFilter(responseMock.Object.Filter);
            attribute.OnResultExecuted(resultExecutedMock.Object);

            //accessed id means it was used for reporting
            publishedContentVariationMock.Verify(e => e.GoogleExperimentId, Times.Once);
            publishedContentVariationMock.Verify(e => e.GoogleVariationId, Times.Once);
        }

        /// <summary>
        /// Sometimes filtering simply will not work and manual reporting will need to be used.
        /// https://github.com/EndzoneSoftware/uSplit/issues/4
        /// </summary>
        [Test]
        public void CannotSetFilter_SetsFlag()
        {
            responseMock.SetupSet(r => r.Filter).Throws<HttpException>();

            var attribute = new VariationReportingActionFilterAttribute();

            Assert.DoesNotThrow(() => attribute.OnActionExecuted(actionExecutedMock.Object));
            Assert.IsFalse((bool)httpContext.Items[Constants.VariationReportedHttpContextItemsKey]);
        }

        [Test]
        public void CannotSetFilter_NotReportedManually_ThrowsException()
        {
            responseMock.SetupSet(r => r.Filter).Throws<HttpException>();

            var attribute = new VariationReportingActionFilterAttribute();
            attribute.OnActionExecuted(actionExecutedMock.Object);

            Assert.IsFalse((bool)httpContext.Items[Constants.VariationReportedHttpContextItemsKey]);
            Assert.Throws<NotSupportedException>(() => attribute.OnResultExecuted(resultExecutedMock.Object));
        }

        [Test]
        public void CannotSetFilter_ReportedManually_AllGood()
        {
            responseMock.SetupSet(r => r.Filter).Throws<HttpException>();

            var attribute = new VariationReportingActionFilterAttribute();

            attribute.OnActionExecuted(actionExecutedMock.Object);
            HtmlHelperExtensions.RenderAbTestingScriptTags(htmlHelper);

            Assert.IsTrue((bool)httpContext.Items[Constants.VariationReportedHttpContextItemsKey]);
            Assert.DoesNotThrow(() => attribute.OnResultExecuted(resultExecutedMock.Object));
        }


        [Test]
        public void NormalRequest_ReportedManually_ReportedOnce()
        {
            var attribute = new VariationReportingActionFilterAttribute();
            attribute.OnActionExecuted(actionExecutedMock.Object);
            HtmlHelperExtensions.RenderAbTestingScriptTags(htmlHelper);
            attribute.OnResultExecuted(resultExecutedMock.Object);

            responseMock.VerifySet(r => r.Filter, Times.Once());

            ExecuteFilter(responseMock.Object.Filter);

            //accessed id means it was used for reporting
            publishedContentVariationMock.Verify(e => e.GoogleExperimentId, Times.Once);
            publishedContentVariationMock.Verify(e => e.GoogleVariationId, Times.Once);
        }

        private void ExecuteFilter(Stream stream)
        {
            Assert.NotNull(stream);
            var filter = stream as VariationReportingHttpResponseFilter;
            filter.Write(Encoding.UTF8.GetBytes("<head>"), 0, 6);
        }

        public class DummyTestController : Controller
        {
            public ActionResult Index()
            {
                return Content("Hi!");
            }
        }
    }
}
