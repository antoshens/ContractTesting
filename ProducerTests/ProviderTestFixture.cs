using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace ProducerTests
{
    public class ProviderTestFixture : IDisposable
    {
        private IWebHost _webHost { get; }

        public string ProviderUri { get; }
        public string PactServiceUri { get; }
        public string PactBrockerUri { get; }

        public ProviderTestFixture()
        {
            ProviderUri = "http://localhost:7118";
            PactBrockerUri = "http://localhost:9292";

            var pactPort = GenarateRandomPort(); // Since the tests can and most likely be run in parallel, we need to always have a free port
            PactServiceUri = $"http://localhost:{pactPort}";

            _webHost = WebHost.CreateDefaultBuilder()
                .UseUrls(PactServiceUri)
                .UseStartup<TestStartup>()
                .Build();

            _webHost.Start();
        }

        private static int GenarateRandomPort()
        {
            int min = 9100;
            int max = 9999;
            var rnd = new Random();

            return rnd.Next(min, max);
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
