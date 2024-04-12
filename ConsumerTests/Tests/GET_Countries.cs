using Consumer.Data;
using PactNet;
using PactNet.Matchers;
using System.Net;
using Xunit.Abstractions;

namespace ConsumerTests.Tests
{
    public class GET_Countries
    {
        private readonly IPactBuilderV3 pact;

        public GET_Countries(ITestOutputHelper output)
        {
            this.pact = TestSetup.SetupPact("Consumer", "CoreLogicMembers", output);
        }

        [Fact]
        [Trait("Category", "Contract")]
        public async Task Should_Return_Countries_Object_Array()
        {
            var affiliationId = "GMAA";
            var exampleRequest = this.GetExpectedCountriesRequest();
            var exampleResponse = this.GetExpectedCountriesResponse();

            // Create the expectation(s) using the fluent API, first the request and then the response
            this.pact
                .UponReceiving("a request to retrieve countries filtered by a given data")
                .WithRequest(HttpMethod.Get, "/api/CoreLogic/Members")
                .WithQuery("affiliationid", affiliationId)
                .WithHeader("Authorization", "ApiKeynMUtYvj2t4VsfrSXEgKkdhLZBvQSuNk3PATJJmMF")
                .WithJsonBody(exampleRequest)
                .WillRespond()
                .WithStatus(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json; charset=utf-8")
                .WithJsonBody(Match.MinType(exampleResponse[0], 1));

            await this.pact.VerifyAsync(async ctx =>
            {
                // All API calls must happen inside this lambda, using the URL provided by the context argument
                var providerClientFactory = new ProviderClientFactory(ctx.MockServerUri);
                var client = new CountryService(providerClientFactory);
                var countries = await client.GetCountriesAsync(exampleRequest);

                Assert.Equal(countries, exampleResponse);
            });
        }

        private List<Country> GetExpectedCountriesResponse()
        {
            var responseCollection = new List<Country>();
            var testCountry = new Country()
            {
                Name = new CountryName
                {
                    NativeName = new Dictionary<string, Translation> { { "Ukraine", new Translation { Official = "Ukraine", Common = "Ukraine" } } }
                }
            };
            responseCollection.Add(testCountry);

            return responseCollection;
        }

        private CountryFilter GetExpectedCountriesRequest()
        {
            return new CountryFilter
            {
                Name = "Ukraine",
                Population = 10,
                Limit = 1,
                SortOrder = SortOrder.asc
            };
        }
    }
}
