using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Endzone.uSplit.Commands;
using Newtonsoft.Json.Serialization;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Endzone.uSplit.API
{
    [PluginController(Constants.PluginName)]
    public abstract class BackofficeController : UmbracoAuthorizedApiController
    {
        protected async Task<TOut> ExecuteAsync<TOut>(Command<TOut> command)
        {
            return await command.ExecuteAsync();
        }

        public HttpResponseMessage CreateResponse(object responseMessageContent)
        {
            try
            {
                var httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, responseMessageContent, JsonMediaTypeFormatter.DefaultMediaType);
                var objectContent = httpResponseMessage.Content as ObjectContent;

                if (objectContent != null)
                {
                    var jsonMediaTypeFormatter = new JsonMediaTypeFormatter
                    {
                        SerializerSettings =
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }
                    };

                    httpResponseMessage.Content = new ObjectContent(objectContent.ObjectType, objectContent.Value, jsonMediaTypeFormatter);
                }

                return httpResponseMessage;
            }
            catch (Exception exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, exception.Message);
            }
        }
    }
}