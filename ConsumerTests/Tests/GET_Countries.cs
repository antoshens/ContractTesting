using Consumer.Data;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            pact = TestSetup.SetupPact("Countries Consumer", "Countries Provider", output);
        }

        [Fact]
        [Trait("Countries", "Contract")]
        public async Task Should_Return_One_Matched_Country()
        {
            var exampleQueryParams = GetExpectedCountriesRequest();
            var exampleResponse = GetExpectedCountriesResponse();

            // Create the expectation(s) using the fluent API, first the request and then the response
            pact
                .UponReceiving("A valid request to retrieve filtered countries")
                    .WithRequest(HttpMethod.Get, "/api/countries")
                    .WithQuery("name", exampleQueryParams["name"])
                    .WithQuery("limit", exampleQueryParams["limit"])
                    .WithQuery("population", exampleQueryParams["population"])
                    .WithQuery("sortOrder", exampleQueryParams["sortOrder"])
                .WillRespond()
                    .WithStatus(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "*/*")
                    .WithJsonBody(Match.Type(exampleResponse));

            await pact.VerifyAsync(async ctx =>
            {
                // All API calls must happen inside this lambda, using the URL provided by the context argument
                var providerClientFactory = new ProviderClientFactory(ctx.MockServerUri);
                var client = providerClientFactory.CreateClient();
                var response = await client.GetAsync(QueryHelpers.AddQueryString("api/countries", exampleQueryParams));

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JObject.Parse(jsonResponse);
                var countries = result["value"]?.ToObject<List<Country>>();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Single(countries);
                Assert.Equal(exampleResponse[0].Name.Common, countries[0].Name.Common);
            });
        }

        [Fact]
        [Trait("Countries", "Contract")]
        public async Task Should_Return_Empty_Array_If_No_Matches_Found()
        {
            var exampleQueryParams = GetWrongExpectedCountriesRequest();

            // Create the expectation(s) using the fluent API, first the request and then the response
            pact
                .UponReceiving("A valid request to retrieve filtered countries")
                    .Given("when no matches found")
                    .WithRequest(HttpMethod.Get, "/api/countries")
                    .WithQuery("name", exampleQueryParams["name"])
                    .WithQuery("limit", exampleQueryParams["limit"])
                    .WithQuery("population", exampleQueryParams["population"])
                    .WithQuery("sortOrder", exampleQueryParams["sortOrder"])
                .WillRespond()
                    .WithStatus(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "*/*")
                    .WithJsonBody(Array.Empty<Country>());

            await pact.VerifyAsync(async ctx =>
            {
                // All API calls must happen inside this lambda, using the URL provided by the context argument
                var providerClientFactory = new ProviderClientFactory(ctx.MockServerUri);
                var client = providerClientFactory.CreateClient();
                var response = await client.GetAsync(QueryHelpers.AddQueryString("api/countries", exampleQueryParams));

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var countries = JsonConvert.DeserializeObject<List<Country>>(jsonResponse);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Empty(countries);
            });
        }

        private List<Country> GetExpectedCountriesResponse()
        {
            var responseCollection = new List<Country>();
            var testCountry = new Country()
            {
                Name = new CountryName
                {
                    NativeName = new Dictionary<string, Translation> { { "Ukraine", new Translation() } }
                },
                Capital = ["Kyiv"],
                Population = 10,
                Area = 12.5,
                StartOfWeek = "Monday",
                Car = new Car { Side = "Right" }
            };
            responseCollection.Add(testCountry);

            return responseCollection;
        }

        private Dictionary<string, string?> GetExpectedCountriesRequest()
        {
            var queryParams = new Dictionary<string, string>
            {
                { "name", "Ukraine" },
                { "limit", "1" },
                { "sortOrder", "asc" },
                { "population", "10" }
            };

            return queryParams;
        }

        private Dictionary<string, string?> GetWrongExpectedCountriesRequest()
        {
            var queryParams = new Dictionary<string, string>
            {
                { "name", "Ukraine123" },
                { "limit", "1" },
                { "sortOrder", "asc" },
                { "population", "10" }
            };

            return queryParams;
        }
    }
}
