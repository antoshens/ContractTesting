using Xunit.Abstractions;
using PactNet.Output.Xunit;
using PactNet.Verifier;

namespace ProducerTests.Tests
{
    public class CountriesApiTests : IClassFixture<ProviderTestFixture>
    {
        private ProviderTestFixture _fixture;
        private ITestOutputHelper _outputHelper { get; }

        public CountriesApiTests(ProviderTestFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _outputHelper = output;
        }

        [Fact]
        public void Ensure_Country_Api_Honours_Pact_With_Consumer()
        {
            // Arrange
            var config = new PactVerifierConfig
            {
                Outputters = new []
                {
                    new XunitOutput(_outputHelper)
                },
                // Output verbose verification logs to the test output
                LogLevel = PactNet.PactLogLevel.Debug
            };

            var pactVerifier = new PactVerifier(config);
            var pactFile = new FileInfo(Path.Join("..", "..", "..", "..", "pacts", "Countries Consumer-Countries Provider.json"));

            // Act / Assert
            pactVerifier
                .ServiceProvider("Provider", new Uri(_fixture.ProviderUri))
                .WithFileSource(pactFile)
                //.WithProviderStateUrl(new Uri($"{_fixture.PactServiceUri}/provider-states"))
                .Verify();
        }
    }
}