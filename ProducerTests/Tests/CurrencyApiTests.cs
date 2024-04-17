using PactNet.Output.Xunit;
using PactNet.Verifier;
using Xunit.Abstractions;

namespace ProducerTests.Tests
{
    public class CurrencyApiTests : IClassFixture<ProviderTestFixture>
    {
        private ProviderTestFixture _fixture;
        private ITestOutputHelper _outputHelper { get; }

        public CurrencyApiTests(ProviderTestFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _outputHelper = output;
        }

        [Fact]
        public void Ensure_Currency_Api_Honours_Pact_With_Consumer()
        {
            // Arrange
            var config = new PactVerifierConfig
            {
                Outputters = new[]
                {
                    new XunitOutput(_outputHelper)
                },
                // Output verbose verification logs to the test output
                LogLevel = PactNet.PactLogLevel.Debug
            };

            var pactVerifier = new PactVerifier(config);
            var pactFile = new FileInfo(Path.Join("..", "..", "..", "..", "pacts", "Currency Consumer-Currency Provider.json"));

            // Act / Assert
            pactVerifier
                .ServiceProvider("Provider", new Uri(_fixture.ProviderUri))
                //.WithUriSource(new Uri(_fixture.ProviderUri))
                .WithFileSource(pactFile)
                .Verify();
        }
    }
}