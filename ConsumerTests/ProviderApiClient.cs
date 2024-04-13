namespace ConsumerTests
{
    internal class ProviderClientFactory : IHttpClientFactory
    {
        private readonly Uri _uri;

        public ProviderClientFactory(Uri providerUri)
        {
            _uri = providerUri;
        }

        public HttpClient CreateClient(string name)
        {
            return new HttpClient
            {
                BaseAddress = new Uri(_uri, "api")
            };
        }
    }
}
