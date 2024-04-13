using Xunit.Abstractions;
using PactNet;
using PactNet.Output.Xunit;

namespace ConsumerTests
{
    public static class TestSetup
    {
        public static IPactBuilderV3 SetupPact(string consumer, string provider, ITestOutputHelper output)
        {
            var config = new PactConfig
            {
                PactDir = "../../../pacts/",
                Outputters = new[]
                {
                    new XunitOutput(output)
                },
                LogLevel = PactLogLevel.Debug
            };

            // You can select which specification version you wish to use by calling either V2 or V3
            IPactV3 pact = Pact.V3(consumer, provider, config);

            return pact.WithHttpInteractions();
        }
    }
}
