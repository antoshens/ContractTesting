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
        const string ErrorMessage = "Unable to add new currency.";
        const string SuccessMessage = "The currency has been added.";

        public POST_Currency(ITestOutputHelper output)
        {
            pact = TestSetup.SetupPact("Currency Consumer", "Currency Provider", output);
        }

        [Fact]
        [Trait("Currency", "Contract")]
        public async Task Should_Return_200_Status_Code()
        {
            // Arrange
            var route = $"api/countries/currency/Sweden";
            var expectedRequestBody = new Currency
            {
                Name = "US Dollar",
                Symbol = "$"
            };
            var jsonBody = JsonSerializer.Serialize(expectedRequestBody);

            pact
                .UponReceiving("A valid request to add new currency")
                    .WithRequest(HttpMethod.Post, $"/{route}")
                    //.WithJsonBody(expectedRequestBody)
                    .WithBody(jsonBody, "application/json")
                .WillRespond()
                    .WithStatus(HttpStatusCode.OK)
                    .WithBody(SuccessMessage, "text/plain; charset=utf-8");

            // Act / Assert
            await pact.VerifyAsync(async ctx =>
            {
                var providerClientFactory = new ProviderClientFactory(ctx.MockServerUri);
                var client = providerClientFactory.CreateClient();

                var body = JsonContent.Create(expectedRequestBody);
                var response = await client.PostAsync(route, body);

                var jsonResponse = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Contains(SuccessMessage, jsonResponse);
            });
        }

        [Fact]
        [Trait("Currency", "Contract")]
        public async Task Should_Return_400_If_Invalid_Country_Provided()
        {
            // Arrange
            var route = $"api/countries/currency/Sweden123";
            var expectedRequestBody = new Currency
            {
                Name = "US Dollar",
                Symbol = "$"
            };
            var jsonBody = JsonSerializer.Serialize(expectedRequestBody);

            pact
                .UponReceiving("An invalid add currency request")
                    .Given("when no country provided")
                    .WithRequest(HttpMethod.Post, $"/{route}")
                    .WithBody(jsonBody, "application/json")
                .WillRespond()
                    .WithStatus(HttpStatusCode.BadRequest)
                    .WithBody(ErrorMessage, "text/plain; charset=utf-8");

            // Act / Assert
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
        public async Task Should_Return_415_If_No_Currency_Provided()
        {
            // Arrange
            var route = $"api/countries/currency/Sweden";

            pact
                .UponReceiving("An invalid add currency request")
                    .Given("when no currency provided")
                    .WithRequest(HttpMethod.Post, $"/{route}")
                .WillRespond()
                    .WithStatus(HttpStatusCode.UnsupportedMediaType);

            // Act / Assert
            await pact.VerifyAsync(async ctx =>
            {
                var providerClientFactory = new ProviderClientFactory(ctx.MockServerUri);
                var client = providerClientFactory.CreateClient();

                var response = await client.PostAsync(route, null);

                Assert.Equal(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
            });
        }
    }
}
