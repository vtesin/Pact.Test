using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PactNet;
using PactNet.Mocks.MockHttpService;

namespace Test.Pact.App.MockService
{
    public class ConsumerMtgApiPact
    {
        private string _pactDir;
        const string ProviderName = "Cardservice";
        const string ClientName = "Cardclient";

        public IPactBuilder PactBuilder { get; private set; }
        public IMockProviderService MockProviderService { get; private set; }

        public int MockServerPort => 8080;
        public string MockProviderServiceBaseUri => $"http://localhost:{MockServerPort}/";

        public ConsumerMtgApiPact()
        {
            var currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            _pactDir = Path.Combine(currentDirectory, @"..\..\..\Docs");
            var logDir = Path.Combine(currentDirectory, @"..\..\..\Docs");

            PactBuilder = new PactBuilder(new PactConfig
                {PactDir = _pactDir, LogDir = logDir, SpecificationVersion = "2.0.0"});

            PactBuilder
                .ServiceConsumer(ClientName)
                .HasPactWith(ProviderName); // Provider Name

            MockProviderService = PactBuilder.MockService(MockServerPort);
            //Configure the http mock server

            //MockProviderService = PactBuilder.MockService(MockServerPort, false);
            // By passing true as the second param, you can enabled SSL. 
            // This will however require a valid SSL certificate installed and bound 
            // with netsh (netsh http add sslcert ipport=0.0.0.0:port certhash=thumbprint appid={app-guid}) 
            // on the machine running the test. See https://groups.google.com/forum/#!topic/nancy-web-framework/t75dKyfgzpg
            //or


            //MockProviderService = PactBuilder.MockService(MockServerPort);
            //By passing true as the bindOnAllAdapters parameter the http mock server will be 
            // able to accept external network requests, but will require admin privileges in order to run

            //MockProviderService = PactBuilder.MockService(MockServerPort, new JsonSerializerSettings());
            //You can also change the default Json serialization settings using this overload		
        }

        public void Dispose()
        {
            PactBuilder.Build();
            //NOTE: Will save the pact file once finished
            var pactPublisher =
                new PactPublisher("https://adesso.pact.dius.com.au/",
                    new PactUriOptions("Vm6YWrQURJ1T7mDIRiKwfexCAc4HbU", "aLerJwBhpEcN0Wm88Wgvs45AR9dXpc"));

            pactPublisher.PublishToBroker(Path.Combine(_pactDir, "cardclient-cardservice.json"),
                "1.0.0", 
                new[] {"feature-Dragon"});
        }
    }
}