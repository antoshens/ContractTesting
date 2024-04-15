using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PactNet;
using System.Net;
using System.Text;

namespace ProducerTests
{
    public class ProviderStateMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDictionary<string, Action> _providerStates;

        public ProviderStateMiddleware(RequestDelegate next)
        {
            _next = next;
            _providerStates = new Dictionary<string, Action>
            {
                {
                    "when a given country exists", // The name of the provider state is specified in the given clause of an interaction in the consumer
                    HandleCountryIsFoundRequestState
                },
                {
                    "when no matches found",
                    HandleNoMatchesFoundRequestState
                }
            };
        }

        private void HandleNoMatchesFoundRequestState()
        {
            Console.WriteLine("This is a 'when no country provided' state handler.");
        }

        private void HandleCountryIsFoundRequestState()
        {
            Console.WriteLine("This is a 'when no currency provided' state handler.");
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/provider-states"))
            {
                await this.HandleProviderStatesRequest(context);
                await context.Response.WriteAsync(String.Empty);
            }
            else
            {
                await this._next(context);
            }
        }

        private async Task HandleProviderStatesRequest(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;

            if (context.Request.Method.ToUpper() == HttpMethod.Post.ToString().ToUpper() &&
                context.Request.Body != null)
            {
                string jsonRequestBody = String.Empty;
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    jsonRequestBody = await reader.ReadToEndAsync();
                }

                var providerState = JsonConvert.DeserializeObject<ProviderState>(jsonRequestBody);

                // A null or empty provider state key must be handled
                if (providerState != null && !String.IsNullOrEmpty(providerState.State))
                {
                    _providerStates[providerState.State].Invoke();
                }
            }
        }
    }
}
