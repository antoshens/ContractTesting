using Consumer.Data;
using PactNet;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit.Abstractions;

namespace ConsumerTests.Tests
{
    public class POST_Currency
    {
        private readonly IPactBuilderV3 pact;
        const string ErrorMessage = "Unable to add new currency";

        public POST_Currency(ITestOutputHelper output)
        {
            pact = TestSetup.SetupPact("Currency Consumer", "Currency Provider", output);
        }

        [Fact]
        [Trait("Currency", "Contract")]
        public async Task Should_Return_200_Status_Code()
        {
            var route = $"api/currency/Sweden";
            var expectedRequestBody = new Currency
            {
                Name = "US Dollar",
                Symbol = "$"
            };
            var jsonBody = JsonSerializer.Serialize(expectedRequestBody);

            pact
                .UponReceiving("a request to add a new currency to the existed country")
                .WithRequest(HttpMethod.Post, $"/{route}")
                //.WithJsonBody(expectedRequestBody)
                .WithBody(jsonBody, "application/json")
                .WithHeader("Content-Type", "application/json")
                .WillRespond()
                .WithStatus(HttpStatusCode.OK)
                .WithHeader("Content-Type", "*/*");

            await pact.VerifyAsync(async ctx =>
            {
                var providerClientFactory = new ProviderClientFactory(ctx.MockServerUri);
                var client = providerClientFactory.CreateClient();

                var body = JsonContent.Create(expectedRequestBody);
                var response = await client.PostAsync(route, body);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            });
        }

        [Fact]
        [Trait("Currency", "Contract")]
        public async Task Should_Return_400_If_No_Country_Provided()
        {
            var route = $"api/currency";
            var expectedRequestBody = new Currency
            {
                Name = "US Dollar",
                Symbol = "$"
            };
            var jsonBody = JsonSerializer.Serialize(expectedRequestBody);

            pact
                .UponReceiving("a request to add a new currency without a country")
                .WithRequest(HttpMethod.Post, $"/{route}")
                .WithBody(jsonBody, "application/json")
                .WithHeader("Content-Type", "application/json")
                .WillRespond()
                .WithStatus(HttpStatusCode.BadRequest)
                .WithHeader("Content-Type", "*/*")
                .WithJsonBody(ErrorMessage);

            await pact.VerifyAsync(async ctx =>
            {
                var providerClientFactory = new ProviderClientFactory(ctx.MockServerUri);
                var client = providerClientFactory.CreateClient();

                var body = JsonContent.Create(expectedRequestBody);
                var response = await client.PostAsync(route, body);

                var jsonResponse = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                Assert.Contains(ErrorMessage, jsonResponse);
            });
        }

        [Fact]
        [Trait("Currency", "Contract")]
        public async Task Should_Return_400_If_No_Currency_Provided()
        {
            var route = $"api/currency/Sweden";

            pact
                .UponReceiving("a request to add a new currency without a body")
                .WithRequest(HttpMethod.Post, $"/{route}")
                .WillRespond()
                .WithStatus(HttpStatusCode.BadRequest)
                .WithHeader("Content-Type", "*/*")
                .WithJsonBody(ErrorMessage);

            await pact.VerifyAsync(async ctx =>
            {
                var providerClientFactory = new ProviderClientFactory(ctx.MockServerUri);
                var client = providerClientFactory.CreateClient();

                var response = await client.PostAsync(route, null);

                var jsonResponse = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                Assert.Contains(ErrorMessage, jsonResponse);
            });
        }
    }
}
