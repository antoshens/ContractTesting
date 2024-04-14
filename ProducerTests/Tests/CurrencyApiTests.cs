using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Xunit.Abstractions;
using PactNet.Output.Xunit;
using PactNet.Verifier;

namespace ProducerTests.Tests
{
    public class CurrencyApiTests : IDisposable
    {
        private string _providerUri { get; }
        private IWebHost _webHost { get; }
        private ITestOutputHelper _outputHelper { get; }

        public CurrencyApiTests(ITestOutputHelper output)
        {
            _outputHelper = output;
            _providerUri = "http://localhost:9000";

            _webHost = WebHost.CreateDefaultBuilder()
                .UseUrls(_providerUri)
                .UseStartup<TestStartup>()
                .Build();

            _webHost.Start();
        }

        [Fact]
        public void Ensure_Currency_Api_Honours_Pact_With_Consumer()
        {
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

            pactVerifier
                .ServiceProvider("Provider", new Uri(_providerUri))
                .WithFileSource(pactFile)
                .WithProviderStateUrl(new Uri($"{_providerUri}/provider-states"))
                .Verify();
        }

        #region IDisposable Support
        private bool disposedValue = false; // Needed to detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _webHost.StopAsync().GetAwaiter().GetResult();
                    _webHost.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}